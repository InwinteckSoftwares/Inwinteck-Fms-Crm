using Inwinteck_CRM.Controllers;
using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Threading.Tasks;
using Inwinteck_CRM.Models;
using System; // Assuming this is where your EF models are located

namespace Inwinteck_CRM.Hubs
{
    public class ChatIndex : Hub
    {
        ChatController chatController = new ChatController();
        string timestamp = DateTime.UtcNow.ToString("o"); // ISO 8601 format

        // Send a text message to the chat group
        public async Task SendChatIndex(string message, string ticketId, string displayName)
        {
            int messageId = await chatController.SaveMessage(displayName, message, ticketId, "text");
            await Clients.Group(ticketId).broadcastMessage(messageId, displayName, message, timestamp);
        }

        // Send an image message to the chat group
        public async Task SendImageChatIndex(string imageUrl, string ticketId, string displayName)
        {
            int messageId = await chatController.SaveMessage(displayName, imageUrl, ticketId, "image");
            await Clients.Group(ticketId).broadcastImage(messageId, displayName, imageUrl, timestamp);
        }

        // Send a video message to the chat group
        public async Task SendVideoChatIndex(string videoUrl, string ticketId, string displayName)
        {
            int messageId = await chatController.SaveMessage(displayName, videoUrl, ticketId, "video");
            await Clients.Group(ticketId).broadcastVideo(messageId, displayName, videoUrl, timestamp);
        }

        // Broadcast delete message to the group
        public void BroadcastDeleteMessageChatIndex(int messageId, string ticketId)
        {
            Clients.Group(ticketId).broadcastDeleteMessage(messageId);
        }

    }
}
