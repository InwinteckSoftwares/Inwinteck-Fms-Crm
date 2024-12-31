using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Concurrent;
using Inwinteck_CRM.Controllers;
using Microsoft.AspNet.Identity;
using Inwinteck_CRM.Models;
using Inwinteck_CRM.Helpers;
using Newtonsoft.Json;
using System.Configuration;
using WebPush;

public class BossMessageHub : Hub
{
    public static ConcurrentDictionary<string, UserConnection> ConnectedUsers = new ConcurrentDictionary<string, UserConnection>();

    ChatController chatController = new ChatController();
    ApplicationDbContext db = new ApplicationDbContext();

    public override Task OnConnected()
    {
        var connectionId = Context.ConnectionId;
        var userEmail = Context.User?.Identity?.Name;
        if (userEmail != null)
        {
            if (!ConnectedUsers.ContainsKey(userEmail))
            {
                ConnectedUsers.TryAdd(userEmail, new UserConnection { UserId = userEmail, DisplayName = userEmail });
            }
            ConnectedUsers[userEmail].ConnectionIds.Add(connectionId);

            // Broadcast the updated user list to all clients
            Clients.All.updateConnectedUsers(ConnectedUsers.Values.ToList());
        }

        return base.OnConnected();
    }

    public override Task OnDisconnected(bool stopCalled)
    {
        var connectionId = Context.ConnectionId;
        var userEmail = Context.User?.Identity?.Name;
        if (userEmail != null && ConnectedUsers.TryGetValue(userEmail, out UserConnection userConnection))
        {
            userConnection.ConnectionIds.Remove(connectionId);

            if (!userConnection.ConnectionIds.Any())
            {
                ConnectedUsers.TryRemove(userEmail, out _);
            }
            // Broadcast the updated user list to all clients
            Clients.All.updateConnectedUsers(ConnectedUsers.Values.ToList());
        }
            
        return base.OnDisconnected(stopCalled);
    }

    public async Task Send(string message, string displayName, string recipientUserId = null)
    {
        var senderUserId = Context.User.Identity.Name;
        int messageId = await chatController.SaveMessageInwinteckInternal(displayName, message, "text", senderUserId, recipientUserId);
        var timestamp = DateTime.UtcNow.ToString("o");

        await SendPushNotificationsAsync(message, displayName, recipientUserId, senderUserId);
        if (!string.IsNullOrEmpty(recipientUserId))
        {
            if (ConnectedUsers.TryGetValue(recipientUserId, out UserConnection recipient))
            {
                foreach (var connId in recipient.ConnectionIds)
                {
                    // Send message to all of the recipient's active connections 
                    await Clients.Client(connId).broadcastMessage(messageId, displayName, message, timestamp, recipientUserId);
                }
            }
            // Send message back to the sender
            await Clients.Caller.broadcastMessage(messageId, displayName, message, timestamp, recipientUserId);
        }
        else
        {
            // Broadcast message to all connected clients
            await Clients.All.broadcastMessage(messageId, displayName, message, timestamp, null); // No recipient means broadcast to all
        }
    }


    public List<UserConnection> GetConnectedUsers()
    {
        return ConnectedUsers.Values.ToList(); // Convert the dictionary values to a list
    }

        public void BroadcastDeleteMessage(int messageId)
        {
            Clients.All.broadcastDeleteMessage(messageId);
        }

    public async Task LoadMessages(string userId)
    {
        var messages = db.ChatMessageInwinteckInternal
            .Where(m => m.RecipientUserId == null || m.SenderUserId == userId || m.RecipientUserId == userId)
            .OrderBy(m => m.Timestamp)
            .Select(m => new
            {
                m.Id,
                m.UserName,
                m.Message,
                m.Timestamp,  
                IsPrivate = !string.IsNullOrEmpty(m.RecipientUserId),
                m.RecipientUserId
            })
            .ToList(); 

        var formattedMessages = messages.Select(m => new
        {
            m.Id,
            m.UserName,
            m.Message,
            Timestamp = m.Timestamp.ToString("MM-dd-yyyy HH:mm:ss"),  // Format the Timestamp now
            m.IsPrivate,
            m.RecipientUserId
        }).ToList();

        await Clients.Caller.loadMessages(formattedMessages);
    }
    public void DeleteMessage(int messageId)
    {
        var message = db.ChatMessageInwinteckInternal.FirstOrDefault(m => m.Id == messageId);
        var currentUserName = Context.User.Identity.Name;

        if (message == null)
        {
            Clients.Caller.onDeleteMessageFailed("Message not found.");
            return;
        }
        if (message.UserName != currentUserName)
        {
            Clients.Caller.onDeleteMessageFailed("You can only delete your own messages.");
            return;
        }

        if ((DateTime.UtcNow - message.Timestamp).TotalMinutes > 10)
        {
            Clients.Caller.onDeleteMessageFailed("Cannot delete messages older than 10 minutes.");
            return;
        }
        db.ChatMessageInwinteckInternal.Remove(message);
        db.SaveChanges();
        Clients.All.broadcastDeleteMessage(messageId);
    }

    private async Task SendPushNotificationsAsync(string message, string senderDisplayName, string recipientUserId, string senderUserId)
    {
        var vapidPublicKey = ConfigurationManager.AppSettings["VapidPublicKey"];
        var vapidPrivateKey = ConfigurationManager.AppSettings["VapidPrivateKey"];
        var subject = "mailto:prashant.kori@inwinteck.com"; // Replace with your contact email

        var webPushClient = new WebPushClient();
        var vapidDetails = new VapidDetails(subject, vapidPublicKey, vapidPrivateKey);

        try
        {
            if (!string.IsNullOrEmpty(recipientUserId))
            {
                // Private message: send only to the recipient
                var subscriptions = (await PushSubscriptionStore.GetAllSubscriptionsAsync())
                    .Where(s => s.UserId == recipientUserId)
                    .ToList();

                foreach (var sub in subscriptions)
                {
                    var pushSubscription = new WebPush.PushSubscription(sub.Endpoint, sub.P256DH, sub.Auth);
                    var payload = JsonConvert.SerializeObject(new
                    {
                        title = "New Private Message",
                        message = message,
                        url = "/Chat/BossMessaging" // URL to open when notification is clicked
                    });

                    try
                    {
                        await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                    }
                    catch (WebPushException ex)
                    {
                        await PushSubscriptionStore.RemoveSubscriptionAsync(sub);
                        System.Diagnostics.Debug.WriteLine($"Failed to send notification: {ex.Message}");
                    }
                }
            }
            else
            {
                // Public message: send to all users except the sender
                var subscriptions = (await PushSubscriptionStore.GetAllSubscriptionsAsync())
                    .Where(s => s.UserId != senderUserId)
                    .ToList();

                foreach (var sub in subscriptions)
                {
                    var pushSubscription = new WebPush.PushSubscription(sub.Endpoint, sub.P256DH, sub.Auth);
                    var payload = JsonConvert.SerializeObject(new
                    {
                        title = "New Message",
                        message = message,
                        url = "/Chat/BossMessaging"
                    });

                    try
                    {
                        await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                    }
                    catch (WebPushException ex)
                    {
                        await PushSubscriptionStore.RemoveSubscriptionAsync(sub);
                        System.Diagnostics.Debug.WriteLine($"Failed to send notification: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Push notification error: {ex.Message}");
        }
    }


}

public class UserConnection
{
    public string UserId { get; set; }
    public string DisplayName { get; set; } 
    public List<string> ConnectionIds { get; set; } = new List<string>(); // Track multiple connection IDs for a user
}
