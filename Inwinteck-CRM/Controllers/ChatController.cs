using Inwinteck_CRM.Helpers;
using Inwinteck_CRM.Hubs;
using Inwinteck_CRM.Models;
using Inwinteck_CRM.viewModel;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebPush;
using PushSubscription = Inwinteck_CRM.Models.PushSubscription;


namespace Inwinteck_CRM.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Source Support, Admin, Sr.Help Desk Manager, Help Desk Manager ,Quality,HR")]
    public class ChatController : Controller
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        string mailer = "Support<support@inwinteck.com>";

        // GET: Chat
        public ActionResult ChatIndex(string ticketId)
        {
            var userEmail = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                throw new Exception("User not found.");
            }
            ViewBag.TicketId = ticketId;
            ViewBag.DisplayName = user.Name;
            ViewBag.iwtTicketNo = db.Ticket.Where(x => x.Id.ToString() == ticketId).Select(x => x.Ticket_No).FirstOrDefault();
            return View();
        }


        public ActionResult GetMessagesAll()
        {
            var messages = db.ChatMessageHdToSource
                .OrderBy(m => m.Timestamp)
                .ToList()
                .Select(m => new
                {
                    m.Id,
                    m.UserName,
                    m.Message,
                    Timestamp = m.Timestamp.ToString("MM-dd-yyyy HH:mm:ss"), // Apply formatting in memory
                    m.MessageType
                })
                .ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteMessage(int messageId)
        {
            var message = db.ChatMessageHdToSource.FirstOrDefault(m => m.Id == messageId);
            if(message == null)
            {
                return Json(new { success = false, message = "Message not found" });
            }
            db.ChatMessageHdToSource.Remove(message);
            db.SaveChanges();

            return Json(new { success = true });
        }
        [HttpPost]
        public JsonResult DeleteMessageChatIndex(int messageId)
        {
            var message = db.ChatMessages.FirstOrDefault(m => m.Id == messageId);
            if (message == null)
            {
                return Json(new { success = false, message = "Message not found" });
            }
            db.ChatMessages.Remove(message);
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteMessageInwinteckInternal(int messageId)
        {
            var message = db.ChatMessageInwinteckInternal.FirstOrDefault(m => m.Id == messageId);
            if (message == null)
            {
                return Json(new { success = false, message = "Message not found" });
            }

            if ((DateTime.UtcNow - message.Timestamp).TotalMinutes > 10)
            {
                return Json(new { success = false, message = "Cannot delete messages older than 10 minutes" });
            }

            db.ChatMessageInwinteckInternal.Remove(message);
            db.SaveChanges();

            return Json(new { success = true });
        }


        public ActionResult GetMessages(string ticketId)
        {
            var messages = db.ChatMessages
                .Where(m => m.TicketId == ticketId)
                .OrderBy(m => m.Timestamp)
                .ToList() 
                .Select(m => new
                {
                    m.UserName,
                    m.Message,
                    Timestamp = m.Timestamp.ToString("MM-dd-yyyy HH:mm:ss"), // Apply formatting in memory
                     m.MessageType
                })
                .ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }





        [HttpGet]
        public ActionResult ChatHdToSource()
        {
            var userEmail = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                throw new Exception("User not found.");
            }
            ViewBag.DisplayName = user.Name;
            return View();
        }
            public async Task<int> SaveMessageHdToSource(string displayName, string message, string messageType)
            {
                var ChatMessageHdToSource = new ChatMessageHdToSource
                {
                    UserName = displayName,
                    Message = message,
                    MessageType = messageType,
                    Timestamp = DateTime.Now
                };
                db.ChatMessageHdToSource.Add(ChatMessageHdToSource);
                await db.SaveChangesAsync();
                return ChatMessageHdToSource.Id;
            }

        public async Task<int> SaveMessageInwinteckInternal(string userName, string message, string messageType, string senderUserId ,string recipientUserId = null )
        {
            var chatMessage = new ChatMessageInwinteckInternal
            {
                UserName = userName,
                Message = message,
                MessageType = messageType,
                Timestamp = DateTime.Now,
                RecipientUserId = recipientUserId ,
                SenderUserId = senderUserId
            };

            db.ChatMessageInwinteckInternal.Add(chatMessage);
            await db.SaveChangesAsync();

            return chatMessage.Id;
        }

        public async Task<int> SaveMessage(string displayName, string message, string ticketId , string messageType)
        {
            var chatMessage = new ChatMessage
            {
                UserName = displayName, 
                Message = message,
                TicketId = ticketId,
                MessageType =messageType ,
                Timestamp = DateTime.Now
            };

            db.ChatMessages.Add(chatMessage);
            await db.SaveChangesAsync();
            return chatMessage.Id;
        }

        [HttpPost]
        public JsonResult UploadImage()
        {
            if (Request.Files.Count == 0)
            {
                return Json(new { success = false, message = "No files received" });
            }

            var file = Request.Files[0];
            if (file != null && file.ContentLength > 0)
            {
                var fileName = $"{Guid.NewGuid()}.png";
                var filePath = Server.MapPath($"~/assets/uploadedChatImages/{fileName}");

                //Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                file.SaveAs(filePath);

                var imageUrl = $"/assets/uploadedChatImages/{fileName}";
                return Json(new { success = true, Url = imageUrl });
            }

            return Json(new { success = false, message = "Invalid file" });
        }

        //[HttpPost]
        //public JsonResult UploadImage()
        //{
        //    if (Request.Files.Count == 0)
        //    {
        //        return Json(new { success = false, message = "No files received" });
        //    }

        //    var file = Request.Files[0];
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        var pic = System.IO.Path.GetFileName(file.FileName);
        //        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
        //        string serverPath = Server.MapPath("~/Upload/ChatImage");
        //        string filePath = System.IO.Path.Combine(serverPath, fileName);

        //        // Ensure the directory exists
        //        if (!Directory.Exists(serverPath))
        //        {
        //            Directory.CreateDirectory(serverPath);
        //        }

        //        file.SaveAs(filePath);

        //        string url = "http://localhost:1957/Upload/Photo/";
        //        var imageUrl = url + fileName;

        //        return Json(new { success = true, Url = imageUrl });
        //    }

        //    return Json(new { success = false, message = "Invalid file" });
        //}



        [HttpPost]
        public JsonResult UploadVideo()
        {
            if (Request.Files.Count == 0)
            {
                return Json(new { success = false, message = "No files received" });
            }

            var file = Request.Files[0];
            if (file != null && file.ContentLength > 0)
            {
                var fileName = $"{Guid.NewGuid()}.mp4"; // Use appropriate file extension based on the video type
                var filePath = Server.MapPath($"~/assets/uploadedChatVideos/{fileName}");

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                file.SaveAs(filePath);

                var videoUrl = $"/assets/uploadedChatVideos/{fileName}";
                return Json(new { success = true, Url = videoUrl });
            }

            return Json(new { success = false, message = "Invalid file" });
        }


        [HttpPost]
        public JsonResult SendChatInitiationEmail(string ticketId , string emailSubject)
        {
            try
            {
                SendChatInitiationEmailInternal(ticketId, emailSubject);
                return Json(new { success = true, message = "Email sent successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        private void SendChatInitiationEmailInternal(string ticketId, string emailSubject)
        {
            string mailer = "Support<support@inwinteck.com>";

            string chatLink = Url.Action("ChatIndex", "Chat", new { ticketId = ticketId }, Request.Url.Scheme);
            string body = utlity.ChatLinkToUser(chatLink);
            string messageId = RetrieveMessageId(ticketId);
            utlity.sendmail2("Prashant.Kori@inwinteck.com,sk@inwinteck.com", emailSubject, body, mailer, messageId, true);
        }
      

        [System.Web.Mvc.Authorize(Roles = "Source Support")]
        public ActionResult SourceSupportWelcome()
        {
            return View();
        }

        //===============================To handle communication field in editTicket============================//

        //[HttpGet]
        //public ActionResult GetCommunications(string ticketId)
        //{
        //    if (string.IsNullOrEmpty(ticketId))
        //    {
        //        return Json(new { error = "Invalid Ticket ID" }, JsonRequestBehavior.AllowGet);
        //    }

        //    if (db == null)
        //    {
        //        return Json(new { error = "Database context is not initialized" }, JsonRequestBehavior.AllowGet);
        //    }

        //    int id;
        //    if (!int.TryParse(ticketId, out id))
        //    {
        //        return Json(new { error = "Invalid Ticket ID format" }, JsonRequestBehavior.AllowGet);
        //    }

        //    var communications = db.Communication
        //                           .Where(c => c.TicketId == id)
        //                           .OrderBy(c => c.CreatedAt)
        //                           .ToList();

        //    if (communications == null || !communications.Any())
        //    {
        //        return Json(new { message = "No communications found for this ticket" }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(communications, JsonRequestBehavior.AllowGet);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> UploadCommunication(HttpPostedFileBase CommunicationFile, string CommunicationText, int TicketId, string Username , string Email_Subject)
        //{
        //    var communication = new Communication
        //    {
        //        TicketId = TicketId,
        //        Username = Username,
        //        CreatedAt = DateTime.Now
        //    };
        //    var fileName = "";
        //    var filePath = "";
        //    if (CommunicationFile != null && CommunicationFile.ContentLength > 0)
        //    {
        //        // Define the path to store the uploaded files
        //         fileName = Path.GetFileName(CommunicationFile.FileName);
        //         filePath = Path.Combine(Server.MapPath("~/assets/uploadedChatVideos"), fileName);

        //        // Ensure the directory exists
        //        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        //        // Save the file to the specified path
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await CommunicationFile.InputStream.CopyToAsync(stream);
        //        }
        //        // Store the relative path
        //        communication.FilePath = $"/assets/uploadedChatVideos/{fileName}";
        //        communication.FileType = CommunicationFile.ContentType;
        //    }

        //    if (!string.IsNullOrEmpty(CommunicationText))
        //    {
        //        communication.MessageText = CommunicationText;
        //    }

        //    db.Communication.Add(communication);
        //    await db.SaveChangesAsync();

        //    SendMessageAsEmailToSource(communication.MessageText, filePath, communication.FileType , TicketId , Email_Subject);
        //    // Redirect to editTicket action in TransactionController with TicketId
        //    TempData["message"] = new List<string> { "Email Sent To Source" };
        //    return RedirectToAction("editTicket", "Transaction", new { id = TicketId });
        //}
        ////==================================Sending Response Email from Edit Ticket============================//
        //public void SendMessageAsEmailToSource(string messageText, string filePath, string fileType , int ticketId , string Email_Subject)
        //{
        //    var attachmentUrls = new List<string>();
        //    if (!string.IsNullOrEmpty(filePath))
        //    {
        //        attachmentUrls.Add(filePath);
        //    }
        //    var messageId = RetrieveMessageId(ticketId.ToString());
        //    string chatLink = Url.Action("SourceResponse", "Chat", new { ticketId = ticketId , emailSubject = Email_Subject, messageId = messageId }, Request.Url.Scheme);
        //    string body = utlity.SourceSupportResponse(messageText, attachmentUrls , chatLink);
        //    utlity.sendmailattach2("Prashant.Kori@inwinteck.com,sk@inwinteck.com", Email_Subject, body, mailer, filePath,messageId,true);
        //}


        //    [HttpGet]
        //    [AllowAnonymous]
        //    public ActionResult SourceResponse(string ticketId, string emailSubject, string messageId)
        //    {
        //        string username = "Source";

        //        var model = new SourceResponseViewModel
        //        {
        //            TicketId = ticketId,
        //            Username = username,
        //            EmailSubject =emailSubject ,
        //            MessageId = messageId
        //        };
        //        return View(model);
        //    }

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult SourceResponse(SourceResponseViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var communication = new Communication
        //        {
        //            TicketId = int.Parse(model.TicketId),
        //            Username = model.Username,
        //            MessageText = model.MessageText,
        //            CreatedAt = DateTime.Now
        //        };

        //        db.Communication.Add(communication);
        //        db.SaveChanges();

        //        // Send the email after saving the communication
        //        string body = utlity.SourceResponseAsEmail(model.MessageText);
        //        utlity.sendmail2("Prashant.Kori@inwinteck.com", model.EmailSubject, body, mailer, model.MessageId, true);

        //        // Set TempData message
        //        TempData["SuccessMessage"] = "Response Received";

        //        // Redirect or show a success message
        //        return RedirectToAction("SourceResponse", new { ticketId = model.TicketId, emailSubject = model.EmailSubject, messageId = model.MessageId });
        //    }
        //    return View(model);
        //}


        //[HttpPost]
        //public void SendResponseLink(string ticketId, string emailSubject)
        //{
        //    string mailer = "Support<support@inwinteck.com>";
        //    string messageId = RetrieveMessageId(ticketId);
        //    string chatLink = Url.Action("SourceResponse", "Chat", new { ticketId = ticketId, emailSubject = emailSubject, messageId = messageId }, Request.Url.Scheme);
        //    string body = utlity.SendResponseLink(chatLink);

        //    utlity.sendmail2("Prashant.Kori@inwinteck.com,sk@inwinteck.com", emailSubject, body, mailer, messageId, true);
        //}

        private string RetrieveMessageId(string ticketId)
        {
            var ticket = db.Ticket.SingleOrDefault(t => t.Id.ToString() == ticketId);
            return ticket?.EmailMessageId;
        }


        //-------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult AddUserToChatGroup(string ticketId, string userId)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatIndex>();
            hubContext.Clients.Group(ticketId).addUserToGroup(userId); // SignalR logic
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult RemoveUserFromChatGroup(string ticketId, string userId)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatIndex>();
            hubContext.Clients.Group(ticketId).removeUserFromGroup(userId); // SignalR logic
            return Json(new { success = true });
        }

        [HttpGet]
        public JsonResult GetAllUsers()
        {
                var users = db.Users.Select(u => new { u.Id, u.UserName }).ToList();
                return Json(users, JsonRequestBehavior.AllowGet);
        }

        //--------------------Boss Message Page---------------------//
        public ActionResult BossMessaging()
        {
            ViewBag.VapidPublicKey = System.Configuration.ConfigurationManager.AppSettings["VapidPublicKey"];
            var userEmail = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Email == userEmail);
            ViewBag.ConnectedUsers = BossMessageHub.ConnectedUsers.Values.ToList();
            ViewBag.DisplayName = user.Email;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Subscribe()
        {
            try
            {
                Log.Information("Subscribe endpoint hit.");

                // Read the request payload
                var json = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
                Log.Information("Received payload: {Json}", json);

                // Deserialize the payload into a dynamic object
                var tempSubscription = JsonConvert.DeserializeObject<dynamic>(json);

                // Map the data to PushSubscription model
                var subscription = new PushSubscription
                {
                    Endpoint = tempSubscription.endpoint,
                    P256DH = tempSubscription.keys?.p256dh,
                    Auth = tempSubscription.keys?.auth,
                    UserId = User.Identity.Name
                };

                Log.Information("Deserialized subscription: {@Subscription}", subscription);

                // Validate the subscription data
                if (subscription == null || string.IsNullOrEmpty(subscription.Endpoint))
                {
                    Log.Warning("Invalid subscription data received: {@Subscription}", subscription);
                    return new HttpStatusCodeResult(400, "Invalid subscription data.");
                }

                // Save the subscription to the database
                Log.Information("Saving subscription to database for user: {UserId}", subscription.UserId);
                await PushSubscriptionStore.AddSubscriptionAsync(subscription);

                Log.Information("Subscription successfully saved for endpoint: {Endpoint}", subscription.Endpoint);

                return new HttpStatusCodeResult(201); // Created
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while processing subscription.");
                return new HttpStatusCodeResult(500, "Internal Server Error");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Unsubscribe()
        {
            try
            {
                var json = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
                var subscription = JsonConvert.DeserializeObject<PushSubscription>(json);

                if (subscription == null || string.IsNullOrEmpty(subscription.Endpoint))
                {
                    return new HttpStatusCodeResult(400, "Invalid subscription data.");
                }

                await PushSubscriptionStore.RemoveSubscriptionAsync(subscription);

                return new HttpStatusCodeResult(200); // OK
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unsubscribe Error: {ex.Message}");
                return new HttpStatusCodeResult(500, "Internal Server Error");
            }
        }

    }
}
