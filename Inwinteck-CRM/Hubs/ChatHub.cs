using Inwinteck_CRM.Controllers;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace Inwinteck_CRM.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        ChatController chatController = new ChatController();
        string timestamp = DateTime.UtcNow.ToString("o"); // ISO 8601 format

        //-----------Methods for ChatHdToSource chat page-------------------//
        public async Task Send(string message, string ticketId, string displayName)
        {
            int  messageId = await chatController.SaveMessageHdToSource(displayName, message, "text");
            await Clients.All.broadcastMessage(messageId, displayName, message, timestamp);
        }
        public async Task SendImage(string imageUrl, string ticketId, string displayName)
        {
            int messageId = await chatController.SaveMessageHdToSource(displayName, imageUrl, "image");
            await Clients.All.broadcastImage(messageId, displayName, imageUrl, timestamp);
        }
        public async Task SendVideo(string videoUrl, string ticketId, string displayName)
        {
            int messageId = await chatController.SaveMessageHdToSource(displayName, videoUrl, "video");
            await Clients.All.broadcastVideo(messageId, displayName, videoUrl, timestamp);
        }
        public void BroadcastDeleteMessage(int messageId)
        {
             Clients.All.broadcastDeleteMessage(messageId);
        }
      
    }
}
