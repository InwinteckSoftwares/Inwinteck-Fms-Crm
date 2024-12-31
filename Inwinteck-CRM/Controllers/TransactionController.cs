using DocumentFormat.OpenXml.Wordprocessing;
using Inwinteck_CRM.DTO;
using Inwinteck_CRM.Hubs;
using Inwinteck_CRM.Models;
using Inwinteck_CRM.viewModel;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Inwinteck_CRM.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Transaction
        string url = "https://fms.inwinteck.com/Upload/";
        string mailer = "Support<support@inwinteck.com>";

        //string url = "http://inwinteckcrm.3sptechmind.com/Upload/";

      //  [Authorize(Roles = "Admin, Sr.Help Desk Manager, Help Desk Manager ,Quality")]
        public ActionResult Ticket(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
    
            List<TicketDetails> li = new List<TicketDetails>();

            li = db.Database.SqlQuery<TicketDetails>("getTicketDetails").ToList();
            return View(li);
        }
        public ActionResult CreateTicket()
        {
            // access right ends 
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();
            ViewBag.Customer = (from s in db.EU_Master where s.Status == 1 select new SelectListItem { Text = s.Customer_Name, Value = SqlFunctions.StringConvert((double)s.Id).TrimStart() }).ToList();
            ViewBag.OEM = (from c in db.HeaderDescription where c.header_id == 4 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.System_Info = (from c in db.HeaderDescription where c.header_id == 19 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Ticket_Type = (from c in db.HeaderDescription where c.header_id == 11 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Ticket_Mode = (from c in db.HeaderDescription where c.header_id == 13 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.SLA = (from c in db.HeaderDescription where c.header_id == 14 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            //ViewBag.Ticket_Priority = (from c in db.HeaderDescription where c.header_id == 15 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Certification = (from c in db.Certification_Master where c.Status == 1 select new SelectListItem { Text = c.Certification_Name, Value = SqlFunctions.StringConvert((double)c.Id).Trim() }).ToList();
            ViewBag.Scope_Of_Work = (from c in db.HeaderDescription
                                     where c.header_id == 20 && c.Status == 1
                                     select new SelectListItem
                                     {
                                         Text = c.header_description,
                                         Value = SqlFunctions.StringConvert((double)c.id).Trim()
                                     }).ToList();


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateTicket(Ticket sa, string Comments, string TSE_Name, string[] email_office, string Email_Subject, List<Ticket_System_Info> IV, List<Ticket_EU_Detail> EUL)
        {
            var log = Log.ForContext("Action", "CreateTicket");
            try
            {
                log.Information("CreateTicket method started.");

                var existingDescription = db.HeaderDescription
                    .FirstOrDefault(x => x.header_id == 20 && x.Status == 1 && x.header_description == sa.Job_Description);

                if (existingDescription == null)
                {
                    log.Debug("No existing description found. Creating a new HeaderDescription.");
                    var newHeaderDescription = new HeaderDescription
                    {
                        header_id = 20,
                        Status = 1,
                        header_description = sa.Job_Description,
                        CreatedOn = DateTime.Now,
                        CreatedBy = User.Identity.GetUserId()
                    };

                    db.HeaderDescription.Add(newHeaderDescription);
                    log.Information("New HeaderDescription added: {HeaderDescription}", newHeaderDescription);
                }

                sa.Ticket_No = utlity.CheckTicketNo(sa.EU_ID);
                sa.Status = 18;
                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                sa.TSE_Name = TSE_Name;
                log.Debug("Ticket details initialized: {@Ticket}", sa);

                List<int> oemCertificates = db.Certification_Master
                    .Where(cm => cm.OEM == sa.OEM)
                    .Select(cm => cm.Id)
                    .ToList();

                sa.Certification_Name = sa.Certification_Need == 1 ? string.Join(", ", oemCertificates) : "";
                sa.EmailMessageId = Guid.NewGuid().ToString();
                log.Debug("Certificates processed: Certification_Name={Certification_Name}", sa.Certification_Name);

                db.Ticket.Add(sa);
                var ticketCreated = db.SaveChanges();
                log.Information("Ticket created with ID: {TicketId}", sa.Id);

                if (ticketCreated > 0)
                {
                    TicketNotificationHub.NotifyNewTicket(sa);
                    log.Information("TicketNotificationHub.NotifyNewTicket called for Ticket ID: {TicketId}", sa.Id);
                }

                string lat = sa.latitude;
                string lng = sa.longitude;

                if (sa.Ticket_Date.HasValue && sa.Dispatch_Date.HasValue)
                {
                    log.Debug("Processing timezone information for Ticket ID: {TicketId}", sa.Id);
                    var caseDt = await Universal.CommonMethod.ConvertIstToLocalAndUtcAsync(lat, lng, sa.Ticket_Date.Value);
                    var dispDt = await Universal.CommonMethod.ConvertLocalToIstAndUtcAsync(lat, lng, sa.Dispatch_Date.Value);

                    var timeZoneRecord = new Models.TimeZone
                    {
                        Ticket_Id = sa.Id,
                        CaseDtIndia = sa.Ticket_Date.Value,
                        CaseDtUs = caseDt.istToUs,
                        CaseDtLocal = caseDt.istToLocal,
                        DispDtLocal = sa.Dispatch_Date,
                        DispDtIndia = dispDt.localToIst,
                        DispDtUs = dispDt.localToUs
                    };

                    db.TimeZone.Add(timeZoneRecord);
                    db.SaveChanges();
                    sa.TimeZoneId = timeZoneRecord.Id;

                    log.Information("TimeZone record created for Ticket ID: {TicketId}, Record: {@TimeZoneRecord}", sa.Id, timeZoneRecord);

                    var ticketSLA = Universal.CommonMethod.CalculateSLA(timeZoneRecord.DispDtLocal, timeZoneRecord.CaseDtLocal);
                    sa.SLA = ticketSLA;
                    db.Entry(sa).State = EntityState.Modified;
                    log.Information("SLA calculated for Ticket ID: {TicketId}, SLA: {SLA}", sa.Id, ticketSLA);
                }

                string eu = db.EU_Master.Where(s => s.Id == sa.EU_ID).Select(s => s.Customer_Name).FirstOrDefault();
                string body = utlity.TicketGenerated(eu, sa.Ticket_No, sa.Case_No, sa.Site_Name, sa.Street_Address, sa.Dispatch_Date.ToString(), sa.Job_Description);
                log.Debug("Email body generated for Ticket ID: {TicketId}", sa.Id);

                if (sa.Case_No == "000000")
                {
                    utlity.sendmail2("prashant.kori@inwinteck.com", Email_Subject, body, mailer, sa.EmailMessageId, false);
                    log.Information("Email sent to prashant.kori@inwinteck.com for Ticket ID: {TicketId}", sa.Id);
                }
                else
                {
                    utlity.sendmail2("hd@inwinteck.com", Email_Subject, body, mailer, sa.EmailMessageId, false);
                    log.Information("Email sent to hd@inwinteck.com for Ticket ID: {TicketId}", sa.Id);
                }

                Ticket_History th = new Ticket_History
                {
                    Ticket_no = sa.Id,
                    Comments = Comments,
                    FE_ID = sa.FE_ID,
                    status = 18,
                    SLA = db.HeaderDescription.FirstOrDefault(hd => hd.id == db.Ticket.Where(t => t.Id == sa.Id).Select(t => t.SLA).FirstOrDefault())?.header_description,
                    CreatedBy = User.Identity.GetUserId(),
                    CreatedOn = DateTime.Now
                };

                db.Ticket_History.Add(th);
                log.Debug("Ticket history added for Ticket ID: {TicketId}", sa.Id);

                Ticket_Email TE = new Ticket_Email
                {
                    Ticket_no = sa.Id,
                    Email_Subject = !string.IsNullOrEmpty(Email_Subject) ? Email_Subject : "Ticket Generated",
                    CreatedBy = User.Identity.GetUserId(),
                    CreatedOn = DateTime.Now
                };

                db.Ticket_Email.Add(TE);
                log.Debug("Ticket email record added for Ticket ID: {TicketId}", sa.Id);

                if (IV != null && IV.Any(iv => iv.System_Information != null))
                {
                    Log.Debug("Processing system information for Ticket ID: {TicketId}", sa.Id);
                    foreach (var iv in IV)
                    {
                        Ticket_System_Info TSI = new Ticket_System_Info
                        {
                            Ticket_no = sa.Id,
                            System_Information = iv.System_Information,
                            Make_Model = iv.Make_Model,
                            Serial_Number = iv.Serial_Number,
                            Required_Tool = iv.Required_Tool,
                            CreatedBy = User.Identity.GetUserId(),
                            CreatedOn = DateTime.Now
                        };
                        db.Ticket_System_Info.Add(TSI);
                    }
                }

                if (EUL != null && EUL.Any(eul => eul.EU_Name != null))
                {
                    log.Debug("Processing EU details for Ticket ID: {TicketId}", sa.Id);
                    foreach (var eul in EUL)
                    {
                        Ticket_EU_Detail TSEU = new Ticket_EU_Detail
                        {
                            Ticket_no = sa.Id,
                            EU_Name = eul.EU_Name,
                            EU_Email = eul.EU_Email,
                            EU_Contact = eul.EU_Contact,
                            CreatedBy = User.Identity.GetUserId(),
                            CreatedOn = DateTime.Now
                        };

                        db.Ticket_EU_Detail.Add(TSEU);
                        log.Debug("EU detail added: {@EUDetail}");
                    }
                }

                int res = db.SaveChanges();
                log.Information("Final save completed for Ticket ID: {TicketId}, Result: {Result}", sa.Id, res);

                if (res > 0)
                {
                    TempData["message"] = new List<string> { "Ticket Created : <br>  Number :" + sa.Ticket_No };
                }
                else
                {
                    TempData["message"] = "Ticket Not Created !! ";
                    log.Warning("Ticket creation failed at final save stage for Ticket ID: {TicketId}", sa.Id);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "An error occurred while creating the ticket.");
                TempData["message"] = "Ticket Not Created !! ";
            }

            log.Information("CreateTicket method ended.");
            return RedirectToAction("editTicket", "Transaction", new { id = sa.Id });
        }

        public static async Task<TimeZoneInfo> GetTimeZoneAsync(string lat, string lng)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiKey = "AIzaSyBvhQbrULBXqk9wPb9RjTeacVk3A3g-ci8"; // Your API key
                string url = $"https://maps.googleapis.com/maps/api/timezone/json?location={lat},{lng}&timestamp={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}&key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var timeZoneInfo = JsonConvert.DeserializeObject<TimeZoneApiResponse>(responseBody);

                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfo.timeZoneId);
                }
                catch (TimeZoneNotFoundException)
                {
                    var baseUtcOffset = TimeSpan.FromSeconds(timeZoneInfo.rawOffset + timeZoneInfo.dstOffset);
                    return TimeZoneInfo.CreateCustomTimeZone(timeZoneInfo.timeZoneId, baseUtcOffset, timeZoneInfo.timeZoneName, timeZoneInfo.timeZoneName);
                }
            }
        }


        private void StoreMessageId(string ticketId, string messageId)
        {
            var ticket = db.Ticket.SingleOrDefault(t => t.Id.ToString() == ticketId);
            if (ticket != null)
            {
                ticket.EmailMessageId = messageId;
                db.SaveChanges();
            }
        }


        [Authorize(Roles = "Admin, Sr.Help Desk Manager, Help Desk Manager ,Quality")]

        public ActionResult editTicket(int id)
        {
            var log = Log.ForContext("Action", "EditTicket");

            log.Information("====================== editTicket method invoked for Ticket ID: {TicketId} ===========================", id);
            try
            {
                var ti = db.Ticket
                   .Include(t => t.TimeZone).Include(c => c.TicketFeCharges)
                   .FirstOrDefault(t => t.Id == id);

                if (ti == null)
                {
                    log.Warning("Ticket with ID {TicketId} not found.", id);
                    return HttpNotFound();
                }
                var userName = User.Identity.Name;

                var formattedCaseDtUs = ti.TimeZone?.CaseDtUs?.ToString("yyyy-MM-ddTHH:mm");
                var formattedCaseDtLocal = ti.TimeZone?.CaseDtLocal?.ToString("yyyy-MM-ddTHH:mm");

                var formattedDispDtUs = ti.TimeZone?.DispDtUs?.ToString("yyyy-MM-ddTHH:mm");
                var formattedDispDtIndia = ti.TimeZone?.DispDtIndia?.ToString("yyyy-MM-ddTHH:mm");

                log.Information("Formatted values: CaseDtUs: {CaseDtUs}, CaseDtLocal: {CaseDtLocal}, DispDtUs: {DispDtUs}, DispDtIndia: {DispDtIndia}",
                                formattedCaseDtUs,
                                formattedCaseDtLocal,
                                formattedDispDtUs,
                                formattedDispDtIndia);

                var formattedInTimeUs = ti.TimeZone?.InTimeUs?.ToString("yyyy-MM-ddTHH:mm");
                var formattedInTimeLocal = ti.TimeZone?.InTimeLocal?.ToString("yyyy-MM-ddTHH:mm");
                var formattedOutTimeUs = ti.TimeZone?.OutTimeUs?.ToString("yyyy-MM-ddTHH:mm");
                var formattedOutTimeLocal = ti.TimeZone?.OutTimeLocal?.ToString("yyyy-MM-ddTHH:mm");

                log.Information(
                                "Formatted values: InTimeUs: {InTimeUs}, InTimeLocal: {InTimeLocal}, OutTimeUs: {OutTimeUs}, OutTimeLocal: {OutTimeLocal}",
                                formattedInTimeUs,
                                formattedInTimeLocal,
                                formattedOutTimeUs,
                                formattedOutTimeLocal
                            );


                var formattedInTimeUs2 = ti.TimeZone?.InTimeUs2?.ToString("yyyy-MM-ddTHH:mm");
                var formattedInTimeLocal2 = ti.TimeZone?.InTimeLocal2?.ToString("yyyy-MM-ddTHH:mm");
                var formattedOutTimeUs2 = ti.TimeZone?.OutTimeUs2?.ToString("yyyy-MM-ddTHH:mm");
                var formattedOutTimeLocal2 = ti.TimeZone?.OutTimeLocal2?.ToString("yyyy-MM-ddTHH:mm");

                log.Information(
                            "Formatted values: InTimeUs2: {InTimeUs2}, InTimeLocal2: {InTimeLocal2}, OutTimeUs2: {OutTimeUs2}, OutTimeLocal2: {OutTimeLocal2}",
                            formattedInTimeUs2,
                            formattedInTimeLocal2,
                            formattedOutTimeUs2,
                            formattedOutTimeLocal2
                        );



                // Ensure ti is not null before proceeding
                if (ti == null)
                {
                    // Handle the null case appropriately (e.g., redirect, show error message)
                    return RedirectToAction("ErrorPage"); // Example redirect
                }

                // Populate formatted values in ViewBag
                ViewBag.CaseDtUs = formattedCaseDtUs;
                ViewBag.CaseDtLocal = formattedCaseDtLocal;
                ViewBag.formattedDispDtUs = formattedDispDtUs;
                ViewBag.formattedDispDtIndia = formattedDispDtIndia;
                ViewBag.inTimeUs = formattedInTimeUs;
                ViewBag.inTimeLocal = formattedInTimeLocal;
                ViewBag.OutTimeUs = formattedOutTimeUs;
                ViewBag.OutTimeLocal = formattedOutTimeLocal;

                ViewBag.inTimeUs2 = formattedInTimeUs2;
                ViewBag.inTimeLocal2 = formattedInTimeLocal2;
                ViewBag.OutTimeUs2 = formattedOutTimeUs2;
                ViewBag.OutTimeLocal2 = formattedOutTimeLocal2;
                ViewBag.UserName = userName;

                // Populate view bags with necessary data and null checks
                ViewBag.Country = db.Country
                                    .Where(c => c.Status == 1)
                                    .Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() })
                                    .ToList();

                ViewBag.Customer = db.EU_Master
                                     .Where(s => s.Id == ti.EU_ID)
                                     .Select(s => s.Customer_Name)
                                     .FirstOrDefault() ?? string.Empty;

                ViewBag.OEM = db.HeaderDescription
                                .Where(c => c.header_id == 4 && c.Status == 1)
                                .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                                .ToList() ?? new List<SelectListItem>();

                ViewBag.Status = db.HeaderDescription
                                  .Where(c => c.header_id == 5 && c.Status == 1)
                                  .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                                  .ToList() ?? new List<SelectListItem>();

                ViewBag.Part_Management = db.HeaderDescription
                                          .Where(c => c.header_id == 6 && c.Status == 1)
                                          .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                                          .ToList() ?? new List<SelectListItem>();

                ViewBag.Part_Handover = db.HeaderDescription
                                        .Where(c => c.header_id == 7 && c.Status == 1)
                                        .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                                        .ToList() ?? new List<SelectListItem>();

                ViewBag.Ticket_Type = db.HeaderDescription
                                      .Where(c => c.header_id == 11 && c.Status == 1)
                                      .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                                      .ToList() ?? new List<SelectListItem>();

                ViewBag.Ticket_Mode = db.HeaderDescription
                                      .Where(c => c.header_id == 13 && c.Status == 1)
                                      .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                                      .ToList() ?? new List<SelectListItem>();

                ViewBag.SLA = db.HeaderDescription
                              .Where(c => c.header_id == 14 && c.Status == 1)
                              .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                              .ToList() ?? new List<SelectListItem>();

                ViewBag.Ticket_Priority = db.HeaderDescription
                                         .Where(c => c.header_id == 15 && c.Status == 1)
                                         .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                                         .ToList() ?? new List<SelectListItem>();

                ViewBag.Certification = db.Certification_Master
                             .Where(c => c.Status == 1)
                             .Select(c => new SelectListItem { Text = c.Certification_Name, Value = c.Id.ToString() })
                             .ToList() ?? new List<SelectListItem>();


                // Optional fields with null checks
                ViewBag.FE = db.FE_Master_Personal
                               .Where(s => s.Id == ti.FE_ID)
                               .Select(s => s.First_Name)
                               .FirstOrDefault() ?? string.Empty;
                log.Information("Before Declined Reason ViewBag");

                ViewBag.Decline_Reason = db.HeaderDescription
                                         .Where(c => c.header_id == 16 && c.Status == 1)
                                         .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                                         .ToList() ?? new List<SelectListItem>();

                ViewBag.Cancel_Reason = db.HeaderDescription
                                        .Where(c => c.header_id == 17 && c.Status == 1)
                                        .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                                        .ToList() ?? new List<SelectListItem>();

                ViewBag.Ticket = "INWIN000" + ti.Id;
                log.Information("IWT Ticket No: {TicketNo}", ViewBag.Ticket);

                ViewBag.EU_Office = db.EU_Master_Branch
                                     .Where(s => s.Id == ti.EU_Office)
                                     .Select(s => s.Office)
                                     .FirstOrDefault() ?? string.Empty;

                ViewBag.Email_Subject = db.Ticket_Email
                                         .Where(s => s.Ticket_no == ti.Id)
                                         .Select(s => s.Email_Subject)
                                         .FirstOrDefault() ?? string.Empty;

                ViewBag.email_office = db.Ticket_Email
                                        .Where(s => s.Ticket_no == ti.Id)
                                        .Select(s => s.Email)
                                        .FirstOrDefault() ?? string.Empty;

                ViewBag.Old_Ticket = db.Ticket
                                      .Where(s => s.Id == ti.Old_Ticket)
                                      .Select(s => s.Ticket_No)
                                      .FirstOrDefault() ?? string.Empty;

                ViewBag.System_info = db.Ticket_System_Info
                                       .Where(s => s.Ticket_no == ti.Id)
                                       .ToList() ?? new List<Ticket_System_Info>();

                ViewBag.Scope_Of_Work = db.HeaderDescription
                                        .Where(c => c.header_id == 20 && c.Status == 1)
                                        .Select(c => new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() })
                                        .ToList() ?? new List<SelectListItem>();

                ViewBag.Currency = db.Currency_Master.ToList();
                log.Information("End");

                return View(ti);
            }
            catch (Exception ex)
            {
                log.Error(ex, "An unexpected error occurred in editTicket method for Ticket ID: {TicketId}", id);
                return RedirectToAction("ErrorPage");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> editTicket(Ticket sa, HttpPostedFileBase pht, Ticket_History ticketHistory, List<Part_Ticket_Data> IV, string[] Certification_Name, Ticket_FE_Charges ticketCharges, Models.TimeZone timezone)
        {
            try
            {
                List<string> messages = new List<string>();
                var email_office = string.Empty;
                if (sa.Case_No == "000000")
                {
                    email_office = "prasahant.kori@inwinteck.com";
                }
                else
                {
                    email_office = "hd@inwinteck.com";
                }
                var emailSubject = "";
                var emailData = db.Ticket_Email.FirstOrDefault(t => t.Ticket_no == sa.Id);

                if (emailData != null)
                {
                    emailSubject = emailData.Email_Subject;
                }

                //UpdateTicketEmail(sa, email);

                var fe = db.FE_Master_Personal.FirstOrDefault(s => s.Id == sa.FE_ID);
                var fe2 = db.FE_Master_Personal.FirstOrDefault(s => s.Id == sa.FE_ID_2);

                if (!ValidateTicket(sa, messages))
                {
                    return RedirectToAction("editTicket", "Transaction", new { id = sa.Id });
                }

                await HandleStatusChangesAsync(sa, fe, fe2, email_office, ticketHistory, messages, emailSubject, ticketCharges, timezone);

                if (pht != null)
                {
                    HandleFileUpload(sa, pht);
                }

                sa.Certification_Name = Certification_Name != null ? string.Join(", ", Certification_Name) : "";

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;


                try
                {
                    var trackedEntity = db.ChangeTracker.Entries<Ticket>().FirstOrDefault(e => e.Entity.Id == sa.Id);

                    if (trackedEntity != null)
                    {
                        trackedEntity.CurrentValues.SetValues(sa);
                    }
                    else
                    {
                        db.Ticket.Attach(sa);
                        db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    }

                    int res = db.SaveChanges();
                    if (res > 0)
                    {
                        AddTicketHistory(sa, ticketHistory, fe, email_office);
                        //AddTicketCharges(sa);

                        // Handle Part_Ticket_Data if provided
                        if (IV != null && IV.Any(iv => iv.Serial_No != null))
                        {
                            foreach (var iv in IV)
                            {
                                AddPartTicketData(sa, iv);
                            }
                        }

                        messages.Add("Ticket Updated !!");
                        TempData["message"] = messages;

                        if (sa.Status == 19 && sa.Is_Reschedule == 1)
                        {
                            await HandleTicketReschedule(sa, fe, email_office, ticketHistory, messages);
                            return RedirectToAction("viewTicket", "Transaction", new { id = sa.Id });
                        }
                        if (sa.Status == 1641)
                        {
                            return await RescheduleAfterClose(sa, fe, email_office, ticketHistory, messages, timezone);
                        }
                        if (sa.Status == 1362) //Ticket Declined
                        {
                            messages.Add("Ticket Updated");
                            return RedirectToAction("viewTicket", "Transaction", new { id = sa.Id });
                        }
                        if (sa.Status == 21) // Ticket cancelled
                        {
                            messages.Add("Ticket Updated");
                            return RedirectToAction("viewTicket", "Transaction", new { id = sa.Id });
                        }
                        if (sa.Status == 20)
                        {
                            return await HandleTicketClosure(sa, fe, fe2, email_office, emailSubject, messages, timezone);
                        }
                    }
                }
                catch (Exception ex)
                {
                    utlity.createlog($"Exception while saving Ticket No {sa.Id}: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = new List<string> { "Ticket Not Updated !!" };
                utlity.createlog($"/Ticket/Error Ticket No : {sa.Id} Error: {ex.Message}");
            }

            return RedirectToAction("editTicket", "Transaction", new { id = sa.Id });
        }
        private void UpdateTicketEmail(Ticket sa, string email)
        {
            var TEE = db.Ticket_Email.FirstOrDefault(s => s.Ticket_no == sa.Id);
            if (TEE != null && email != TEE.Email)
            {
                TEE.Email = email;
                db.Entry(TEE).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        private bool ValidateTicket(Ticket sa, List<string> messages)
        {
            if (sa.Status == 20)
            {
                if (sa.In_Time == null)
                {
                    messages.Add("Do enter In Time before Closing the ticket !!");
                    return false;
                }
                else if (sa.Out_Time == null)
                {
                    messages.Add("Do enter Out Time before Closing the ticket !!");
                    return false;
                }
                else if (sa.In_Time > sa.Out_Time)
                {
                    messages.Add("Do Check Out Time is past In Time!!");
                    return false;
                }
            }
            else if (sa.Status == 1362)
            {
                sa.Is_Decline = 1;
            }
            else if (sa.Status == 19)
            {
                if (sa.In_Time == null)
                {
                    sa.Is_Reschedule = 1;
                }
                else
                {
                    messages.Add("Ticket is already checked in! Can't be Rescheduled !!");
                    return false;
                }
            }
            return true;
        }

        private async Task HandleStatusChangesAsync(Ticket sa, FE_Master_Personal fe, FE_Master_Personal fe2, string email_office, Ticket_History ticketHistory, List<string> messages, string emailSubject, Ticket_FE_Charges ticketCharges, Models.TimeZone timezone)
        {
            switch (sa.Status)
            {
                case 20:  // Ticket Closed
                    if (!ValidateTicketClosure(sa, messages))
                    {
                        return;
                    }
                    break;

                case 1362:  // Ticket Declined
                    sa.Is_Decline = 1;
                    messages.Add("Ticket Declined");
                    break;

                case 19:  // Ticket Rescheduled
                    if (sa.In_Time == null)
                    {
                        sa.Is_Reschedule = 1;
                    }
                    else
                    {
                        messages.Add("Ticket is already checked in! Can't be Rescheduled !!");
                        return;
                    }
                    break;

                case 1414:  // Engineer Details Sent to Customer i.e FE Selected
                    SendEngineerDetailsToCustomer(sa, fe, fe2, email_office, messages, ticketHistory, emailSubject, ticketCharges);
                    break;

                case 1415:  // Engineer Checked In
                    await SendEngineerCheckedInEmail(sa, fe, fe2, email_office, messages, emailSubject, timezone);
                    break;

                case 1372:  // Ticket Scheduled
                    SendJobConfirmation(sa, fe, messages, emailSubject);
                    break;

                case 2692:  // Send Travel Charge
                    SendTravelCharges(sa, ticketCharges, emailSubject ,messages);
                    break;

                default:
                    break;
            }
        }

        private bool ValidateTicketClosure(Ticket sa, List<string> messages)
        {
            if (sa.In_Time == null)
            {
                messages.Add("Do enter In Time before Closing the ticket !!");
                return false;
            }
            if (sa.Out_Time == null)
            {
                messages.Add("Do enter Out Time before Closing the ticket !!");
                return false;
            }
            if (sa.In_Time > sa.Out_Time)
            {
                messages.Add("Do Check Out Time is past In Time!!");
                return false;
            }
            return true;
        }

        private void SendEngineerDetailsToCustomer(Ticket sa, FE_Master_Personal fe, FE_Master_Personal fe2, string email_office, List<string> messages, Ticket_History ticketHistory, string emailSubject, Ticket_FE_Charges ticketCharges)
        {
            try
            {
                // Retrieve customer name
                string eu = db.EU_Master
                    .Where(s => s.Id == sa.EU_ID)
                    .Select(s => s.Customer_Name)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(eu))
                {
                    throw new Exception("Customer name not found for the given EU_ID.");
                }
                var TF = new Ticket_Eu_Selection
                {
                    Eu_ID = sa.EU_ID,
                    Fe_Id = sa.FE_ID,
                    Fe_Id_2 = sa.FE_ID_2,
                    Ticket_no = sa.Id,
                    Status = "Sent",
                    CreatedBy = User.Identity.GetUserId(),
                    CreatedOn = DateTime.Now
                };


                var existingTicketCharge = db.TicketFeCharges
                   .FirstOrDefault(tc => tc.Ticket_Id == ticketCharges.Ticket_Id);

                if (existingTicketCharge != null)
                {
                    existingTicketCharge.Travel_Charge_1 = ticketCharges.Travel_Charge_1;
                    existingTicketCharge.Travel_Amount_1 = ticketCharges.Travel_Amount_1;
                    existingTicketCharge.Charge_Type_1 = ticketCharges.Charge_Type_1;
                    existingTicketCharge.Per_Hour_1 = ticketCharges.Per_Hour_1;
                    existingTicketCharge.Per_Job_1 = ticketCharges.Per_Job_1;
                    existingTicketCharge.Currency = ticketCharges.Currency;
                    existingTicketCharge.Travel_Charge_2 = ticketCharges.Travel_Charge_2;
                    existingTicketCharge.Travel_Amount_2 = ticketCharges.Travel_Amount_2;
                    existingTicketCharge.Charge_Type_2 = ticketCharges.Charge_Type_2;
                    existingTicketCharge.Per_Hour_2 = ticketCharges.Per_Hour_2;
                    existingTicketCharge.Per_Job_2 = ticketCharges.Per_Job_2;
                    db.Entry(existingTicketCharge).State = EntityState.Modified;
                }
                else
                {
                    db.TicketFeCharges.Add(ticketCharges);
                }
                db.Ticket_Eu_Selection.Add(TF);
                db.SaveChanges();
                var callbackUrl = Url.Action("TicketEUSelection", "Transaction", new { Id = TF.Id }, protocol: Request.Url.Scheme);

                string body = string.Empty;

                if (!sa.feChanged)   //fe not Changed
                {
                    body = sa.FE_ID_2 != null
                        ? utlity.CustomerSchedulewith2ndFE(
                              eu,
                              sa.Case_No,
                              $"{fe.First_Name} {fe.Last_Name}",
                              fe.Email,
                              $"{fe.Phone_Number_Code}-{fe.Phone_Number}",
                              $"{fe2.First_Name} {fe2.Last_Name}",
                              fe2.Email,
                              $"{fe2.Phone_Number_Code}-{fe2.Phone_Number}",
                              callbackUrl
                          )
                        : utlity.CustomerSchedule(
                              eu,
                              sa.Case_No,
                              $"{fe.First_Name} {fe.Last_Name}",
                              fe.Email,
                              $"{fe.Phone_Number_Code}-{fe.Phone_Number}",
                              callbackUrl
                          );
                }
                else   //FE Changed
                {
                    body = sa.FE_ID_2 == null
                        ? utlity.FEChanged(
                              sa.Case_No,
                              $"{fe.First_Name} {fe.Last_Name}",
                              fe.Email,
                              $"{fe.Phone_Number_Code}-{fe.Phone_Number}",
                              callbackUrl
                          )
                        : utlity.FEChanged2ndFE(
                              sa.Case_No,
                              $"{fe.First_Name} {fe.Last_Name}",
                              fe.Email,
                              $"{fe.Phone_Number_Code}-{fe.Phone_Number}",
                              $"{fe2.First_Name} {fe2.Last_Name}",
                              fe2.Email,
                              $"{fe2.Phone_Number_Code}-{fe2.Phone_Number}",
                              callbackUrl

                          );
                }

                // Retrieve the message ID
                string messageId = RetrieveMessageId(sa.Id.ToString());
                string recipientEmail = string.Empty;

                if (sa.Case_No == "000000")
                {
                    recipientEmail = "prashant.kori@inwinteck.com";
                }
                else
                {
                    if (sa.CreatedBy == "etc@sourcesupport.com")
                    {
                        recipientEmail = "etc@sourcesupport.com,hd@inwinteck.com,sk@inwinteck.com"; 
                    }
                    else
                    {
                        recipientEmail = "hd@inwinteck.com";
                    }
                }
                // Send the email
                utlity.sendmail2(recipientEmail, emailSubject, body, mailer, messageId, true);

                messages.Add("Engineer details sent to customer");
            }
            catch (Exception ex)
            {
                messages.Add($"Error sending engineer details to customer: {ex.Message}");
            }
        }

        public void SendTravelCharges(Ticket sa, Ticket_FE_Charges ticketCharges , string emailSubject, List<string> message)
        {
            var existingTicketCharge = db.TicketFeCharges
                            .FirstOrDefault(tc => tc.Ticket_Id == ticketCharges.Ticket_Id);

            if (existingTicketCharge != null)
            {
                existingTicketCharge.SentTravelCharge = ticketCharges.SentTravelCharge;

                if (ticketCharges.SentTravelCharge2.HasValue)
                {
                    existingTicketCharge.SentTravelCharge2 = ticketCharges.SentTravelCharge2.Value;
                }
            }

            int saveCharges = db.SaveChanges();
            string caseNo = sa.Case_No;
            if (saveCharges != 0)
            {
                string body = string.Empty;
                if(sa.IsSecondFEEnabled != true)
                {
                   body = utlity.SendTravelCharge(ticketCharges , caseNo);
                }
                else
                {
                    body = utlity.SendTravelCharge2(ticketCharges , caseNo);
                }
                string messageId = RetrieveMessageId(sa.Id.ToString());
                string recipientEmail = string.Empty;
                if(sa.Case_No == "000000")
                {
                    recipientEmail = "prashant.kori@inwinteck.com";
                }
                else
                {
                    if (sa.CreatedBy == "etc@sourcesupport.com")
                    {
                        recipientEmail = "etc@sourcesupport.com,hd@inwinteck.com,sk@inwinteck.com"; 
                    }
                    else
                    {
                        recipientEmail = "hd@inwinteck.com";
                    }
                }
                
                utlity.sendmail2(recipientEmail, emailSubject, body, mailer, messageId, true);
                message.Add("Travel Details Sent to Customer");
            }
            else
            {
                message.Add("Unable To Send TravelCharges");
            }
        }

        public string RetrieveMessageId(string ticketId)
        {
            var ticket = db.Ticket.SingleOrDefault(t => t.Id.ToString() == ticketId);
            return ticket?.EmailMessageId;
        }
        private async Task SendEngineerCheckedInEmail(Ticket sa, FE_Master_Personal fe, FE_Master_Personal fe2, string email_office, List<string> messages, string emailSubject, Models.TimeZone timezone)
        {   
            if (sa.In_Time.HasValue)
            {
                try
                {
                    var timeZoneRecord = await db.TimeZone.FirstOrDefaultAsync(tz => tz.Ticket_Id == sa.Id);

                    if (timeZoneRecord != null)
                    {
                        timeZoneRecord.InTimeLocal = timezone.InTimeLocal;
                        timeZoneRecord.InTimeUs = timezone.InTimeUs;
                        timeZoneRecord.InTimeIndia = sa.In_Time;

                        if (sa.In_Time2.HasValue)
                        {
                            timeZoneRecord.InTimeLocal2 = timezone.InTimeLocal2;
                            timeZoneRecord.InTimeUs2 = timezone.InTimeUs2;
                            timeZoneRecord.InTimeIndia2 = sa.In_Time2;
                        }

                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        timeZoneRecord = new Models.TimeZone
                        {
                            Ticket_Id = sa.Id,
                            InTimeLocal = timezone.InTimeLocal,
                            InTimeUs = timezone.InTimeUs,
                            InTimeIndia = sa.In_Time,
                            InTimeLocal2 = sa.In_Time2.HasValue ? timezone.InTimeLocal2 : (DateTime?)null,
                            InTimeUs2 = sa.In_Time2.HasValue ? timezone.InTimeUs2 : (DateTime?)null,
                            InTimeIndia2 = sa.In_Time2
                        };

                        db.TimeZone.Add(timeZoneRecord);
                    }

                    await db.SaveChangesAsync();


                    string eu = db.EU_Master.Where(s => s.Id == sa.EU_ID).Select(s => s.Customer_Name).FirstOrDefault();
                    string body;

                    if (sa.FE_ID_2 == null)
                    {
                        body = utlity.FECheckedIn(
                            eu,
                            sa.Case_No,
                            $"{fe.First_Name} {fe.Last_Name}",
                            fe.Email,
                            $"{fe.Phone_Number_Code}-{fe.Phone_Number}",
                            timeZoneRecord.InTimeLocal.HasValue ? timeZoneRecord.InTimeLocal.Value.ToString("dd/MM/yyyy") : "N/A",
                            timeZoneRecord.InTimeLocal.HasValue ? timeZoneRecord.InTimeLocal.Value.ToString("HH:mm:ss") : "N/A"
                        );
                    }
                    else
                    {
                        body = utlity.FECheckedIn2ndFE(
                            eu,
                            sa.Case_No,
                            $"{fe.First_Name} {fe.Last_Name}",
                            fe.Email,
                            $"{fe.Phone_Number_Code}-{fe.Phone_Number}",
                            $"{fe2.First_Name} {fe2.Last_Name}",
                            fe2.Email,
                            $"{fe2.Phone_Number_Code}-{fe2.Phone_Number}",
                            timeZoneRecord.InTimeLocal.HasValue ? timeZoneRecord.InTimeLocal.Value.ToString("dd/MM/yyyy") : "N/A",
                            timeZoneRecord.InTimeLocal.HasValue ? timeZoneRecord.InTimeLocal.Value.ToString("HH:mm:ss") : "N/A",
                            timeZoneRecord.InTimeLocal2.HasValue ? timeZoneRecord.InTimeLocal2.Value.ToString("dd/MM/yyyy") : "N/A",
                            timeZoneRecord.InTimeLocal2.HasValue ? timeZoneRecord.InTimeLocal2.Value.ToString("HH:mm:ss") : "N/A"
                        );
                    }

                    string messageId = RetrieveMessageId(sa.Id.ToString());

                    string combinedEmails = string.Empty;

                    if (sa.Case_No != "000000")
                    {
                        if (sa.CreatedBy == "etc@sourcesupport.com")
                        {
                            combinedEmails = "etc@sourcesupport.com,hd@inwinteck.com,sk@inwinteck.com";
                        }
                        else
                        {
                            combinedEmails = "hd@inwinteck.com";
                        }
                        utlity.sendmail2(combinedEmails, emailSubject, body, mailer, messageId, true);
                    }

                    messages.Add("Checked In Email Sent to Source");

                }
                catch (DbUpdateException ex)
                {
                    // Log the exception or handle it as needed
                    // Example: _logger.LogError(ex, "An error occurred while updating time zone record.");
                }
            }
        }

        private void SendJobConfirmation(Ticket sa, FE_Master_Personal fe, List<string> messages, string emailSubject)
        {
            messages.Add("Ticket has been Scheduled. FE has been informed by email");
            string sysInfo = GenerateSystemInfoTable(sa.Id);

            string oem = db.HeaderDescription.Where(s => s.id == sa.OEM).Select(s => s.header_description).FirstOrDefault();
            string body = utlity.JobConfirm($"{fe.First_Name} {fe.Last_Name}", sa.Site_Name, sa.EU_Name, sa.Street_Address, sa.Dispatch_Date.Value.ToString("dd MMM yyy HH':'mm tt"), sa.Job_Description, sa.pregame_detail, sysInfo, oem);

            if (sa.pregame_upload != null)
            {
                utlity.sendmailattach(fe.Email, $"Job Confirmation and additional details for Inwinteck Ticket no : {sa.Ticket_No}", body, mailer, sa.pregame_upload);
            }
            else
            {
                string messageId = RetrieveMessageId(sa.Id.ToString());
                utlity.sendmail2(fe.Email, emailSubject, body, mailer, messageId, true);
            }
        }

        private async Task<ActionResult> RescheduleAfterClose(Ticket sa, FE_Master_Personal fe, string email_office, Ticket_History ticketHistory, List<string> messages, Models.TimeZone timezoneModel)
        {
            await HandleTicketReschedule(sa, fe, email_office, ticketHistory, messages);
            var timeZoneRecord = await db.TimeZone.FirstOrDefaultAsync(tz => tz.Ticket_Id == sa.Id);

            if (timeZoneRecord != null && sa.Out_Time.HasValue)
            {
                timeZoneRecord.OutTimeIndia = sa.Out_Time;
                timeZoneRecord.OutTimeLocal = timezoneModel.OutTimeLocal;
                timeZoneRecord.OutTimeUs = timezoneModel.OutTimeUs;

                if (timezoneModel.OutTimeLocal2.HasValue && timezoneModel.OutTimeUs2.HasValue)
                {
                    timeZoneRecord.OutTimeIndia2 = sa.Out_Time2;
                    timeZoneRecord.OutTimeLocal2 = timezoneModel.OutTimeLocal2;
                    timeZoneRecord.OutTimeUs2 = timezoneModel.OutTimeUs2;
                }
                if (timeZoneRecord.InTimeLocal.HasValue && timeZoneRecord.OutTimeLocal.HasValue)
                {
                    TimeSpan timeDifference = timeZoneRecord.OutTimeLocal.Value - timeZoneRecord.InTimeLocal.Value;
                    sa.Total_Hours = $"{(int)timeDifference.TotalHours} hours {timeDifference.Minutes} minutes";
                }

                if (sa.In_Time2.HasValue && sa.Out_Time2.HasValue && timeZoneRecord.InTimeLocal2.HasValue && timeZoneRecord.OutTimeLocal2.HasValue)
                {
                    TimeSpan timeDifference2 = timeZoneRecord.OutTimeLocal2.Value - timeZoneRecord.InTimeLocal2.Value;
                    sa.Total_Hours2 = $"{(int)timeDifference2.TotalHours} hours {timeDifference2.Minutes} minutes";
                }

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    TempData["error"] = "An error occurred while saving changes. Please try again.";
                    return RedirectToAction("viewTicket", "Transaction", new { id = sa.Id });
                }
            }

            TempData["message"] = messages;
            return RedirectToAction("viewTicket", "Transaction", new { id = sa.Id });
        }

        public async Task<ActionResult> ValidationByManager(Models.TimeZone timezone, string[] email_office, string Hidden_Case_No , string Vender_Remark,string Quality_Remark)
        {
            if (User.IsInRole("Quality") || User.IsInRole("Sr.Help Desk Manager"))
            {
                Log.Information("Validation action started for user {UserId}.", User.Identity.GetUserId());

                int statusId = User.IsInRole("Quality") ? 1632 : 1631;

                List<string> message = new List<string>();

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (timezone == null)
                        {
                            Log.Warning("Timezone data was not provided for ticket ID {TicketId}.", timezone.Ticket_Id);
                            TempData["message"] = new List<string> { "Oops!! Timezone data was not provided." };
                            return RedirectToAction("viewTicket", "Transaction", new { id = timezone.Ticket_Id });
                        }

                        var ticket = await db.Ticket.Include(t => t.TimeZone).FirstOrDefaultAsync(t => t.Id == timezone.Ticket_Id);
                        if (ticket == null)
                        {
                            Log.Warning("Ticket not found for ID {TicketId}.", timezone.Ticket_Id);
                            TempData["message"] = new List<string> { "Oops!! Ticket not found." };
                            return RedirectToAction("viewTicket", "Transaction", new { id = timezone.Ticket_Id });
                        }

                        Log.Information("Updating ticket ID {TicketId} with status {StatusId}.", ticket.Id, statusId);

                        ticket.Case_No = Hidden_Case_No;
                        ticket.In_Time = timezone.InTimeIndia;
                        ticket.Out_Time = timezone.OutTimeIndia;
                        ticket.Status = statusId;
                        ticket.Vender_Remark = Vender_Remark;
                        ticket.Quality_Remark = Quality_Remark;

                        ticket.Total_Hours = timezone.OutTimeLocal.HasValue && timezone.InTimeLocal.HasValue
                     ? (timezone.OutTimeLocal.Value - timezone.InTimeLocal.Value).ToString(@"hh\:mm")
                     : string.Empty;

                        ticket.Total_Hours2 = timezone.OutTimeLocal2.HasValue && timezone.InTimeLocal2.HasValue
                                              ? (timezone.OutTimeLocal2.Value - timezone.InTimeLocal2.Value).ToString(@"hh\:mm")
                                              : string.Empty;


                        var timeZoneRecord = ticket.TimeZone;
                        if (timeZoneRecord == null)
                        {
                            Log.Warning("Timezone record not found for ticket ID {TicketId}.", timezone.Ticket_Id);
                            TempData["message"] = new List<string> { "Oops!! Timezone record not found." };
                            return RedirectToAction("viewTicket", "Transaction", new { id = timezone.Ticket_Id });
                        }

                        UpdateTimeZoneRecord(timeZoneRecord, timezone);

                        var th = new Ticket_History
                        {
                            Ticket_no = timezone.Ticket_Id,
                            status = statusId,
                            CreatedBy = User.Identity.GetUserId(),
                            CreatedOn = DateTime.Now
                        };
                        db.Ticket_History.Add(th);

                        await db.SaveChangesAsync();
                        transaction.Commit();

                        if (User.IsInRole("Quality")  )
                        {
                            SendFinalClosingEmail(ticket, timezone, email_office, message);
                        }

                        Log.Information("Ticket {TicketId} successfully approved with status {StatusId}.", timezone.Ticket_Id, statusId);

                        message.Add("Ticket approved. Thank you for your Time!");
                        TempData["message"] = message;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Log.Error(ex, "An error occurred in the ValidationByManager action for ticket ID {TicketId}.", timezone.Ticket_Id);
                        TempData["message"] = new List<string> { "Oops!! There was an error: " + ex.Message + ". Please contact your developer." };
                    }
                }
                return RedirectToAction("viewTicket", "Transaction", new { id = timezone.Ticket_Id });
            }

            Log.Warning("Unauthorized approval attempt by user {UserId}.", User.Identity.GetUserId());
            TempData["message"] = new List<string> { "Huh, nice try! You are not allowed to approve this." };
            return RedirectToAction("viewTicket", "Transaction", new { id = timezone.Ticket_Id });
        }

        private void SendFinalClosingEmail(Ticket ticket, Models.TimeZone timeZone, string[] email_office, List<string> message)
        {

            var fe = db.FE_Master_Personal.FirstOrDefault(s => s.Id == ticket.FE_ID);
            var fe2 = db.FE_Master_Personal.FirstOrDefault(s => s.Id == ticket.FE_ID_2);
            string eu = db.EU_Master.Where(s => s.Id == ticket.EU_ID).Select(s => s.Customer_Name).FirstOrDefault();
            string callbackUrl = Url.Action("CSAT", "Transaction", new { Id = ticket.Id }, protocol: Request.Url.Scheme);
            string partDetail = GetPartDetail(ticket);
            string sysInfo = GenerateSystemInfoTable(ticket.Id);
            string otherCharge = string.Empty;
            string oem = db.HeaderDescription.Where(s => s.id == ticket.OEM).Select(s => s.header_description).FirstOrDefault();

            var emailSubject = string.Empty;
            var emailData = db.Ticket_Email.FirstOrDefault(t => t.Ticket_no == ticket.Id);

            string customerBody = ticket.FE_ID_2 == null ?
                utlity.CustomterClosing(eu, timeZone.InTimeLocal.Value.ToString("dd/MM/yyyy"), timeZone.InTimeLocal.Value.ToString("HH:mm:ss"), timeZone.OutTimeLocal.Value.ToString("dd/MM/yyyy"), timeZone.OutTimeLocal.Value.ToString("HH:mm:ss"), "", "", "", callbackUrl, ticket.Case_No, ticket.Ticket_No, oem, sysInfo, $"{fe.First_Name} {fe.Last_Name}", ticket.Total_Hours, otherCharge, partDetail, ticket.Job_Description) :
                utlity.CustomterClosing2ndFE(
                    eu,
                    timeZone.InTimeLocal?.ToString("dd/MM/yyyy") ?? string.Empty,
                    timeZone.InTimeLocal?.ToString("HH:mm:ss") ?? string.Empty,
                    timeZone.OutTimeLocal?.ToString("dd/MM/yyyy") ?? string.Empty,
                    timeZone.OutTimeLocal?.ToString("HH:mm:ss") ?? string.Empty,
                    timeZone.InTimeLocal2?.ToString("dd/MM/yyyy") ?? string.Empty,
                   timeZone.InTimeLocal2?.ToString("HH:mm:ss") ?? string.Empty,
                    timeZone.OutTimeLocal2?.ToString("dd/MM/yyyy") ?? string.Empty,
                    timeZone.OutTimeLocal2?.ToString("HH:mm:ss") ?? string.Empty,
                    "", // Parts_retained
                    "", // Parts_returned
                    "", // Other_Details
                    callbackUrl,
                    ticket.Case_No,
                    ticket.Ticket_No,
                    oem,
                    sysInfo,
                    $"{fe.First_Name} {fe.Last_Name}",
                    ticket.Total_Hours.ToString(), // TRT
                    otherCharge,
                    partDetail,
                    $"{fe2.First_Name} {fe2.Last_Name}",
                    ticket.Total_Hours2, // TRT2
                    ticket.Job_Description
                );

            string messageId = RetrieveMessageId(ticket.Id.ToString());

            string combinedEmails = string.Empty;
            if(ticket.Case_No == "000000")
            {
                combinedEmails = "prashant.kori@inwinteck.com";
            }
            else
            {
                if (ticket.CreatedBy == "etc@sourcesupport.com")
                {
                    combinedEmails = "etc@sourcesupport.com,hd@inwinteck.com,sk@inwinteck.com";
                }
                else
                {
                    combinedEmails = "hd@inwinteck.com";
                }
            }

           

            string invAddress = fe.Country == "India" ?
                "Inwinteck Private Limited, 10/34, Vijay Garden, Off G.B Road, Thane (W) -400615" :
                "Inwinteck Pte Ltd, 23 KelantaBn Lane,#04-01 Kim Hoe Centre, Singapore – 208642.";

            utlity.sendmail2(combinedEmails, emailData.Email_Subject, customerBody, mailer, messageId, true);
            string engineerBody = utlity.FEClosing($"{fe.First_Name} {fe.Last_Name}", ticket.Ticket_No, oem, sysInfo, ticket.Total_Hours, "", partDetail, ticket.In_Time.Value.ToString("dd/MM/yyyy"), ticket.In_Time.Value.ToString("HH:mm:ss"), ticket.Out_Time.Value.ToString("dd/MM/yyyy"), ticket.Out_Time.Value.ToString("HH:mm:ss"), "", invAddress);
            utlity.sendmail(fe.Email, $"Inwinteck Ticket no {ticket.Ticket_No} is Closed", engineerBody, mailer);

            message.Add("Closer Email Sent To Customer and Engineer");
        }

        private void UpdateTimeZoneRecord(Models.TimeZone timeZoneRecord, Models.TimeZone timezone)
        {
            timeZoneRecord.InTimeIndia = timezone.InTimeIndia;
            timeZoneRecord.InTimeLocal = timezone.InTimeLocal;
            timeZoneRecord.InTimeUs = timezone.InTimeUs;
            timeZoneRecord.OutTimeIndia = timezone.OutTimeIndia;
            timeZoneRecord.OutTimeLocal = timezone.OutTimeLocal;
            timeZoneRecord.OutTimeUs = timezone.OutTimeUs;

            if (timezone.OutTimeIndia2 != null && timezone.InTimeIndia2 != null)
            {
                timeZoneRecord.InTimeIndia2 = timezone.InTimeIndia2;
                timeZoneRecord.InTimeLocal2 = timezone.InTimeLocal2;
                timeZoneRecord.InTimeUs2 = timezone.InTimeUs2;
                timeZoneRecord.OutTimeIndia2 = timezone.OutTimeIndia2;
                timeZoneRecord.OutTimeLocal2 = timezone.OutTimeLocal2;
                timeZoneRecord.OutTimeUs2 = timezone.OutTimeUs2;
            }
        }


        private async Task HandleTicketReschedule(Ticket oldTicket, FE_Master_Personal fe, string email_office, Ticket_History oldTicketHistory, List<string> messages)
        {
            try
            {
                DateTime? dispatchDate = oldTicket.Reschedule_DT;

                var newTicket = new Ticket
                {
                    Ticket_Type = oldTicket.Ticket_Type,
                    Ticket_Mode = oldTicket.Ticket_Mode,
                    SLA = oldTicket.SLA,
                    Dispatch_Date = oldTicket.Reschedule_DT,
                    Ticket_No = utlity.CheckTicketNo(oldTicket.EU_ID),
                    Status = 18,
                    Pregame = oldTicket.Pregame,
                    latitude = oldTicket.latitude,
                    longitude = oldTicket.longitude,
                    Is_Reschedule = 0,
                    Old_Ticket = oldTicket.Id,
                    CreatedBy = User.Identity.GetUserId(),
                    CreatedOn = DateTime.Now,
                    EU_ID = oldTicket.EU_ID,
                    Site_Name = oldTicket.Site_Name,
                    Street_Address = oldTicket.Street_Address,
                    Zip_Pin_Code = oldTicket.Zip_Pin_Code,
                    City = oldTicket.City,
                    Country = oldTicket.Country,
                    State = oldTicket.State,
                    Ticket_Date = DateTime.Now,
                    TSE_Name = oldTicket.TSE_Name,
                    Certification_Need = oldTicket.Certification_Need,
                    FE_ID = oldTicket.FE_ID,
                    FE_Email = oldTicket.FE_Email,
                    ph_contact = oldTicket.ph_contact,
                    Case_No = oldTicket.Case_No,
                    Job_Description = oldTicket.Job_Description,
                    pregame_detail = oldTicket.pregame_detail,
                    pregame_upload = oldTicket.pregame_upload,
                    OEM = oldTicket.OEM,
                    Certification_Name = oldTicket.Certification_Name,
                    IsSecondFEEnabled = oldTicket.IsSecondFEEnabled,
                    FE_ID_2 = oldTicket.FE_ID_2,
                    EmailMessageId = Guid.NewGuid().ToString()
                };

                db.Ticket.Add(newTicket);
                await db.SaveChangesAsync();

                // Add ticket history, email, and system info for reschedule
                AddTicketHistoryForReschedule(newTicket, oldTicket);
                AddTicketEmailForReschedule(newTicket, oldTicket);
                AddSystemInfoForReschedule(newTicket, oldTicket);

                // Convert time zones
                if (newTicket.Ticket_Date.HasValue)
                {
                    var caseDt = await Universal.CommonMethod.ConvertIstToLocalAndUtcAsync(oldTicket.latitude, oldTicket.longitude, newTicket.Ticket_Date.Value);
                    var dispDt = newTicket.Dispatch_Date.HasValue
                        ? await Universal.CommonMethod.ConvertLocalToIstAndUtcAsync(oldTicket.latitude, oldTicket.longitude, newTicket.Dispatch_Date.Value)
                        : (localToUs: (DateTime?)null, localToIst: (DateTime?)null);

                    var timeZoneRecord = new Models.TimeZone
                    {
                        Ticket_Id = newTicket.Id,
                        CaseDtIndia = newTicket.Ticket_Date.Value,
                        CaseDtUs = caseDt.istToUs,
                        CaseDtLocal = caseDt.istToLocal,
                        DispDtLocal = newTicket.Dispatch_Date,
                        DispDtIndia = dispDt.localToIst,
                        DispDtUs = dispDt.localToUs
                    };

                    db.TimeZone.Add(timeZoneRecord);
                    await db.SaveChangesAsync();

                    // Update SLA and TimeZoneId
                    if (newTicket.Dispatch_Date.HasValue)
                    {
                        newTicket.SLA = Universal.CommonMethod.CalculateSLA(timeZoneRecord.DispDtLocal, timeZoneRecord.CaseDtLocal);
                    }
                    newTicket.TimeZoneId = timeZoneRecord.Id;
                    db.Entry(newTicket).State = EntityState.Modified;
                }

                int res = await db.SaveChangesAsync();
                messages.Add($"Ticket has been Rescheduled. New Ticket No {newTicket.Ticket_No}");

                if (res > 0)
                {
                    if(newTicket.Case_No != "000000")
                    {
                        await RescheduleNotifyCustomerAndFE(newTicket, oldTicket, fe, email_office, messages);
                    }
                }
            }
            catch (Exception ex)
            {
                utlity.createlog($"/Ticket/Not Rescheduled Ticket No : {oldTicket.Id} Error: {ex.Message}");
                messages.Add("Ticket Not Rescheduled !!");
            }
        }

        private async Task RescheduleNotifyCustomerAndFE(Ticket newTicket, Ticket oldTicket, FE_Master_Personal fe, string email_office, List<string> messages)
        {
            try
            {
                string oldTicketNo = await db.Ticket.Where(s => s.Id == newTicket.Old_Ticket).Select(s => s.Ticket_No).FirstOrDefaultAsync();
                string eu = await db.EU_Master.Where(s => s.Id == oldTicket.EU_ID).Select(s => s.Customer_Name).FirstOrDefaultAsync();
                var newDispatchedDate = newTicket.Dispatch_Date?.ToString("dd MMM yyyy HH':'mm tt");

                string body = utlity.CustomerReschedule(eu, newTicket.Ticket_No, oldTicket.Case_No, newDispatchedDate, oldTicketNo, oldTicket.Reschedule_Reason);
                utlity.sendmail(email_office, email_office, body, mailer);
                string body1 = utlity.FEReschedule($"{fe.First_Name} {fe.Last_Name}", oldTicketNo, newDispatchedDate, oldTicket.Reschedule_Reason);
                utlity.sendmail2(fe.Email, $"Ticket Rescheduled for Inwinteck Ticket no : {oldTicketNo}", body1, mailer, newTicket.EmailMessageId, false);

                messages.Add("Customer and FE have been informed");
            }
            catch (Exception ex)
            {
                messages.Add("Unable to Send Email to FE and Customer: " + ex.Message);
            }
        }

        private void AddTicketHistoryForReschedule(Ticket newTicket, Ticket oldTicket)
        {
            var th = new Ticket_History
            {
                Ticket_no = newTicket.Id,
                FE_ID = newTicket.FE_ID,
                status = 18,
                CreatedBy = User.Identity.GetUserId(),
                CreatedOn = DateTime.Now
            };

            db.Ticket_History.Add(th);
            // No SaveChanges here, we'll save everything at once later
        }

        private void AddTicketEmailForReschedule(Ticket newTicket, Ticket oldTicket)
        {
            var oldEmail = db.Ticket_Email.FirstOrDefault(s => s.Ticket_no == oldTicket.Id);
            if (oldEmail != null)
            {
                var newEmail = new Ticket_Email
                {
                    Ticket_no = newTicket.Id,
                    //Email = oldEmail.Email,
                    Email_Subject = oldEmail.Email_Subject,
                    CreatedBy = User.Identity.GetUserId(),
                    CreatedOn = DateTime.Now
                };

                db.Ticket_Email.Add(newEmail);
                // No SaveChanges here, we'll save everything at once later
            }
        }

        private void AddSystemInfoForReschedule(Ticket newTicket, Ticket oldTicket)
        {
            var oldSystemInfo = db.Ticket_System_Info.Where(s => s.Ticket_no == oldTicket.Id).ToList();
            foreach (var info in oldSystemInfo)
            {
                var newSystemInfo = new Ticket_System_Info
                {
                    Ticket_no = newTicket.Id,
                    System_Information = info.System_Information,
                    Make_Model = info.Make_Model,
                    Serial_Number = info.Serial_Number,
                    Required_Tool = info.Required_Tool,
                    CreatedBy = User.Identity.GetUserId(),
                    CreatedOn = DateTime.Now
                };

                db.Ticket_System_Info.Add(newSystemInfo);
                // No SaveChanges here, we'll save everything at once later
            }
        }

        private void HandleFileUpload(Ticket sa, HttpPostedFileBase pht)
        {
            string pic = Path.GetFileName(pht.FileName);
            string fileName = $"{sa.Id}{DateTime.Now:yyyyMMddHHmmssFFF}_{pic}";
            string path = Path.Combine(Server.MapPath("~/Upload/Pregame"), fileName);
            pht.SaveAs(path);

            sa.pregame_upload = url + "Pregame/" + fileName;
        }

        private void AddTicketHistory(Ticket sa, Ticket_History ticketHistory, FE_Master_Personal fe, string email_office)
        {
            //var slaValue = db.Ticket.Where(t => t.Id == sa.Id)
            //             .Select(t => t.SLA)
            //             .FirstOrDefault();

            var sla = db.HeaderDescription.FirstOrDefault(hd => hd.id == sa.SLA)?.header_description;
            var th = new Ticket_History
            {
                Ticket_no = sa.Id,
                Comments = ticketHistory.Comments,
                FE_ID = sa.FE_ID,
                status = sa.Status,
                SLA = sla,
                CreatedBy = User.Identity.GetUserId(),
                CreatedOn = DateTime.Now,
                ModifiedBy = User.Identity.GetUserName(),
                ModifiedOn = DateTime.Now
            };

            db.Ticket_History.Add(th);
            db.SaveChanges();

        }

        private void AddPartTicketData(Ticket sa, Part_Ticket_Data iv)
        {
            var TSI = new Part_Ticket_Data
            {
                Ticket_No = sa.Id,
                Make_Model = iv.Make_Model,
                Part_Description = iv.Part_Description,
                Part_type = iv.Part_type,
                Serial_No = iv.Serial_No,
                CreatedBy = User.Identity.GetUserId(),
                CreatedOn = DateTime.Now
            };

            db.Part_Ticket_Data.Add(TSI);
            db.SaveChanges();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateDispatchDateBtn(int ticketId, DateTime caseDate, DateTime dispatchedDate, string lat, string lng)
        {
            var dispatchedDt = await Universal.CommonMethod.ConvertLocalToIstAndUtcAsync(lat, lng, dispatchedDate);
            var existTimezone = db.TimeZone.FirstOrDefault(t => t.Ticket_Id == ticketId);

            if (existTimezone != null)
            {
                existTimezone.DispDtUs = dispatchedDt.localToUs;
                existTimezone.DispDtIndia = dispatchedDt.localToIst;
                existTimezone.DispDtLocal = dispatchedDate;

                var SLA = Universal.CommonMethod.CalculateSLA(dispatchedDate, caseDate);
                var ticket = db.Ticket.FirstOrDefault(t => t.Id == ticketId);

                if (ticket != null)
                {
                    ticket.SLA = SLA;
                    ticket.Dispatch_Date = dispatchedDate;
                    await db.SaveChangesAsync();
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Timezone or Ticket entry not found.");
        }



        private async Task<ActionResult> HandleTicketClosure(Ticket sa, FE_Master_Personal fe, FE_Master_Personal fe2, string email_office, string emailSubject, List<string> messages, Models.TimeZone timezoneModel)
        {
            if (sa.Status != 20) // Ticket is not closed
            {
                return RedirectToAction("editTicket", "Transaction", new { id = sa.Id });
            }

            var timeZoneRecord = await db.TimeZone.FirstOrDefaultAsync(tz => tz.Ticket_Id == sa.Id);
            if (timeZoneRecord != null && sa.Out_Time.HasValue)
            {
                timeZoneRecord.OutTimeIndia = sa.Out_Time;
                timeZoneRecord.OutTimeLocal = timezoneModel.OutTimeLocal;
                timeZoneRecord.OutTimeUs = timezoneModel.OutTimeUs;

                if (timezoneModel.OutTimeLocal2.HasValue && timezoneModel.OutTimeUs2.HasValue)
                {
                    timeZoneRecord.OutTimeIndia2 = sa.Out_Time2;
                    timeZoneRecord.OutTimeLocal2 = timezoneModel.OutTimeLocal2;
                    timeZoneRecord.OutTimeUs2 = timezoneModel.OutTimeUs2;
                }

                if (timeZoneRecord.InTimeLocal.HasValue && timeZoneRecord.OutTimeLocal.HasValue)
                {
                    TimeSpan totalHours = timeZoneRecord.OutTimeLocal.Value - timeZoneRecord.InTimeLocal.Value;
                    sa.Total_Hours = $"{(int)totalHours.TotalHours} hours {totalHours.Minutes} minutes";
                }

                if (sa.In_Time2.HasValue && sa.Out_Time2.HasValue)
                {
                    TimeSpan totalHours2 = timeZoneRecord.OutTimeLocal2.Value - timeZoneRecord.InTimeLocal2.Value;
                    sa.Total_Hours2 = $"{(int)totalHours2.TotalHours} hours {totalHours2.Minutes} minutes";
                }

                await db.SaveChangesAsync();
            }

            //string sysInfo = GenerateSystemInfoTable(sa.Id);
            //string partDetail = GetPartDetail(sa);
            //string callbackUrl = Url.Action("CSAT", "Transaction", new { Id = sa.Id }, protocol: Request.Url.Scheme);
            //string oem = db.HeaderDescription.Where(s => s.id == sa.OEM).Select(s => s.header_description).FirstOrDefault();
            //string eu = db.EU_Master.Where(s => s.Id == sa.EU_ID).Select(s => s.Customer_Name).FirstOrDefault();
            //string otherCharge = string.Empty;

            //string customerBody = sa.FE_ID_2 == null ?
            //    utlity.CustomterClosing(eu, timeZoneRecord.InTimeLocal.Value.ToString("dd/MM/yyyy"), timeZoneRecord.InTimeLocal.Value.ToString("HH:mm:ss"), timeZoneRecord.OutTimeLocal.Value.ToString("dd/MM/yyyy"), timeZoneRecord.OutTimeLocal.Value.ToString("HH:mm:ss"), "", "", "", callbackUrl, sa.Case_No, sa.Ticket_No, oem, sysInfo, $"{fe.First_Name} {fe.Last_Name}", sa.Total_Hours, otherCharge, partDetail, sa.Job_Description) :
            //    utlity.CustomterClosing2ndFE(
            //        eu,
            //        timeZoneRecord.InTimeLocal?.ToString("dd/MM/yyyy") ?? string.Empty,
            //        timeZoneRecord.InTimeLocal?.ToString("HH:mm:ss") ?? string.Empty,
            //        timeZoneRecord.OutTimeLocal?.ToString("dd/MM/yyyy") ?? string.Empty,
            //        timeZoneRecord.OutTimeLocal?.ToString("HH:mm:ss") ?? string.Empty,
            //        timeZoneRecord.InTimeLocal2?.ToString("dd/MM/yyyy") ?? string.Empty,
            //       timeZoneRecord.InTimeLocal2?.ToString("HH:mm:ss") ?? string.Empty,
            //        timeZoneRecord.OutTimeLocal2?.ToString("dd/MM/yyyy") ?? string.Empty,
            //        timeZoneRecord.OutTimeLocal2?.ToString("HH:mm:ss") ?? string.Empty,
            //        "", // Parts_retained
            //        "", // Parts_returned
            //        "", // Other_Details
            //        callbackUrl,
            //        sa.Case_No,
            //        sa.Ticket_No,
            //        oem,
            //        sysInfo,
            //        $"{fe.First_Name} {fe.Last_Name}",
            //        sa.Total_Hours.ToString(), // TRT
            //        otherCharge,
            //        partDetail,
            //        $"{fe2.First_Name} {fe2.Last_Name}",
            //        sa.Total_Hours2, // TRT2
            //        sa.Job_Description
            //    );

            //string messageId = RetrieveMessageId(sa.Id.ToString());

            //string combinedEmails = email_office[0];
            //string invAddress = fe.Country == "India" ?
            //    "Inwinteck Private Limited, 10/34, Vijay Garden, Off G.B Road, Thane (W) -400615" :
            //    "Inwinteck Pte Ltd, 23 Kelantan Lane,#04-01 Kim Hoe Centre, Singapore – 208642.";

            //utlity.sendmail2(combinedEmails, emailSubject, customerBody, mailer, messageId, true);
            //string engineerBody = utlity.FEClosing($"{fe.First_Name} {fe.Last_Name}", sa.Ticket_No, oem, sysInfo, sa.Total_Hours, "", partDetail, sa.In_Time.Value.ToString("dd/MM/yyyy"), sa.In_Time.Value.ToString("HH:mm:ss"), sa.Out_Time.Value.ToString("dd/MM/yyyy"), sa.Out_Time.Value.ToString("HH:mm:ss"), "", invAddress);
            //utlity.sendmail(fe.Email, $"Inwinteck Ticket no {sa.Ticket_No} is Closed", engineerBody, mailer);

            //TempData["message"] = new List<string> { "Closer Email Sent To Customer and Engineer", "Ticket Closed" };
            TempData["message"] = new List<string> { "Ticket Closed" };
            return RedirectToAction("viewTicket", "Transaction", new { id = sa.Id });
        }

        private string GenerateSystemInfoTable(int ticketId)
        {
            string sysInfo = "<table style='width: 100%; height: 40px; padding-left: 20px; border: 1px;'><tr style='background: rgb(131, 194, 51);'><td>System Information</td><td>Make/Model</td><td>Serial Number</td><td>Required Tool</td></tr>";

            var tsi = db.Ticket_System_Info.Where(s => s.Ticket_no == ticketId).ToList();
            foreach (var info in tsi)
            {
                sysInfo += $"<tr><td>{info.System_Information}</td><td>{info.Make_Model}</td><td>{info.Serial_Number}</td><td>{info.Required_Tool}</td></tr>";
            }
            sysInfo += "</table>";

            return sysInfo;
        }

        private string GetPartDetail(Ticket sa)
        {
            string partDetail = "";

            if (sa.Part_Management == 22)
            {
                string returnLabelDetail = sa.Return_Label == 1 ? "Return Label Available" : "Return Label Not Available";
                //  string storageChargeDetail = sa.FE_Part_Storage_Charge == 1 ? "Part Storage Charges is applicable." : "Part Storage Charges is not applicable.";
                partDetail = $"Part Management: Pick up, {returnLabelDetail}. Tracking Number: {sa.Tracking_number} ";
            }
            else if (sa.Part_Management == 23)
            {
                string partHandoverDescription = db.HeaderDescription.Where(s => s.id == sa.Part_Handover).Select(s => s.header_description).FirstOrDefault();
                partDetail = $"Part Management: Handover, Part Handover {partHandoverDescription}. Contact Name: {sa.ph_Name}, Contact Number: {sa.ph_contact}";
            }
            else if (sa.Part_Management == 1371)
            {
                partDetail = "Part Management: Not Applicable";
            }

            return partDetail;
        }

        [Authorize(Roles = "Admin, Sr.Help Desk Manager, Help Desk Manager ,Quality")]

        public ActionResult viewTicket(int id)
        {
            var ticket = db.Ticket.Include(t => t.TimeZone).FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var formattedCaseDtUs = ticket.TimeZone?.CaseDtUs?.ToString("yyyy-MM-ddTHH:mm");
            var formattedCaseDtLocal = ticket.TimeZone?.CaseDtLocal?.ToString("yyyy-MM-ddTHH:mm");

            var formattedDispDtUs = ticket.TimeZone?.DispDtUs?.ToString("yyyy-MM-ddTHH:mm");
            var formattedDispDtIndia = ticket.TimeZone?.DispDtIndia?.ToString("yyyy-MM-ddTHH:mm");

            var formattedInTimeIndia = ticket.TimeZone?.InTimeIndia?.ToString("yyyy-MM-ddTHH:mm");
            var formattedInTimeUs = ticket.TimeZone?.InTimeUs?.ToString("yyyy-MM-ddTHH:mm");
            var formattedInTimeLocal = ticket.TimeZone?.InTimeLocal?.ToString("yyyy-MM-ddTHH:mm");
            var formattedOutTimeIndia = ticket.TimeZone?.OutTimeIndia?.ToString("yyyy-MM-ddTHH:mm");
            var formattedOutTimeUs = ticket.TimeZone?.OutTimeUs?.ToString("yyyy-MM-ddTHH:mm");
            var formattedOutTimeLocal = ticket.TimeZone?.OutTimeLocal?.ToString("yyyy-MM-ddTHH:mm");

            var formattedInTimeIndia2 = ticket.TimeZone?.InTimeIndia2?.ToString("yyyy-MM-ddTHH:mm");
            var formattedInTimeUs2 = ticket.TimeZone?.InTimeUs2?.ToString("yyyy-MM-ddTHH:mm");
            var formattedInTimeLocal2 = ticket.TimeZone?.InTimeLocal2?.ToString("yyyy-MM-ddTHH:mm");
            var formattedOutTimeIndia2 = ticket.TimeZone?.OutTimeIndia2?.ToString("yyyy-MM-ddTHH:mm");
            var formattedOutTimeUs2 = ticket.TimeZone?.OutTimeUs2?.ToString("yyyy-MM-ddTHH:mm");
            var formattedOutTimeLocal2 = ticket.TimeZone?.OutTimeLocal2?.ToString("yyyy-MM-ddTHH:mm");

            ViewBag.CaseDtUs = formattedCaseDtUs;
            ViewBag.CaseDtLocal = formattedCaseDtLocal;

            ViewBag.formattedDispDtUs = formattedDispDtUs;
            ViewBag.formattedDispDtIndia = formattedDispDtIndia;
            ViewBag.inTimeIndia = formattedInTimeIndia;
            ViewBag.inTimeUs = formattedInTimeUs;
            ViewBag.inTimeLocal = formattedInTimeLocal;
            ViewBag.OutTimeIndia = formattedOutTimeIndia;
            ViewBag.OutTimeUs = formattedOutTimeUs;
            ViewBag.OutTimeLocal = formattedOutTimeLocal;

            ViewBag.inTimeIndia2 = formattedInTimeIndia2;
            ViewBag.inTimeUs2 = formattedInTimeUs2;
            ViewBag.inTimeLocal2 = formattedInTimeLocal2;
            ViewBag.OutTimeIndia2 = formattedOutTimeIndia2;
            ViewBag.OutTimeUs2 = formattedOutTimeUs2;
            ViewBag.OutTimeLocal2 = formattedOutTimeLocal2;

            var qualityMarksBraekdown = (from s in db.QualityMarksBraekdown where s.Ticket_id == ticket.Id select s).FirstOrDefault();
            if (qualityMarksBraekdown == null)
            {
                qualityMarksBraekdown = new QualityMarksBraekdown { Ticket_id = ticket.Id };
            }
            if (ticket == null)
            {
                return RedirectToAction("NotFound", "Dashboard"); // Handle case where ticket is not found
            }
            TicketQualityViewModel ti = new TicketQualityViewModel
            {
                Ticket = ticket,
                QualityMarksBraekdown = qualityMarksBraekdown,
            };

            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();
            ViewBag.Customer = (from s in db.EU_Master where s.Id == ti.Ticket.EU_ID select s.Customer_Name).FirstOrDefault();
            ViewBag.OEM = (from c in db.HeaderDescription where c.header_id == 4 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Status = (from c in db.HeaderDescription where c.header_id == 5 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Part_Management = (from c in db.HeaderDescription where c.header_id == 6 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Part_Handover = (from c in db.HeaderDescription where c.header_id == 7 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Ticket_Type = (from c in db.HeaderDescription where c.header_id == 11 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Ticket_Mode = (from c in db.HeaderDescription where c.header_id == 13 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.SLA = (from c in db.HeaderDescription where c.header_id == 14 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Ticket_Priority = (from c in db.HeaderDescription where c.header_id == 15 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Certification = (from c in db.Certification_Master where c.Status == 1 select new SelectListItem { Text = c.Certification_Name, Value = SqlFunctions.StringConvert((double)c.Id).Trim() }).ToList();
            ViewBag.Cust_Business = (from s in db.EU_Rate_Card where s.EU_ID == ti.Ticket.EU_ID && s.Country == ti.Ticket.Country select s.Business_Hr).FirstOrDefault();
            ViewBag.Cust_Business_Non = (from s in db.EU_Rate_Card where s.EU_ID == ti.Ticket.EU_ID && s.Country == ti.Ticket.Country select s.Non_Business_Hr).FirstOrDefault();
            ViewBag.Cust_Minimum = (from s in db.EU_Rate_Card where s.EU_ID == ti.Ticket.EU_ID && s.Country == ti.Ticket.Country select s.Minimum_Hrs).FirstOrDefault();
            ViewBag.Cust_Charges_job = (from s in db.EU_Rate_Card where s.EU_ID == ti.Ticket.EU_ID && s.Country == ti.Ticket.Country select s.Per_Job).FirstOrDefault();
            ViewBag.FE = (from s in db.FE_Master_Personal where s.Id == ti.Ticket.FE_ID select s.First_Name).FirstOrDefault();
            ViewBag.Decline_Reason = (from c in db.HeaderDescription where c.header_id == 16 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Cancel_Reason = (from c in db.HeaderDescription where c.header_id == 17 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Ticket = "INWIN000" + ti.Ticket.Id;
            ViewBag.Currency = (from s in db.Currency_Master where s.Country == ti.Ticket.Country select s.Currency).FirstOrDefault();
            ViewBag.EU_Office = (from s in db.EU_Master_Branch where s.Id == ti.Ticket.EU_Office select s.Office).FirstOrDefault();
            ViewBag.Email_Subject = (from s in db.Ticket_Email where s.Ticket_no == ti.Ticket.Id select s.Email_Subject).FirstOrDefault();
            ViewBag.email_office = (from s in db.Ticket_Email where s.Ticket_no == ti.Ticket.Id select s.Email).FirstOrDefault();
            ViewBag.Old_Ticket = (from s in db.Ticket where s.Id == ti.Ticket.Old_Ticket select s.Ticket_No).FirstOrDefault();
            ViewBag.New_Ticket = (from s in db.Ticket where s.Old_Ticket == ti.Ticket.Id select s.Ticket_No).FirstOrDefault();
            var qualityRemarkPermissions = db.Quality_Remark_Permission.Select(s => s.userId).ToList();
            ViewBag.Quality_Remark_Permission = qualityRemarkPermissions;

            var hist = db.Database.SqlQuery<TicketHist>("gethistory @id", new SqlParameter("@id", id)).ToList();
            ViewBag.HandledBy = new List<string>
            {
                "ramesh.anuse@inwinteck.com",
                "malang.bagwe@inwinteck.com",
                "raj.kumbhar@inwinteck.com",
                "Abhijit.Jadhav@inwinteck.com",
                "karan.sonar@inwinteck.com",
                "prajakta.karale@inwinteck.com",
                "pranali.kamble@inwinteck.com",
                "ashish.thakur@inwinteck.com",
                "harishgupta51@inwinteck.com",
                "mukesh52@inwinteck.com",
                "sona@inwinteck.com",
                "santosh.chavan@inwinteck.com",
                "Pawan.Yadav@inwinteck.com"
            };
            ViewBag.Currency = db.Currency_Master.ToList();
            return View(ti);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult viewTicketUpdate(QualityMarksBraekdown qualityMarksBraekdown)
        {
            if (!User.IsInRole("Quality"))
            { 
                TempData["message"] = new List<string> { "You are Not Authorized For Update" };
                return RedirectToAction("viewTicket", new { id = qualityMarksBraekdown.Ticket_id });
            }

            List<string> messages = new List<string>();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingQualityScore = db.QualityMarksBraekdown.SingleOrDefault(q => q.Ticket_id == qualityMarksBraekdown.Ticket_id);
                    if (existingQualityScore == null)
                    {
                        qualityMarksBraekdown.CreatedBy = User.Identity.GetUserName();
                        qualityMarksBraekdown.CreatedOn = DateTime.Now;
                        qualityMarksBraekdown.ModifiedOn = DateTime.Now;
                        qualityMarksBraekdown.ModifiedBy = User.Identity.GetUserName();
                        db.QualityMarksBraekdown.Add(qualityMarksBraekdown);
                        db.SaveChanges();
                        TempData["message"] = new List<string> { "Quality Score updated successfully." };
                    }
                    else
                    {

                        // Update existing record
                        existingQualityScore.Greet_FE_Score = qualityMarksBraekdown.Greet_FE_Score;
                        existingQualityScore.GD_Name_And_Handover_Score = qualityMarksBraekdown.GD_Name_And_Handover_Score;
                        existingQualityScore.Pregame_Manual_Site_Contact_Score = qualityMarksBraekdown.Pregame_Manual_Site_Contact_Score;
                        existingQualityScore.Greet_TSE_Score = qualityMarksBraekdown.Greet_TSE_Score;
                        existingQualityScore.Comm_Verif_Serial_Number_Score = qualityMarksBraekdown.Comm_Verif_Serial_Number_Score;
                        existingQualityScore.FE_Following_Instruction_Score = qualityMarksBraekdown.FE_Following_Instruction_Score;
                        existingQualityScore.Parts_Details_Score = qualityMarksBraekdown.Parts_Details_Score;
                        existingQualityScore.Closer_Form_Score = qualityMarksBraekdown.Closer_Form_Score;
                        existingQualityScore.Ticket_Creation_Score = qualityMarksBraekdown.Ticket_Creation_Score;
                        existingQualityScore.Thank_You_Score = qualityMarksBraekdown.Thank_You_Score;
                        existingQualityScore.Total_Score = qualityMarksBraekdown.Total_Score;
                        existingQualityScore.Additional_Remark = qualityMarksBraekdown.Additional_Remark;
                        existingQualityScore.Handler1Name = qualityMarksBraekdown.Handler1Name;
                        existingQualityScore.Handler1Score = qualityMarksBraekdown.Handler1Score;
                        existingQualityScore.Remark1 = qualityMarksBraekdown.Remark1;
                        existingQualityScore.Handler2Name = qualityMarksBraekdown.Handler2Name;
                        existingQualityScore.Handler2Score = qualityMarksBraekdown.Handler2Score;
                        existingQualityScore.Remark2 = qualityMarksBraekdown.Remark2;
                        existingQualityScore.Handler3Name = qualityMarksBraekdown.Handler3Name;
                        existingQualityScore.Handler3Score = qualityMarksBraekdown.Handler3Score;
                        existingQualityScore.Remark3 = qualityMarksBraekdown.Remark2;
                        existingQualityScore.ModifiedBy = User.Identity.GetUserName();
                        existingQualityScore.ModifiedOn = DateTime.Now;

                        db.Entry(existingQualityScore).State = EntityState.Modified;
                        TempData["message"] = new List<string> { "Quality Score updated successfully." };
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    TempData["message"] = new List<string> { "An error occurred while updating the ticket. Please try again later" };
                }
            }
            else
            {
                TempData["ErrorMessage"] = "There was an issue with the data submitted. Please review and try again.";
            }

            return RedirectToAction("viewTicket", new { id = qualityMarksBraekdown.Ticket_id });
        }




        /*----------------------------------------------------------------------------------------------*/

        public ActionResult FEInvoice(DateTime? from, DateTime? to, int FE_ID = 0)
        {
            List<ManageHeader_FE> MH = new List<ManageHeader_FE>();
            List<FEL> FE = new List<FEL>();
            FE = (from c in db.Ticket
                  join r in db.FE_Master_Personal on c.FE_ID equals r.Id
                  where c.Status == 20
                  select new FEL
                  { id = r.Id, name = r.First_Name + "" + r.Last_Name }).Distinct().ToList();
            ViewBag.FE = FE;


            if (from != null && to != null && FE_ID > 0)
            {

                string frm = from.Value.ToString("yyyyMMdd");
                string to1 = to.Value.ToString("yyyyMMdd");
                MH = db.Database.SqlQuery<ManageHeader_FE>("GetTicketPriceFE @FE,@from,@to",
                    new SqlParameter("@FE", FE_ID), new SqlParameter("@from", frm),
                    new SqlParameter("@to", to1)).ToList();
                ViewBag.Message = 1;
            }
            else
            {
                MH = null;
                ViewBag.Message = 0;
            }

            return View(MH);
        }

        public ActionResult GenerateInvoiceFE(int Ticket)
        {
            Invoice_FE inv = new Invoice_FE();
            inv = db.Database.SqlQuery<Invoice_FE>("GetFEInvoiceDetail @id", new SqlParameter("@id ", Ticket)).FirstOrDefault();

            DateTime start = inv.In_Time;
            DateTime end = inv.Out_Time;

            int count = 0;
            int countnon = 0;

            for (var i = start; i < end; i = i.AddMinutes(15))
            {
                if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (i.TimeOfDay.Hours >= 9 && i.TimeOfDay.Hours < 17)
                    {
                        count++;
                    }
                    else
                    {
                        countnon++;
                    }

                }
                else
                {
                    countnon++;
                }
            }

            TimeSpan bc = TimeSpan.FromMinutes(15 * count);
            TimeSpan bnc = TimeSpan.FromMinutes(15 * countnon);
            ViewBag.Bussiness = string.Format("{0:00}:{1:00}", (int)bc.TotalHours, bc.Minutes);
            ViewBag.Bussinessnon = string.Format("{0:00}:{1:00}", (int)bnc.TotalHours, bnc.Minutes);
            ViewBag.BussinessnonChg = ((inv.FE_Non_Buss_Hrs / 4) * countnon);
            ViewBag.BussinessChg = ((inv.FE_Buss_Hrs / 4) * count);

            if (inv.FE_Payment_Mode == "F")
            {
                ViewBag.Total = inv.FE_Fixed + inv.FE_Allowance;
            }
            else if (inv.FE_Payment_Mode == "H")
            {
                ViewBag.Total = ViewBag.BussinessnonChg + ViewBag.BussinessChg + inv.FE_Allowance;
            }

            return View(inv);
        }

        [HttpPost]
        public ActionResult GenerateInvoiceFE(Header_Invoice_Detail_FE sa)
        {
            int cnt = (from s in db.Header_Invoice_Detail_FE where s.Ticket_no == sa.Ticket_no select s).Count();
            decimal amt_total = 0;
            if (cnt > 0)
            {
                TempData["message"] = "Invoice allready Generated !! ";
            }
            else
            {
                try
                {
                    if (sa.Fixed_amt > 0)
                    {

                        amt_total = sa.Fixed_amt + sa.Travel_Charge + sa.Part_Handling_Charge;
                    }
                    else
                    {
                        amt_total = sa.Business_hour_amt + sa.Non_Business_hour_amt + sa.Travel_Charge + sa.Part_Handling_Charge;

                    }
                    Header_Invoice_FE hf = new Header_Invoice_FE();
                    hf.FE_ID = sa.FE_ID;
                    hf.Total_Amt = amt_total;
                    hf.Invoice_Date = DateTime.Now.Date;
                    hf.FE_Payment_Status = "Unpaid";
                    hf.CreatedBy = User.Identity.GetUserId();
                    hf.CreatedOn = DateTime.Now;
                    db.Header_Invoice_FE.Add(hf);
                    int res = db.SaveChanges();
                    if (res > 0)
                    {

                        sa.Inv_no = hf.Id;
                        sa.CreatedBy = User.Identity.GetUserId();
                        sa.CreatedOn = DateTime.Now;
                        db.Header_Invoice_Detail_FE.Add(sa);
                        db.SaveChanges();


                    }

                    TempData["message"] = "FE Invoice Generated : Invoice No !!" + " " + hf.Id;
                    TempData["link"] = "Yes";
                    TempData["INV"] = hf.Id;
                }
                catch (Exception ex)
                {
                    TempData["message"] = "Invoice Not Generated !! ";
                }
            }

            return RedirectToAction("AllInvoiceFE", "Transaction");
        }
        public ActionResult PrintInvoiceFE(int InvoiceNo)
        {
            Invoice_FE_Print inv = new Invoice_FE_Print();
            inv = db.Database.SqlQuery<Invoice_FE_Print>("GetFEInvoicePrint @id", new SqlParameter("@id ", InvoiceNo)).FirstOrDefault();

            return View(inv);
        }

        public ActionResult AllInvoiceFE()
        {
            List<Invoice_FE_Print> inv = new List<Invoice_FE_Print>();
            try
            {
                inv = db.Database.SqlQuery<Invoice_FE_Print>("GetFEInvoiceAll").ToList();
                ViewBag.Payment_Mode = (from c in db.HeaderDescription where c.header_id == 10 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            }
            catch (Exception ex)
            {

            }
            return View(inv);
        }
        /*----------------------------------------------------------------------------------------------*/
        public ActionResult EUInvoice(DateTime? from, DateTime? to, int EU_ID = 0)
        {
            List<ManageHeader_FE> MH = new List<ManageHeader_FE>();
            List<FEL> EU = new List<FEL>();
            EU = (from c in db.Ticket
                  join r in db.EU_Master on c.EU_ID equals r.Id
                  where c.Status == 20
                  select new FEL
                  { id = r.Id, name = r.Customer_Name }).Distinct().ToList();
            ViewBag.EU = EU;


            if (from != null && to != null && EU_ID > 0)
            {

                string frm = from.Value.ToString("yyyyMMdd");
                string to1 = to.Value.ToString("yyyyMMdd");
                MH = db.Database.SqlQuery<ManageHeader_FE>("GetTicketPriceEU @EU,@from,@to",
                    new SqlParameter("@EU", EU_ID), new SqlParameter("@from", frm),
                    new SqlParameter("@to", to1)).ToList();
                ViewBag.Message = 1;
            }
            else
            {
                MH = null;
                ViewBag.Message = 0;
            }

            return View(MH);
        }

        public ActionResult GenerateInvoiceEU(int Ticket)
        {
            Invoice_EU inv = new Invoice_EU();
            inv = db.Database.SqlQuery<Invoice_EU>("GetEUInvoiceDetail @id", new SqlParameter("@id ", Ticket)).FirstOrDefault();



            int count = 0;
            int countnon = 0;
            if (inv.Cancel_Charge > 0)
            {
                ViewBag.Total = inv.Cancel_Charge;
            }
            else
            {
                DateTime start = inv.In_Time;
                DateTime end = inv.Out_Time;
                for (var i = start; i < end; i = i.AddMinutes(15))
                {
                    if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday)
                    {
                        if (i.TimeOfDay.Hours >= 9 && i.TimeOfDay.Hours < 17)
                        {
                            count++;
                        }
                        else
                        {
                            countnon++;
                        }

                    }
                    else
                    {
                        countnon++;
                    }
                }
                TimeSpan bc = TimeSpan.FromMinutes(15 * count);
                TimeSpan bnc = TimeSpan.FromMinutes(15 * countnon);
                ViewBag.Bussiness = string.Format("{0:00}:{1:00}", (int)bc.TotalHours, bc.Minutes);
                ViewBag.Bussinessnon = string.Format("{0:00}:{1:00}", (int)bnc.TotalHours, bnc.Minutes);
                ViewBag.BussinessnonChg = ((inv.CT_Non_Buss_Hrs / 4) * countnon);
                ViewBag.BussinessChg = ((inv.CT_Buss_Hrs / 4) * count);
                if (inv.CT_Payment_Mode == "F")
                {
                    ViewBag.Total = inv.CT_Fixed + inv.CT_Allowance + inv.Reschedule_Charge;
                }
                else if (inv.CT_Payment_Mode == "H")
                {
                    ViewBag.Total = ViewBag.BussinessnonChg + ViewBag.BussinessChg + inv.CT_Allowance + inv.Reschedule_Charge;
                }

            }



            return View(inv);
        }

        [HttpPost]
        public ActionResult GenerateInvoiceEU(Header_Invoice_Detail_EU sa)
        {
            int cnt = (from s in db.Header_Invoice_Detail_EU where s.Ticket_no == sa.Ticket_no select s).Count();

            if (cnt > 0)
            {
                TempData["message"] = "Invoice allready Generated !! ";
            }
            else
            {
                try
                {
                    decimal amt_total = sa.Business_hour_amt + sa.Non_Business_hour_amt + sa.Travel_Charge + sa.Part_Handling_Charge + sa.Cancel_Charge + sa.Reschedule_Charge + sa.Fixed_amt;
                    Header_Invoice_EU hf = new Header_Invoice_EU();
                    hf.EU_ID = sa.EU_ID;
                    hf.Total_Amt = amt_total;
                    hf.Invoice_Date = DateTime.Now.Date;
                    hf.CreatedBy = User.Identity.GetUserId();
                    hf.CreatedOn = DateTime.Now;
                    db.Header_Invoice_EU.Add(hf);
                    int res = db.SaveChanges();
                    if (res > 0)
                    {

                        sa.Inv_no = hf.Id;
                        sa.CreatedBy = User.Identity.GetUserId();
                        sa.CreatedOn = DateTime.Now;
                        db.Header_Invoice_Detail_EU.Add(sa);
                        db.SaveChanges();


                    }

                    TempData["message"] = "EU Invoice Generated : Invoice No !!" + " " + hf.Id;
                    TempData["link"] = "Yes";
                    TempData["INV"] = hf.Id;
                }
                catch (Exception ex)
                {
                    TempData["message"] = "Invoice Not Generated !! ";
                }

            }

            return RedirectToAction("AllInvoiceEU", "Transaction");
        }
        public ActionResult PrintInvoiceEU(int InvoiceNo)
        {
            Invoice_EU_Print inv = new Invoice_EU_Print();
            inv = db.Database.SqlQuery<Invoice_EU_Print>("GetEUInvoicePrint @id", new SqlParameter("@id ", InvoiceNo)).FirstOrDefault();

            return View(inv);
        }

        public ActionResult AllInvoiceEU()
        {
            List<Invoice_EU_Print> inv = new List<Invoice_EU_Print>();
            try
            {
                inv = db.Database.SqlQuery<Invoice_EU_Print>("GetEUInvoiceAll").ToList();
                ViewBag.Payment_Mode = (from c in db.HeaderDescription where c.header_id == 10 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();

            }
            catch (Exception ex)
            {


            }

            return View(inv);
        }
        /*----------------------------------------------------------------------------------------------*/
        public ActionResult FEPayment(Header_Invoice_FE sa)
        {
            try
            {
                Header_Invoice_FE hi = new Header_Invoice_FE();
                hi = (from s in db.Header_Invoice_FE where s.Id == sa.Id select s).FirstOrDefault();
                hi.FE_PaymentMode = sa.FE_PaymentMode;
                hi.FE_Payment_Status = sa.FE_Payment_Status;
                hi.FE_Pay_Date = sa.FE_Pay_Date;
                hi.FE_Reference_ID = sa.FE_Reference_ID;
                hi.ModifiedBy = User.Identity.GetUserId();
                hi.ModifiedOn = DateTime.Now;
                db.Entry(hi).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["message"] = "Payment Status Updated!! ";


            }
            catch (Exception ex)
            {
                TempData["message"] = "Payment Status Not Updated!! ";
            }
            return RedirectToAction("AllInvoiceFE", "Transaction");
        }

        /*----------------------------------------------------------------------------------------------*/

        [AllowAnonymous]
        public ActionResult TicketSelection(int? Id)
        {
            try
            {
                if (Id != null)
                {
                    Ticket_FE_Selection sa = new Ticket_FE_Selection();
                    sa = (from s in db.Ticket_FE_Selection where s.Id == Id.Value select s).FirstOrDefault();
                    var fe = (from s in db.FE_Master_Personal where s.Id == sa.FE_ID select s).FirstOrDefault();
                    var tic = (from s in db.Ticket where s.Id == sa.Ticket_no select s).FirstOrDefault();
                    string oem = (from s in db.HeaderDescription where s.id == tic.OEM select s.header_description).FirstOrDefault();

                    ViewBag.Site_address = tic.Street_Address;
                    ViewBag.Datetime = tic.Dispatch_Date.Value.ToString("dd MMM yyy HH':'mm tt");
                    ViewBag.Scope = tic.Job_Description;
                    ViewBag.OEM = oem;
                    if (sa.Status == "Sent")
                    {
                        ViewBag.Message = "Yes";
                    }
                    return View(sa);
                }
                else
                {

                    return View();
                }

            }
            catch
            {
                return View();
            }

        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult TicketSelection(Ticket_FE_Selection sa)
        {

            try
            {
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var fe = (from s in db.FE_Master_Personal where s.Id == sa.FE_ID select s).FirstOrDefault();
                var tic = (from s in db.Ticket where s.Id == sa.Ticket_no select s).FirstOrDefault();
                string oem = (from s in db.HeaderDescription where s.id == tic.OEM select s.header_description).FirstOrDefault();
                string body = utlity.FESelectionStatus("Team", tic.Street_Address, tic.Dispatch_Date.Value.ToString("dd MMM yyy HH':'mm tt"), tic.Job_Description, oem, tic.Ticket_No, sa.Remark, sa.Status, fe.First_Name + " " + fe.Last_Name);

                if (sa.Status == "Accepted")
                {
                    if (fe.Status == 1)
                    {
                        TempData["message"] = "Thank you for accepting the opportunity.  Our support team will share further details to you post confirmation from client.!!";

                        utlity.sendmail("support@inwinteck.com", "Request for onsite/call out/intervention for Inwinteck Ticket No :" + tic.Ticket_No + " has been Accepted", body, mailer);

                    }
                    else
                    {
                        TempData["message"] = "Thank you for accepting the opportunity.<br/> To proceed further, you need to activate your account and confirm your charges for this job . <br/> You can also share your charges on our email id support@inwinteck.com. <br/> For activation of account,  kindly login to https://fms.inwinteck.com/ .<br/>Your email id  is the user id .  You have to reset the password and proceed.<br/> Our support team will reach out to you with further details. ";

                        utlity.sendmail("support@inwinteck.com", "Request for onsite/call out/intervention for Inwinteck Ticket No :" + tic.Ticket_No + " has been Accepted and FE Status is Deactive", body, mailer);

                    }
                }
                else if (sa.Status == "Rejected")
                {

                    if (fe.Status == 1)
                    {
                        TempData["message"] = "Thank you for your prompt response. Our support team will get in touch with you for new business opportunity in the future.!!";

                        utlity.sendmail("support@inwinteck.com", "Request for onsite/call out/intervention for Inwinteck Ticket No :" + tic.Ticket_No + " has been rejected", body, mailer);
                    }
                    else
                    {
                        TempData["message"] = "Dear Partner,<br/> We appreciate your time and response. Unfortunately, you have denied this Onsite request for the reason mentioned by you .<br/>  Our support team will get in touch with you for new business opportunity. <br/> We request you to kindly activate your account by completing the profile so that we can  reach out to you again.<br/> Thank you and appreciate it.";

                        utlity.sendmail("support@inwinteck.com", "Request for onsite/call out/intervention for Inwinteck Ticket No :" + tic.Ticket_No + " has been rejected", body, mailer);
                    }

                }
            }
            catch
            {
                TempData["message"] = "Status Not updated !!";
            }


            return View();
        }

        [AllowAnonymous]
        //[Authorize(Roles = "Source Support")]

        public ActionResult TicketEUSelection(int? Id)
        {
            try
            {
                if (Id != null)
                {
                    Ticket_Eu_Selection sa = new Ticket_Eu_Selection();
                    sa = (from s in db.Ticket_Eu_Selection where s.Id == Id.Value select s).FirstOrDefault();
                    var fe = db.FE_Master_Personal.Where(x => x.Id == sa.Fe_Id).FirstOrDefault();
                    var eu = (from s in db.EU_Master where s.Id == sa.Eu_ID select s).FirstOrDefault();
                    var ticket = (from s in db.Ticket where s.Id == sa.Ticket_no select s).FirstOrDefault();
                    if (sa.Status == "Sent")
                    {
                        ViewBag.Message = "Yes";
                    }
                    return View(sa);
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Authorize(Roles = "Source Support")]
        public ActionResult TicketEUSelection(Ticket_Eu_Selection sa)
        {
            try
            {
                var existingRecord = db.Ticket_Eu_Selection.Find(sa.Id);

                if (existingRecord != null)
                {
                    // Update existing record
                    existingRecord.Status = sa.Status;
                    existingRecord.Remark = sa.Remark;
                    existingRecord.ModifiedOn = DateTime.Now;
                    existingRecord.ModifiedBy = User.Identity.GetUserId();

                    db.Entry(existingRecord).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    // Add new record
                    sa.CreatedOn = sa.CreatedOn ?? DateTime.Now; // Set to current time if null
                    sa.CreatedBy = sa.CreatedBy ?? "System"; // Set to default if null
                    sa.ModifiedOn = DateTime.Now;

                    db.Ticket_Eu_Selection.Add(sa);
                }

                db.SaveChanges();
                TempData["message"] = "Status updated successfully!";

                //calling SignalR method from SourceStatusAboutFE Hub
                SourceStatusAboutFE.SendStatusUpdate(sa);
            }
            catch (Exception ex)
            {
                TempData["message"] = "Status not updated!" + ex.Message;
            }
            return View();
        }


        [AllowAnonymous]
        public ActionResult CSAT(int? Id)
        {
            try
            {
                if (Id != null)
                {
                    int cnt = (from s in db.CSAT where s.TicketNo == Id select s).Count();

                    if (cnt > 0)
                    {
                        ViewBag.cnt = 1;
                    }
                    ViewBag.Ticket = Id;
                    ViewBag.caseno = (from s in db.Ticket where s.Id == Id select s.Case_No).FirstOrDefault();
                    ViewBag.inwinteck = (from s in db.Ticket where s.Id == Id select s.Ticket_No).FirstOrDefault();
                }
                else
                {
                    ViewBag.cnt = 1;
                }

            }
            catch
            {

            }

            return View();

        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CSAT(CSAT sa)
        {
            int cnt = (from s in db.CSAT where s.TicketNo == sa.TicketNo select s).Count();

            try
            {
                if (cnt > 0)
                {
                    ViewBag.cnt = 1;
                    ViewBag.Ticket = sa.TicketNo;
                }
                else
                {
                    sa.CreatedOn = DateTime.Now;
                    db.CSAT.Add(sa);
                    db.SaveChanges();

                    int cs = sa.Q1 + sa.Q2 + sa.Q3 + sa.Q4 + sa.Q5;
                    decimal avg = Convert.ToDecimal(cs / 5);
                    TempData["CsatRating"] = avg;
                    TempData["message"] = "Dear Customer, we appreciate your valuable feedback . Thank you.";


                    //if (sa.Q3 == 1 || sa.Q3 == 2)
                    //{
                    //    string sub = "Dissatisfied customer (Feedback Rating 1 / 2)";
                    //    var tkt = (from a in db.Ticket where a.id == sa.TicketNo select a).FirstOrDefault();
                    //    var customer = (from s in db.Customer where s.id == tkt.Customer_ID select s).FirstOrDefault();
                    //    var cp = (from s in db.CollectionPoint where s.id == tkt.CP_ID select s).FirstOrDefault();
                    //    string branch = (from s in db.HeaderDescriptions where s.id.ToString() == cp.cp_branchcode select s.header_description).FirstOrDefault();

                    //    string body = mailutility.CSAT(customer.cust_name, customer.cust_cont, sa.TicketNo.ToString(), cp.cp_shop, branch, sa.Q3.ToString(), DateTime.Now.ToString("dd-MM-yyyy"), sa.Remarks);
                    //    string to = "feedback@vipbags.com, bhavisha.saraiya@jetairservices.in, aadesh.kumar@jetairservices.in, anoop.gopinathan@vipbags.com, neeraj.rajwal@vipbags.com, sonali.desai@vipbags.com, anjan.sengupta@vipbags.com,serviceteamleaders@vipbags.com";

                    //    string From = "Customer Feedback <smtp@vipbags.com>";
                    //    string ress = mailutility.sendmail_csat(to, sub, body, From);
                    //}
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Not updated !!";
            }


            return RedirectToAction("CSAT", new { Id = sa.TicketNo });
        }

        [HttpGet]
        public JsonResult GetContact(int Cnt)
        {
            string res = "";
            List<EU_Master_Contacts> CC = new List<EU_Master_Contacts>();
            CC = (from c in db.EU_Master_Contacts where c.Office_ID == Cnt select c).ToList();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetOffice(int Cnt)
        {
            string res = "";
            List<EU_Master_Branch> CC = new List<EU_Master_Branch>();
            CC = (from c in db.EU_Master_Branch where c.EU_ID == Cnt select c).ToList();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult getFEdetails(string PinCode, string Country, int ticket)
        {
            string res = "";
            List<FEDetails> CC = new List<FEDetails>();

            CC = db.Database.SqlQuery<FEDetails>("getFEdetails @id,@cnt", new SqlParameter("@id", PinCode), new SqlParameter("@cnt", Country)).ToList();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult getFEInfo(int id)
        {
            FEDetails CC = db.Database.SqlQuery<FEDetails>("getFEdetails @id", new SqlParameter("@id", id)).FirstOrDefault();
            if (CC != null)
            {
                return Json(new { res = "Success", CC.Certifications, CC }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { res = "Failed" }, JsonRequestBehavior.AllowGet);
        }





        [HttpGet]
        public JsonResult GetTicketHistory(int tn)
        {
            string tv = "";
            List<TicketHist> hist = new List<TicketHist>();
            hist = db.Database.SqlQuery<TicketHist>("gethistory @id", new SqlParameter("@id ", tn)).ToList();

            if (hist != null)
            {
                tv = "success";
            }
            else
            {
                tv = "failure";
            }
            return Json(new { tv, hist }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUserNameOfTicketHandlers(int tn)
        {
            List<TicketHist> hist = db.Database.SqlQuery<TicketHist>("gethistory @id", new SqlParameter("@id", tn)).ToList();

            // Extract distinct 'CreatedBy' values
            var distinctCreatedBy = hist.Select(h => h.CreatedBy).Distinct().ToList();

            // Check if there are any distinct values
            string tv = distinctCreatedBy.Any() ? "success" : "failure";

            // Return the distinct 'CreatedBy' values and the status
            return Json(new { tv, distinctCreatedBy }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetLocation(string PinCode)
        {
            string res = "";
            string pzm = "";
            int cnt;
            PinCodeMaster Location = new PinCodeMaster();
            List<PinCodeMaster> PZ = new List<PinCodeMaster>();
            Location = (from c in db.PinCode where c.pincode == PinCode && c.status == 1 select c).FirstOrDefault();
            cnt = (from s in db.PinCode where s.pincode == PinCode && s.status == 1 select s).Count();
            if (Location != null)
            {
                res = "Success";
                if (cnt > 1)
                {
                    PZ = (from s in db.PinCode where s.pincode == PinCode && s.status == 1 select s).ToList();
                    pzm = "List";
                }
            }
            return Json(new { res, Location, PZ, pzm }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetLocationCity(string City)
        {
            string res = "";
            string pzm = "";
            int cnt;
            PinCodeMaster Location = new PinCodeMaster();
            List<PinCodeMaster> PZ = new List<PinCodeMaster>();
            Location = (from c in db.PinCode where c.city == City && c.status == 1 select c).FirstOrDefault();
            cnt = (from s in db.PinCode where s.city == City && s.status == 1 select s.Country).Distinct().Count();
            if (Location != null)
            {
                res = "Success";
                if (cnt > 1)
                {
                    PZ = (from s in db.PinCode where s.city == City && s.status == 1 select s).Distinct().ToList();
                    pzm = "List";
                }
            }
            return Json(new { res, Location, PZ, pzm }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult getFEdetailsMap(int id)
        {
            // db.Database.CommandTimeout = 180;
            //string res = "";
            //List<FEDetails> CC = new List<FEDetails>();
            //List<FEDetailsmap> FEM = new List<FEDetailsmap>();

            //CC = db.Database.SqlQuery<FEDetails>("getFEdetailsmap").ToList();
            //FEM = db.Database.SqlQuery<FEDetailsmap>("getFEdetailsmap").ToList();
            //if (CC != null)
            //{
            //    res = "Success";
            //}

            string username = User.Identity.GetUserName();
            List<FEDetails> data = new List<FEDetails>();



            data = db.Database.SqlQuery<FEDetails>("getFEdetailsmap @id", new SqlParameter("@id", id)).ToList();



            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ContactEUMaster(EU_Master_Contacts sa, int Contact_ID)
        {
            if (Contact_ID != 0)
            {
                try
                {
                    sa.Id = Contact_ID;
                    sa.CreatedBy = (from s in db.EU_Master_Contacts where s.Id == Contact_ID select s.CreatedBy).FirstOrDefault();
                    sa.CreatedOn = (from s in db.EU_Master_Contacts where s.Id == Contact_ID select s.CreatedOn).FirstOrDefault();
                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    TempData["message"] = "EU Master's Contact Updated !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "EU Master's Contact Not Updated !! ";
                }
            }
            else
            {
                try
                {
                    sa.CreatedBy = User.Identity.GetUserId();
                    sa.CreatedOn = DateTime.Now;
                    db.EU_Master_Contacts.Add(sa);
                    db.SaveChanges();


                    TempData["message"] = "EU Master's Contact Added !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "EU Master's Contact Not Created !! ";
                }
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult deleteContact(int OC)
        {
            int od = (from s in db.EU_Master_Contacts where s.Id == OC select s.Office_ID).FirstOrDefault();
            int ED = (from s in db.EU_Master_Branch where s.Id == od select s.EU_ID).FirstOrDefault();
            if (OC > 0)
            {

                EU_Master_Contacts sa = new EU_Master_Contacts();
                sa = (from c in db.EU_Master_Contacts where c.Id == OC select c).FirstOrDefault();
                if (sa != null)
                {
                    db.Entry(sa).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }

            }
            else
            {
                TempData["message"] = "Empty Data !! ";
            }


            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getFEMail(int id, int Ticket)
        {
            string res = "";
            string fename = "";
            Ticket_FE_Selection TF = new Ticket_FE_Selection();

            TF.FE_ID = id;
            TF.Ticket_no = Ticket;
            TF.Status = "Sent";
            TF.CreatedBy = User.Identity.GetUserId();
            TF.CreatedOn = DateTime.Now;
            db.Ticket_FE_Selection.Add(TF);
            int cc = db.SaveChanges();
            if (cc > 0)
            {
                res = "Success";
                var fe = (from s in db.FE_Master_Personal where s.Id == TF.FE_ID select s).FirstOrDefault();
                fename = fe.First_Name + " " + fe.Last_Name;
                var tic = (from s in db.Ticket where s.Id == Ticket select s).FirstOrDefault();
                string oem = (from s in db.HeaderDescription where s.id == tic.OEM select s.header_description).FirstOrDefault();
                var callbackUrl = Url.Action("TicketSelection", "Transaction", new { Id = TF.Id }, protocol: Request.Url.Scheme);
                string body = utlity.FESelection(fe.First_Name + " " + fe.Last_Name, tic.Street_Address, tic.Dispatch_Date.Value.ToString("dd MMM yyy HH':'mm tt"), tic.Job_Description, oem, callbackUrl, tic.Ticket_No);
                utlity.sendmail(fe.Email, "Request for onsite/call out/intervention for Inwinteck Ticket No :" + tic.Ticket_No, body, mailer);
            }

            return Json(new { res, fename }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFETicketStatus(int Ticket)
        {
            string res = "";
            List<FETicket> PZ = new List<FETicket>();
            PZ = (from s in db.Ticket_FE_Selection
                  join a in db.FE_Master_Personal on s.FE_ID equals a.Id
                  where s.Ticket_no == Ticket
                  select new FETicket { FE_ID = a.Id, Name = a.First_Name + " " + a.Last_Name, Status = s.Status, Sent = s.CreatedOn.ToString(), Remark = s.Remark, Request = s.ModifiedOn.ToString() }).ToList();
            res = "Success";
            return Json(new { res, PZ }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetEUStatusAboutFE(int ticketId)
        {
            try
            {
                var ticketNoParam = new SqlParameter("@ticketNo", ticketId);
                // Call the stored procedure
                var result = db.Database.SqlQuery<EuStatusDto>(
                    "EXEC GetEuStatusAboutFe @ticketNo",
                    ticketNoParam
                ).FirstOrDefault();



                //if (result != null)
                //{
                //    var ticket = db.Ticket.FirstOrDefault(x => x.Id == ticketId);
                //    if (ticket != null)
                //    {
                //        if (result.Status == "Accepted")
                //        {
                //            ticket.Status = 1372; // Status for "Accepted"
                //        }

                //        db.SaveChanges();
                //    }
                //}

                // Return the result as JSON
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpPost]
        public JsonResult PartSaveData(string[] empdata)
        {
            List<Part_Ticket_Data> LPTD = new List<Part_Ticket_Data>();

            string res = "";
            Part_Ticket_Data PTD = new Part_Ticket_Data();

            //Loop and insert records.
            foreach (Part_Ticket_Data iv in LPTD)
            {
                PTD.Ticket_No = iv.Ticket_No;
                PTD.Make_Model = iv.Make_Model;
                PTD.Part_type = iv.Part_type;
                PTD.Serial_No = iv.Serial_No;
                PTD.Part_Description = iv.Part_Description;
                PTD.CreatedBy = User.Identity.GetUserId();
                PTD.CreatedOn = DateTime.Now;
                db.Part_Ticket_Data.Add(PTD);
                db.SaveChanges();
            }

            res = "Success";

            return Json(new { res }, JsonRequestBehavior.AllowGet);
        }

    }
}