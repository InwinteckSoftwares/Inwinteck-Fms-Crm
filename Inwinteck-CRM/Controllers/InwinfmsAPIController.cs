using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Inwinteck_CRM.Models;
using System.IO;
using System.Web.Http.Cors;
using System.Web.Hosting;
using System.Web;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Xml.Linq;
using Inwinteck_CRM.ApiAuthorization;
using System.Threading;
using System.Data.Entity;
using Inwinteck_CRM.Hubs;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Inwinteck_CRM.Controllers
{

    [RoutePrefix("api/InwinfmsAPI")]
    public class InwinfmsAPIController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();

     //   string url = "https://fms.inwinteck.com/Upload/";
        string mailer = "Support<support@inwinteck.com>";

        //ticket.Site_Name,ticket.Street_Address,ticket.Country,ticket.State, ticket.Zip_Pin_Code, "AIzaSyBvhQbrULBXqk9wPb9RjTeacVk3A3g-ci8"
        private async Task<Ticket> GetCoordinatesAsync(string Site_Name, string streetAddress, string country,string State, string city, string zipCode, string apiKey)
        {
            // Construct the address by including the Site_Name.
            string address = $"{Site_Name}, {streetAddress}, {city}, {country}";

            // Create the API request URI, prioritizing the Site_Name, street address, and city.
            string requestUri = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address + " " + zipCode)}&key={apiKey}";

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(content);

                    if (json["status"].ToString() == "OK")
                    {
                        var location = json["results"][0]["geometry"]["location"];
                        return new Ticket
                        {
                            latitude = location["lat"].ToString(),
                            longitude = location["lng"].ToString()
                        };
                    }
                }
            }
            return null;
        }


        [HttpPost]
        [InwinAuthentication]
        public async Task<HttpResponseMessage> CreateTicket(apiTicket ticket)
        {
            string userName = Thread.CurrentPrincipal.Identity.Name;
            generalresponse gr = new generalresponse();


            if (string.IsNullOrEmpty(userName))
            {
                gr.statusmessage = "User is not authenticated.";
                gr.statuscode = 400;
                return Request.CreateResponse(HttpStatusCode.Unauthorized, gr);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                Ticket_Stagging log = new Ticket_Stagging();
                log.Email_Subject = ticket.Email_Subject;
                log.Case_No = ticket.Case_No;
                log.EU_Name = ticket.EU_Name;
                log.Street_Address = ticket.Street_Address;
                log.City = ticket.City;
                log.Status = "Added";
                log.Country = ticket.Country;
                log.Zip_Pin_Code = ticket.Zip_Pin_Code;
                log.CreatedOn = DateTime.Now;
                log.Dispatch_Date = ticket.Dispatch_Date;
                log.EU_Contact = ticket.EU_Contact;
                log.Job_Description = ticket.Job_Description;
                log.OEM = ticket.OEM;
                log.Site_Name = ticket.Site_Name;
                log.State = ticket.State;
                log.Type = "CreateTicket";
                log.EU_Email = ticket.EU_Email;
                log.Certification_Name = ticket.Certification_Name;
                log.TSE_Name = ticket.TSE_Name;

                try
                {
                    db.Ticket_Stagging.Add(log);
                    int res = db.SaveChanges();
                    Ticket_Stagging ts = new Ticket_Stagging();
                    ts = (from s in db.Ticket_Stagging where s.ID == log.ID select s).FirstOrDefault();

                    if (res > 0)
                    {
                        Ticket sa = new Ticket();

                        sa.Case_No = ticket.Case_No;
                        sa.EU_Name = ticket.EU_Name;
                        sa.Street_Address = ticket.Street_Address;
                        sa.City = ticket.City;
                        sa.EU_Email = ticket.EU_Email;
                        sa.Country = ticket.Country;
                        sa.Zip_Pin_Code = ticket.Zip_Pin_Code;
                        sa.CreatedOn = DateTime.Now;
                        sa.CreatedBy = "etc@sourcesupport.com";
                        sa.Ticket_Date = DateTime.Now;
                        sa.Dispatch_Date = ticket.Dispatch_Date;
                        sa.EU_Contact = ticket.EU_Contact;
                        sa.Job_Description = ticket.Job_Description;
                        
                        sa.OEM = (from s in db.HeaderDescription where s.header_id == 4 && s.header_description.Contains(ticket.Certification_Name) select s.id).FirstOrDefault();
                        sa.Site_Name = ticket.Site_Name;
                        sa.State = ticket.State;
                        sa.Ticket_No = utlity.CheckTicketNo(4);
                        sa.Status = 18;
                        sa.TSE_Name = ticket.TSE_Name;
                        sa.Scope_Off_Work = log.OEM;

                        if (sa.OEM == 0)    
                        {
                            // Add the new OEM value to the HeaderDescription table
                            var newHeader = new HeaderDescription
                            {
                                header_id = 4,
                                header_description = ticket.OEM,
                                Status =1 ,
                                CreatedBy = User.Identity.GetUserId(),
                                CreatedOn= DateTime.Now

                            };
                            db.HeaderDescription.Add(newHeader);
                            db.SaveChanges();

                            // Retrieve the new OEM id
                            sa.OEM = (from s in db.HeaderDescription where s.header_id == 4 && s.header_description.Contains(ticket.OEM) select s.id).FirstOrDefault();
                        }
                        // Fetch latitude and longitude
                        try
                        {
                            var coordinates = await GetCoordinatesAsync(ticket.Site_Name,ticket.Street_Address,ticket.Country,ticket.State,ticket.City ,ticket.Zip_Pin_Code, "AIzaSyBvhQbrULBXqk9wPb9RjTeacVk3A3g-ci8");
                            if (coordinates != null)
                            {
                                sa.latitude = coordinates.latitude;
                                sa.longitude = coordinates.longitude;
                            }
                            else
                            {
                                sa.latitude = "0";
                                sa.longitude = "0";
                            }
                        }
                        catch (Exception geoEx)
                        {
                            sa.latitude = "0";
                            sa.longitude = "0";
                        }

                        sa.Certification_Need = 1;
                        //List<int> oemCertificates = db.Certification_Master
                        //         .Where(cm => cm.OEM == sa.OEM)
                        //         .Select(cm => cm.Id)  
                        //         .ToList();
                        //sa.Certification_Name = string.Join(", ", oemCertificates);

                        // Create a dictionary to store Certification_Name and their corresponding certificate IDs
                        var certificateMap = new Dictionary<string, int[]>
                        {
                            { "HPE-MAIN", new int[] { 3, 4, 1048, 1058 } },
                            { "INFINIDAT-MAIN", new int[] { 9, 30, 1049, 1050, 1057 } },
                            { "Optos-MAIN", new int[] { 1059 } },
                            { "Quantum-MAIN", new int[] { 1044,1051,1052,1053 } }

                        };

                        // Check if the Certification_Name exists in the dictionary
                        if (certificateMap.TryGetValue(log.Certification_Name, out int[] certificateIds))
                        {
                            sa.Certification_Name = string.Join(", ", certificateIds);
                        }
                        else
                        {
                            sa.Certification_Name = string.Empty; // Or any default value
                        }


                        sa.EU_ID = 4;
                        //sa.CreatedBy = "27b3bb6b-9331-4470-8026-d02e9125bab4";
                        //sa.CreatedOn = DateTime.Now;
                        sa.pregame_detail = @"1. Please reach the site on time Important.
                                               2. Please follow the formal dress Code.
                                               3. Make sure you carry all desired tools onsite as requested.
                                                4. Phone should be completely charged.
                                                5. Engineer has to share the system picture with the serial number before commencing the work.
                                                6. We need to verify the serial number of the device prior to starting any repairs and we need to get the customer's approval before starting any repairs.
                                                7. Be polite, patient and do not use any profanity language while onsite.
                                                8. If any issues or any doubt, please call.
                                                9. If the parts are damaged or any fault in the system made by the end customer, you should inform us immediately or before leaving the site.
                                                10. If any issue related to payment, extra hours, pricing, or any other issues related to payments please do not communicate or discuss with the client, do not mention on WhatsApp group.
                                                11. Once activity is completed, make sure the site is left neat & clean always. Support Team needs to confirm the repair with the customer prior to your release.
                                                12. If any faulty part needs to be shipped, please write a mail to parts@inwinteck.com with all details such as Serial No and Return Label details with the address of location from where parts to be picked up.
                                                13. Following are the pregame points for Supermicro:
                                                • Make sure you carry all desired tools, the torque limiting screwdriver with Bits T20, T25, T30, and thermal paste.
                                                • Once the task is completed, please pack the faulty parts, label them, and share the picture in the group. And do not carry the part.";
                        sa.EmailMessageId = Guid.NewGuid().ToString();
                        db.Ticket.Add(sa);
                        int ress = db.SaveChanges();

                        // Trigger the SignalR notification after the ticket is successfully created
                        TicketNotificationHub.NotifyNewTicket(sa);

                        if (ress > 0)
                        {
                            if (sa.Ticket_Date.HasValue && sa.Dispatch_Date.HasValue)
                            {
                                var caseDt = await Universal.CommonMethod.ConvertIstToLocalAndUtcAsync(sa.latitude, sa.longitude, sa.Ticket_Date.Value);
                                var dispDt = await Universal.CommonMethod.ConvertLocalToIstAndUtcAsync(sa.latitude, sa.longitude, sa.Dispatch_Date.Value);
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
                                db.SaveChanges(); // Save the timeZoneRecord to get its Id

                                sa.TimeZoneId = timeZoneRecord.Id;

                                var ticketSLA = Universal.CommonMethod.CalculateSLA(timeZoneRecord.DispDtLocal, timeZoneRecord.CaseDtLocal);
                                sa.SLA = ticketSLA;
                                db.Entry(sa).State = EntityState.Modified;
                                //db.SaveChanges();
                            }
                            Ticket_History th = new Ticket_History();
                            th.Ticket_no = sa.Id;
                            th.Comments = "Ticket Created from source support api";
                            th.FE_ID = sa.FE_ID;
                            th.status = 18;
                            th.CreatedBy = "SourceSupportAPI";
                            th.CreatedOn = DateTime.Now;
                            db.Ticket_History.Add(th);
                            db.SaveChanges();

                            if (ts != null)
                            {
                                ts.Status = "Success";
                                ts.Ticket_No = sa.Ticket_No;
                                ts.Error_Msg = "NA";
                                db.Entry(ts).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }

                            Ticket_Email TE = new Ticket_Email();
                            TE.Ticket_no = sa.Id;
                            TE.Email = sa.EU_Email;
                            TE.Email_Subject = string.IsNullOrEmpty(ticket.Email_Subject) ? "Ticket Created :: Ticket Number : " + sa.Ticket_No : ticket.Email_Subject;
                            TE.CreatedBy = User.Identity.GetUserId();
                            TE.CreatedOn = DateTime.Now;
                            db.Ticket_Email.Add(TE);
                            db.SaveChanges();

                            Ticket_EU_Detail TSEU = new Ticket_EU_Detail();
                            TSEU.Ticket_no = sa.Id;
                            TSEU.EU_Name = sa.EU_Name;
                            TSEU.EU_Email = sa.EU_Email;
                            TSEU.EU_Contact = sa.EU_Contact;
                            TSEU.CreatedBy = sa.CreatedBy;
                            TSEU.CreatedOn = DateTime.Now;
                            db.Ticket_EU_Detail.Add(TSEU);
                            db.SaveChanges();

                            string eu = (from s in db.EU_Master where s.Id == sa.EU_ID select s.Customer_Name).FirstOrDefault();

                            if(sa.Case_No != "000000")
                            {
                                string body = utlity.TicketGenerated(eu, sa.Ticket_No, sa.Case_No, sa.Site_Name, sa.Street_Address, sa.Dispatch_Date.ToString(), sa.Job_Description);
                                utlity.sendmail2("etc@sourcesupport.com", TE.Email_Subject, body, mailer, sa.EmailMessageId, false);
                                string body1 = utlity.TicketApiDetails(sa.Ticket_No, ticket.Email_Subject, ticket.Case_No, ticket.OEM, sa.TSE_Name, ticket.Site_Name, ticket.Street_Address, ticket.City, ticket.Zip_Pin_Code, ticket.Country, ticket.State, ticket.Dispatch_Date.ToString(), ticket.Job_Description, ticket.EU_Name, ticket.EU_Email, ticket.EU_Contact);
                                utlity.sendmail("hd@inwinteck.com,sk@inwinteck.com", TE.Email_Subject, body1, mailer);
                            }
                            
                            gr.statuscode = 200;
                            gr.statusmessage = "Ticket Created : Number :" + sa.Ticket_No;
                          
                        }
                    }
                }
                catch (Exception ex)
                {
                    gr.statuscode = 201;
                    gr.statusmessage = "Ticket Not Created !! " + ex.Message;

                    if (log != null)
                    {
                        log.Status = "Error";
                        log.Error_Msg = ex.Message;
                        db.Entry(log).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    return Request.CreateResponse(HttpStatusCode.InternalServerError, gr);
                }
            }
            else
            {
                gr.statuscode = 401;
                gr.statusmessage = "Unauthorised Access.";
                return Request.CreateResponse(HttpStatusCode.Unauthorized, gr);
            }

            return Request.CreateResponse(HttpStatusCode.OK, gr);
        }
      


        [HttpPost]
        [InwinAuthentication]
        public HttpResponseMessage UpdateTicket(apiTicketUpdate ticket)
        {
            string userName = Thread.CurrentPrincipal.Identity.Name;

            generalresponse gr = new generalresponse();


            if (userName != "")
            {

                Ticket_Stagging log = new Ticket_Stagging();
                log.Case_No = ticket.Case_No;
                log.EU_Name = ticket.EU_Name;
                log.Street_Address = ticket.Street_Address;
                log.City = ticket.City;
                log.Status = "Added";
                log.Country = ticket.Country;
                log.Zip_Pin_Code = ticket.Zip_Pin_Code;
                log.CreatedOn = DateTime.Now;
                log.Dispatch_Date = ticket.Dispatch_Date;
                log.EU_Contact = ticket.EU_Contact;
                log.Job_Description = ticket.Job_Description;
                log.OEM = ticket.OEM;
                log.Site_Name = ticket.Site_Name;
                log.State = ticket.State;
                log.Type = "UpdateTicket";
                log.EU_Email = ticket.EU_Email;
                log.Ticket_No = ticket.Ticket_No;
                log.Certification_Name = ticket.Certification_Name;

                db.Ticket_Stagging.Add(log);
                int res = db.SaveChanges();


                Ticket_Stagging ts = new Ticket_Stagging();
                ts = (from s in db.Ticket_Stagging where s.ID == log.ID select s).FirstOrDefault();
                Ticket sa = new Ticket();
                sa = (from s in db.Ticket where s.Ticket_No == log.Ticket_No select s).FirstOrDefault();
                try
                {

                    if (res > 0)
                    {

                        sa.Case_No = ticket.Case_No;
                        sa.EU_Name = ticket.EU_Name;
                        sa.Street_Address = ticket.Street_Address;
                        sa.City = ticket.City;
                        sa.EU_Email = ticket.EU_Email;
                        sa.Country = ticket.Country;
                        sa.Zip_Pin_Code = ticket.Zip_Pin_Code;
                        sa.CreatedOn = DateTime.Now;
                        sa.Dispatch_Date = ticket.Dispatch_Date;
                        sa.EU_Contact = ticket.EU_Contact;
                        sa.Job_Description = ticket.Job_Description;
                        sa.OEM = (from s in db.HeaderDescription where s.header_id == 4 && s.header_description.Contains(ticket.OEM) select s.id).FirstOrDefault();
                        sa.Site_Name = ticket.Site_Name;
                        sa.State = ticket.State;
                        sa.ModifiedBy = "27b3bb6b-9331-4470-8026-d02e9125bab4";
                        sa.ModifiedOn = DateTime.Now;
                        db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                        int ress = db.SaveChanges();
                        if (ress > 0)
                        {
                            Ticket_History th = new Ticket_History();
                            th.Ticket_no = sa.Id;
                            th.Comments = ticket.Comments;
                            th.FE_ID = sa.FE_ID;
                            th.status = sa.Status;
                            th.CreatedBy = User.Identity.GetUserId();
                            th.CreatedOn = DateTime.Now;
                            db.Ticket_History.Add(th);
                            db.SaveChanges();

                            if (ts != null)
                            {
                                ts.Status = "Sucess";
                                ts.Ticket_No = sa.Ticket_No;
                                ts.Error_Msg = "NA";
                                db.Entry(ts).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }

                        }

                    }

                    gr.statuscode = 200;
                    gr.statusmessage = "Ticket Updated : Number :" + sa.Ticket_No;

                }
                catch (Exception ex)
                {
                    gr.statuscode = 201;
                    gr.statusmessage = "Ticket Not Updated !! " + ex.InnerException.ToString();

                    if (ts != null)
                    {
                        ts.Status = "Error";
                        ts.Error_Msg = ex.InnerException.ToString();
                        db.Entry(ts).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                gr.statuscode = 401;
                gr.statusmessage = "Unauthorised Access.";

                return Request.CreateResponse(HttpStatusCode.Unauthorized, gr);

            }

            return Request.CreateResponse(HttpStatusCode.OK, gr);
        }

        //=============For Notification Feature belows method is created=====================//

        [HttpGet]
        [Route("tickets")]  
        public IHttpActionResult GetTickets()
        {
            List<TicketDetails> li = db.Database.SqlQuery<TicketDetails>("getTicketDetails").ToList();
            return Ok(li);
        }

    }
}