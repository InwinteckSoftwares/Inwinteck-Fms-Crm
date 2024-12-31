using System;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using Inwinteck_CRM.Models;
using System.Data;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Xml.Linq;

namespace Inwinteck_CRM.Models
{

    static class utlity
    {


        public static string[] GetdtLatLong(string url)
        {
            string[] latlng = new string[2];

            try
            {

                WebRequest request = WebRequest.Create(url);

                WebResponse response = request.GetResponse();
                XDocument xdoc = XDocument.Load(response.GetResponseStream());

                XElement result = xdoc.Element("GeocodeResponse").Element("result");
                XElement locationElement = result.Element("geometry").Element("location");
                XElement lat = locationElement.Element("lat");
                XElement lng = locationElement.Element("lng");



                latlng[0] = lat.ToString().Replace("<lat>", "").Replace("</lat>", "");
                latlng[1] = lng.ToString().Replace("<lng>", "").Replace("</lng>", "");

                return latlng;
            }
            catch (Exception ex)
            {

                latlng[0] = "None";
                latlng[1] = "None";
                string msg = "/BlukUpload/FEMaster/log/Status: Error In Address";
                utlity.createlog(msg);
                return latlng;

            }



        }

        public static void createlog(string message)
        {
            try
            {


                String filename = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year;
                filename += ".txt";

                using (StreamWriter sw = new StreamWriter(@"C:\Inwinteck\Log\" + filename, true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + " : " + message);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static string ResetPassword(string Name, string code)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/ResetPassword.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);

            body = body.Replace("{code}", code);

            return body;
        }

        public static string ChangePassword(string Name)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/PasswordChange.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);

            return body;
        }

        public static string WelcomeAbord(string Name, string Type, string City, string Country)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/Welcome.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);
            body = body.Replace("{Type}", Type);
            body = body.Replace("{City}", City);
            body = body.Replace("{Country}", Country);

            return body;
        }

        public static string Changelog(string Name, string changes)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/ChangeLog.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);
            body = body.Replace("{changes}", changes);

            return body;
        }
        public static string ChangePasswordS(string Name, string user)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/PasswordChangeS.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);

            body = body.Replace("{user}", user);

            return body;
        }
        
        public static string WelcomeFE(string Name, string Password, string username, string Type, string City, string Country)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/Register.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);

            body = body.Replace("{password}", Password);

            body = body.Replace("{username}", username);
            body = body.Replace("{Type}", Type);
            body = body.Replace("{City}", City);
            body = body.Replace("{Country}", Country);

            return body;
        }

        public static string TicketGenerated(string Name, string Ticket_no, string Case_no, string Site_Name, string Site_Address, string Dispatch_Date, string Job_Description)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/TicketGenerated.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);

            body = body.Replace("{Ticket_no}", Ticket_no);

            body = body.Replace("{Case_no}", Case_no);

            body = body.Replace("{Site_Name}", Site_Name);

            body = body.Replace("{Site_Address}", Site_Address);

            body = body.Replace("{Dispatch_Date}", Dispatch_Date);

            body = body.Replace("{Job_Description}", Job_Description);

            return body;
        }

        public static string WebEnquiry(string comp, string name, string desig, string email, string country, string state, string location,string mobile,string requirement,string msg)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/WebEnquiry.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{comp}", comp);

            body = body.Replace("{name}", name);

            body = body.Replace("{desig}", desig);

            body = body.Replace("{email}", email);

            body = body.Replace("{country}", country);

            body = body.Replace("{state}", state);

            body = body.Replace("{location}", location);
            body = body.Replace("{mobile}", mobile);
            body = body.Replace("{requirement}", requirement);
            body = body.Replace("{msg}", msg);

            return body;
        }

        public static string WebCareer(string name, string emailid, string mobile, string msg)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/WebCareer.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", name);

            body = body.Replace("{emailid}", emailid);

            body = body.Replace("{mobile}", mobile);

            body = body.Replace("{msg}", msg);

            return body;
        }

        public static string FESelection(string Name, string Site_Address, string Date_Time, string Scope_Work, string OEM, string link, string ticketno)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/FEselection.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);

            body = body.Replace("{Date_Time}", Date_Time);

            body = body.Replace("{Scope_Work}", Scope_Work);

            body = body.Replace("{OEM}", OEM);

            body = body.Replace("{Site_Address}", Site_Address);

            body = body.Replace("{link}", link);

            body = body.Replace("{ticketno}", ticketno);
            return body;
        }

        public static string FESelectionStatus(string Name, string Site_Address, string Date_Time, string Scope_Work, string OEM, string ticketno, string Remark, string Status, string FE)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/FEselectionStatus.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);

            body = body.Replace("{Date_Time}", Date_Time);

            body = body.Replace("{Scope_Work}", Scope_Work);

            body = body.Replace("{OEM}", OEM);

            body = body.Replace("{Site_Address}", Site_Address);

            body = body.Replace("{ticketno}", ticketno);

            body = body.Replace("{Remark}", Remark);

            body = body.Replace("{Status}", Status);

            body = body.Replace("{FE}", FE);

            return body;
        }

        public static string CustomerSchedule(string Name, string Case_no, string FE, string Email, string Contact, string Link)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/CustomerSchedule.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);
            body = body.Replace("{Case_no}", Case_no);
            body = body.Replace("{FE}", FE);
            body = body.Replace("{Email}", Email);
            body = body.Replace("{Contact}", Contact);

            body = body.Replace("{Link}", Link);

            return body;
        }

      
        public static string CustomerSchedulewith2ndFE(
    string Name, 
    string Case_no, 
    string FE, 
    string Email, 
    string Contact, 
    string FE2, 
    string Email2, 
    string contact2, 
    string Link 
    )
{
    string body = string.Empty;
    using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/CustomerSchedulewith2ndFE.html")))
    {
        body = reader.ReadToEnd();
    }

    body = body.Replace("{name}", Name);
    body = body.Replace("{Case_no}", Case_no);
    body = body.Replace("{FE}", FE);
    body = body.Replace("{Email}", Email);
    body = body.Replace("{Contact}", Contact);

    // Replace second engineer details
    body = body.Replace("{FE2}", FE2);
    body = body.Replace("{Email2}", Email2);
    body = body.Replace("{Contact2}", contact2);
    body = body.Replace("{Link}", Link);

    return body;
}

        public static string FECheckedIn(string Name, string Case_no, string FE, string Email, string Contact, string In_Date, string In_time)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/FECheckedIn.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);

            body = body.Replace("{Case_no}", Case_no);

            body = body.Replace("{FE}", FE);

            body = body.Replace("{Email}", Email);

            body = body.Replace("{Contact}", Contact);
            body = body.Replace("{In_Date}", In_Date);

            body = body.Replace("{In_time}", In_time);

            return body;
        }  
        public static string FECheckedIn2ndFE(string Name, string Case_no, string FE, string Email, string Contact, string FE2, string Email2, string Contact2, string In_Date, string In_time, string In_Date2, string In_time2)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/FECheckedIn2ndFE.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);

            body = body.Replace("{Case_no}", Case_no);

            body = body.Replace("{FE}", FE);

            body = body.Replace("{Email}", Email);

            body = body.Replace("{Contact}", Contact);

            body = body.Replace("{In_Date}", In_Date);

            body = body.Replace("{In_time}", In_time);


            body = body.Replace("{FE2}", FE2);

            body = body.Replace("{Email2}", Email2);

            body = body.Replace("{Contact2}", Contact2);
            body = body.Replace("{In_Date2}", In_Date2);

            body = body.Replace("{In_time2}", In_time2);




            return body;
        }

        public static string FEChanged( string Case_no, string Name, string Email, string Contact ,string callbackUrl )
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/CustomerFEChanged.html")))
            {
                body = reader.ReadToEnd();
            }


            body = body.Replace("{Case_No}", Case_no);

            body = body.Replace("{Name}", Name);

            body = body.Replace("{Email}", Email);

            body = body.Replace("{Contact}", Contact);

            body = body.Replace("{Link}", callbackUrl);


            return body;
        }
        public static string FEChanged2ndFE(string Case_no, string Name, string Email, string Contact, string Name2, string Email2, string Contact2, string callbackUrl )
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/CustomerFEChanged2ndFE.html")))
            {
                body = reader.ReadToEnd();
            }


            body = body.Replace("{Case_No}", Case_no);

            body = body.Replace("{Name}", Name);

            body = body.Replace("{Email}", Email);

            body = body.Replace("{Contact}", Contact);

            body = body.Replace("{Name2}", Name2);

            body = body.Replace("{Email2}", Email2);

            body = body.Replace("{Contact2}", Contact2);
            body = body.Replace("{Link}", callbackUrl);

            return body;
        }


        public static string FeAvailableAtDifferentTime(string Case_no, string Name, string Email, string Contact, string Fe_Time)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/FeAvailableAtDifferentTime.html")))
            {
                body = reader.ReadToEnd();
            }

            
            body = body.Replace("{provided_time}", Fe_Time);

            body = body.Replace("{Case_No}", Case_no);

            body = body.Replace("{Name}", Name);

            body = body.Replace("{Email}", Email);

            body = body.Replace("{Contact}", Contact);

            return body;
        }
        public static string JobConfirm(string Name, string Site_Name, string Contact_Name, string Site_Address, string Disptach_Date_Time, string Scope_Work, string Pregame, string system_info, string oem)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/JobConfirm.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);
            body = body.Replace("{Site_Name}", Site_Name);
            body = body.Replace("{Contact_Name}", Contact_Name);
            body = body.Replace("{Site_Address}", Site_Address);
            body = body.Replace("{Disptach_Date_Time}", Disptach_Date_Time);
            body = body.Replace("{Scope_Work}", Scope_Work);
            body = body.Replace("{Pregame}", Pregame.Replace(Environment.NewLine, "<br>")); // Replace new lines with <br>
            body = body.Replace("{system_info}", system_info);
            body = body.Replace("{oem}", oem);

            return body;
        }

        public static string CustomterClosing(string Name, string In_Date, string In_time, string Out_Date, string Out_time, string Parts_retained, string Parts_returned, string Other_Details, string link,
            string Case_No, string Ticket_No, string OEM, string system_info, string FE, string TRT, string RTT, string Part_Detail , string Job_Description)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/CustomterClosing.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{Name}", Name);

            body = body.Replace("{In_Date}", In_Date);

            body = body.Replace("{In_time}", In_time);

            body = body.Replace("{Out_Date}", Out_Date);

            body = body.Replace("{Out_time}", Out_time);

            body = body.Replace("{Parts_retained}", Parts_retained);    

            body = body.Replace("{Parts_returned}", Parts_returned);

            body = body.Replace("{Other_Details}", Other_Details);

            body = body.Replace("{link}", link);

            body = body.Replace("{Case_No}", Case_No);

            body = body.Replace("{Ticket_No}", Ticket_No);

            body = body.Replace("{OEM}", OEM);

            body = body.Replace("{system_info}", system_info);

            body = body.Replace("{FE}", FE);

            body = body.Replace("{TRT}", TRT);

            body = body.Replace("{RTT}", RTT);

            //body = body.Replace("{Part_Detail}", Part_Detail);
            body = body.Replace("{Job_Description}", Job_Description);

            return body;
        }

        public static string CustomterClosing2ndFE(
     string Name,
     string In_Date,
     string In_time,
     string Out_Date,
     string Out_time,
     string In_Date2,
     string In_time2,
     string Out_Date2,
     string Out_time2,
     string Parts_retained,
     string Parts_returned,
     string Other_Details,
     string link,
     string Case_No,
     string Ticket_No,
     string OEM,
     string system_info,
     string FE,
     string TRT,
     string RTT,
     string Part_Detail,
     string FE2,
     string TRT2,
     string Job_Description)
        {
            string body = string.Empty;

            // Load the template
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/CustomterClosing2ndFE.html")))
            {
                body = reader.ReadToEnd();
            }

            // Replace placeholders with actual values
            body = body.Replace("{Name}", Name);
            body = body.Replace("{In_Date}", In_Date);
            body = body.Replace("{In_time}", In_time);
            body = body.Replace("{Out_Date}", Out_Date);
            body = body.Replace("{Out_time}", Out_time);
            body = body.Replace("{Parts_retained}", Parts_retained);
            body = body.Replace("{Parts_returned}", Parts_returned);
            body = body.Replace("{Other_Details}", Other_Details);
            body = body.Replace("{link}", link);
            body = body.Replace("{Case_No}", Case_No);
            body = body.Replace("{Ticket_No}", Ticket_No);
            body = body.Replace("{OEM}", OEM);
            body = body.Replace("{system_info}", system_info);
            body = body.Replace("{FE}", FE);
            body = body.Replace("{TRT}", TRT);
            body = body.Replace("{RTT}", RTT);
            body = body.Replace("{FE2}", FE2);
            body = body.Replace("{In_Date2}", In_Date2);
            body = body.Replace("{In_time2}", In_time2);
            body = body.Replace("{Out_Date2}", Out_Date2);
            body = body.Replace("{Out_time2}", Out_time2);
            body = body.Replace("{TRT2}", TRT2);
            body = body.Replace("{Part_Detail}", Part_Detail);
            body = body.Replace("{Job_Description}", Job_Description);

            return body;
        }

        public static string FEClosing(string Name, string Ticket, string OEM, string system_info, string TRT, string RTT, string Part_Detail, string In_Date, string In_time, string Out_Date,
            string Out_time, string Other_Details,string Inv_address)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/FEClosing.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);
            body = body.Replace("{Ticket}", Ticket);
            body = body.Replace("{OEM}", OEM);

            body = body.Replace("{system_info}", system_info);

            body = body.Replace("{TRT}", TRT);

            body = body.Replace("{RTT}", RTT);

            body = body.Replace("{Part_Detail}", Part_Detail);
            body = body.Replace("{In_Date}", In_Date);

            body = body.Replace("{In_time}", In_time);

            body = body.Replace("{Out_Date}", Out_Date);

            body = body.Replace("{Out_time}", Out_time);
            body = body.Replace("{Other_Details}", Other_Details);
            body = body.Replace("{Inv_address}", Inv_address);
            return body;
        }

        public static string CustomerReschedule(string name, string ticket, string caseNo, string dispatchDate, string ticketOld, string rescheduleReason)
        {
            string body;

            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/CustomerReschedule.html")))
            {
                body = reader.ReadToEnd();
            }

            // Replace placeholders in the template with actual values
            body = body.Replace("{Reschedule_Reason}", rescheduleReason)
                       .Replace("{name}", name)
                       .Replace("{Ticket_No}", ticket)
                       .Replace("{Ticket_No_Old}", ticketOld)
                       .Replace("{Case_no}", caseNo);

            // Handle the dispatch date, providing a default message if it's null
            body = body.Replace("{Dispatch_Date}", dispatchDate ?? "Reschedule Date is not fixed yet");

            return body;
        }

        public static string FEReschedule(string Name, string Ticket, string Dispatch_Date, string Reschedule_Reason)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/FEReschedule.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{Reschedule_Reason}", Reschedule_Reason);
            body = body.Replace("{name}", Name);
            body = body.Replace("{Ticket_No}", Ticket);
            body = body.Replace("{Dispatch_Date}", Dispatch_Date);


            return body;
        }

        public static string SendTravelCharge(Ticket_FE_Charges ticketCharges , string caseNo) 
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/TravelCharges.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{Travel_Amount1}", ticketCharges.SentTravelCharge.ToString());
            body = body.Replace("{Case_No}", caseNo);

            return body;
        }
        public static string SendTravelCharge2(Ticket_FE_Charges ticketCharges, string caseNo)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/TravelCharges2.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Travel_Amount1}", ticketCharges.SentTravelCharge.ToString());
            body = body.Replace("{Travel_Amount2}", ticketCharges.SentTravelCharge2.ToString());
            body = body.Replace("{Case_No}", caseNo);
            return body;
        }

        public static string FEDecline(string Name)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/FEDecline.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);


            return body;
        }

        public static string CustomerDecline(string Name)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/CustomerDecline.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);


            return body;
        }

        public static string FECancel(string Name)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/FECancel.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);


            return body;
        }

        public static string CustomerCancel(string Name, string Ticket_No, string Case_No, string Site_name, string Site_address, string Scope_of_work, string Dispatch_Date, string Dispatch_Time, string Reason_for_cancellation)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/CustomerCancel.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);
            body = body.Replace("{Ticket_No}", Ticket_No);
            body = body.Replace("{Case_No}", Case_No);
            body = body.Replace("{Site_name}", Site_name);
            body = body.Replace("{Site_address}", Site_address);
            body = body.Replace("{Scope_of_work}", Scope_of_work);
            body = body.Replace("{Dispatch_Date}", Dispatch_Date);
            body = body.Replace("{Dispatch_Time}", Dispatch_Time);
            body = body.Replace("{Reason_for_cancellation}", Reason_for_cancellation);

            return body;
        }
        public static string sendmailAcc(string recp, string subj, string body, string From)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string ress;

            MailMessage mm = new MailMessage("support@inwinteck.com", recp);
            mm.Subject = subj;
            mm.Bcc.Add("partner@inwinteck.com, vk@inwinteck.com, contact@3sptechmind.com,partnereurope@inwinteck.com,partnermea@inwinteck.com,partneramer@inwinteck.com,support@inwinteck.com,hd@inwinteck.com");
            //  mm.Bcc.Add("contact@3sptechmind.com");
            mm.IsBodyHtml = true;
            mm.Body = body;

            // Initialize the SmtpClient object
            SmtpClient smtp = new SmtpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            smtp.Host = "smtp.office365.com";
            smtp.EnableSsl = true;
            //   NetworkCredential NetworkCred = new NetworkCredential("partner@inwinteck.com", "Inwinteck@2577");
            NetworkCredential NetworkCred = new NetworkCredential("support@inwinteck.com", "Inwinteck@2577");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            //
            try
            {
                mm.From = new MailAddress(From);

                smtp.Send(mm);
                ress = "true";
                //EmailLog EL = new EmailLog();
                //EL.emailid = recp;
                //EL.type = subj;
                //EL.date = DateTime.Now;
                //db.EmailLog.Add(EL);
                //EL.Status = "Send";
                //EL.Status_Msg = "Email Send Successfully";
                string msg = "/Email/Log" + " " + "Recp: " + recp + " " + "subject: " + subj + " " + "From: " + From + " " + " Status: Success";
                utlity.createlog(msg);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ress = ex.ToString();
                string msg = "/Email/Log" + " " + "Recp: " + recp + " " + "subject: " + subj + " " + "From: " + From + " " + " Status: Error" + " Message: " + " " + ress;
                utlity.createlog(msg);
                //EmailLog EL = new EmailLog();
                //EL.emailid = recp;
                //EL.type = subj;
                //EL.date = DateTime.Now;
                //db.EmailLog.Add(EL);
                //EL.Status = "Email Not Send";
                //EL.Status_Msg = ress;
            }
            return ress;
        }


        public static string sendmail(string recp, string subj, string body, string From)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string ress;

            MailMessage mm = new MailMessage("support@inwinteck.com", recp);
            mm.Bcc.Add("vk@inwinteck.com, contact@3sptechmind.com");
            mm.Subject = subj;
            mm.IsBodyHtml = true;
            mm.Body = body;


            SmtpClient smtp = new SmtpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            smtp.Host = "smtp.office365.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential("support@inwinteck.com", "Inwinteck@2577");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            //
            try
            {
                mm.From = new MailAddress(From);

                smtp.Send(mm);
                ress = "true";
                //EmailLog EL = new EmailLog();
                //EL.emailid = recp;
                //EL.type = subj;
                //EL.date = DateTime.Now;
                //db.EmailLog.Add(EL);
                //EL.Status = "Send";
                //EL.Status_Msg = "Email Send Successfully";
                string msg = "/Email/Log" + " " + "Recp: " + recp + " " + "subject: " + subj + " " + "From: " + From + " " + " Status: Success";
                utlity.createlog(msg);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ress = ex.Message.ToString();
                string msg = "/Email/Log" + " " + "Recp: " + recp + " " + "subject: " + subj + " " + "From: " + From + " " + " Status: Error" + " " + ress;
                utlity.createlog(msg);
                //EmailLog EL = new EmailLog();
                //EL.emailid = recp;
                //EL.type = subj;
                //EL.date = DateTime.Now;
                //db.EmailLog.Add(EL);
                //EL.Status = "Email Not Send";
                //EL.Status_Msg = ress;
            }
            return ress;
        }


        public static string sendmail2(string recp, string subj, string body, string from, string messageId, bool isReply)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string ress;

            MailMessage mm = new MailMessage(from, recp);
            mm.Bcc.Add("vk@inwinteck.com, contact@3sptechmind.com");

            //if (isReply && !subj.StartsWith("Re:"))
            //{
            //    subj = "Re: " + subj;
            //}

            mm.Subject = subj;
            mm.IsBodyHtml = true;
            mm.Body = body;

            if (isReply)
            {
                // For replies, use In-Reply-To and References headers
                mm.Headers.Add("In-Reply-To", $"<{messageId}>");
                mm.Headers.Add("References", $"<{messageId}>");
            }
            else
            {
                // For initial email, set Message-ID
                mm.Headers.Add("Message-ID", $"<{messageId}>");
            }

            SmtpClient smtp = new SmtpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            smtp.Host = "smtp.office365.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential("support@inwinteck.com", "Inwinteck@2577");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;

            try
            {
                smtp.Send(mm);
                ress = "true";
                string msg = "/Email/Log" + " " + "Recp: " + recp + " " + "subject: " + subj + " " + "From: " + from + " " + " Status: Success";
                utlity.createlog(msg);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ress = ex.Message.ToString();
                string msg = "/Email/Log" + " " + "Recp: " + recp + " " + "subject: " + subj + " " + "From: " + from + " " + " Status: Error" + " " + ress;
                utlity.createlog(msg);
            }
            return ress;
        }

        public static string sendmailattach(string recp, string subj, string body, string from, string filename)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string ress;

            var mm = new MailMessage(from, recp)
            {
                Subject = subj,
                Body = body,
                IsBodyHtml = true
            };
            mm.Bcc.Add("vk@inwinteck.com, contact@3sptechmind.com");

            // Check if filename is not null or empty before adding it as an attachment
            if (!string.IsNullOrEmpty(filename))
            {
                mm.Attachments.Add(new Attachment(filename));
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.office365.com",
                EnableSsl = true,
                Port = 587,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential("support@inwinteck.com", "Inwinteck@2577")
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                mm.From = new MailAddress(from);

                smtp.Send(mm);
                ress = "true";
                string msg = $"/Email/Log Recp: {recp} subject: {subj} From: {from} Status: Success";
                utlity.createlog(msg);
            }
            catch (Exception ex)
            {
                ress = ex.Message;
                string msg = $"/Email/Log Recp: {recp} subject: {subj} From: {from} Status: Error {ress}";
                utlity.createlog(msg);
            }
            return ress;
        }
        public static string sendmailattach2(string recp, string subj, string body, string from, string filename, string messageId, bool isReply)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string ress;

            var mm = new MailMessage(from, recp)
            {
                Subject = subj,
                Body = body,
                IsBodyHtml = true
            };
            mm.Bcc.Add("vk@inwinteck.com, contact@3sptechmind.com");

            // Check if filename is not null or empty before adding it as an attachment
            if (!string.IsNullOrEmpty(filename))
            {
                mm.Attachments.Add(new Attachment(filename));
            }

            if (isReply)
            {
                // For replies, use In-Reply-To and References headers
                mm.Headers.Add("In-Reply-To", $"<{messageId}>");
                mm.Headers.Add("References", $"<{messageId}>");
            }
            else
            {
                // For initial email, set Message-ID
                mm.Headers.Add("Message-ID", $"<{messageId}>");
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.office365.com",
                EnableSsl = true,
                Port = 587,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential("support@inwinteck.com", "Inwinteck@2577")
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                mm.From = new MailAddress(from);

                smtp.Send(mm);
                ress = "true";
                string msg = $"/Email/Log Recp: {recp} subject: {subj} From: {from} Status: Success";
                utlity.createlog(msg);
            }
            catch (Exception ex)
            {
                ress = ex.Message;
                string msg = $"/Email/Log Recp: {recp} subject: {subj} From: {from} Status: Error {ress}";
                utlity.createlog(msg);
            }
            return ress;
        }


        public static int GetBussinessId(string path)
        {
            int bid = 0;
            try
            {
                if (path != null)
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    bid = (from c in db.component_master where c.target_url == path select c.business_object_id).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {

            }
            return bid;
        }



        public static string CheckTicketNo(int eu_id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string ticketno;
            string year;
            string serial;
            string eutick;
            string euabb;

            try
            {
                string tn = (from s in db.Ticket orderby s.Id descending select s.Ticket_No).FirstOrDefault();
                string euno = (from s in db.Ticket where s.EU_ID == eu_id orderby s.Id descending select s.Ticket_No).FirstOrDefault();
                if (tn != null)
                {
                    string tn1 = tn.Remove(tn.Length - 2);
                    serial = tn1.Substring(tn1.Length - 6);
                    serial = (Convert.ToInt32(serial) + 1).ToString("D6");
                    if (euno != null)
                    {
                        string tn2 = tn.Remove(tn.Length - 8);
                        eutick = tn2.Substring(tn2.Length - 3);
                        eutick = (Convert.ToInt32(eutick) + 1).ToString("D3");
                    }
                    else
                    {
                        eutick = "001";
                    }
                }
                else
                {
                    serial = "000001";
                    if (euno != null)
                    {
                        string tn2 = tn.Remove(tn.Length - 8);
                        eutick = tn2.Substring(tn2.Length - 3);
                        eutick = (Convert.ToInt32(eutick) + 1).ToString("D3");
                    }
                    else
                    {
                        eutick = "001";
                    }
                }

                year = DateTime.Now.ToString("yy");
                euabb = (from s in db.EU_Master where s.Id == eu_id select s.Ticket_Abv).FirstOrDefault();
                ticketno = euabb + eutick + serial + year;

            }
            catch (Exception ex)
            {
                ticketno = "";
            }
            return ticketno;
        }

        public static string NDAPDF(string recp, string subj, string body, string From, string FEName, string DOA, string filename)
        {
            string ress;
            //========================================== Create PDF =====================================
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<p style='text-aling:center;'>");
                    sb.Append("<img src='https://fms.inwinteck.com/assets/images/logo-L.png'>");
                    sb.Append("</p><br>");
                    sb.Append("<p style='text-aling:center;'>");
                    sb.Append("<b>Contracted Services Non-disclosure and Confidentiality</b>");
                    sb.Append("</p><br>");
                    sb.Append("<p>");
                    sb.Append("We at Inwinteck Pte Ltd registered at 23 Kelantan Lane,#04-01 Kim Hoe Centre, Singapore - 208642 is a Global onsite on time IT resource provider look forward to working with your company in helping to provide the quality service that has helped us to become successful in the industry.");
                    sb.Append("</p><br>");
                    sb.Append("<p>");
                    sb.Append("<b>DECLARE: </b><br>");
                    sb.Append("Inwinteck provides installation and maintenance onsite service worldwide.This Non-disclosure and Confidentiality Agreement, hereinafter referred to as Agreement”, is made and entered into on the date set forth below, by and between Inwinteck., hereinafter referred to as “Company”, and your company, hereinafter referred to as “Contractor”. whereas the Contractor is contracted to provide services to the Company; and whereas the Contractor will or may have access to and will or may become exposed to Confidential and Proprietary information, Trade Secrets or Trade Secret Information of the Company and its customers, agents, vendors, suppliers, and other related parties with whom the Company has a business relationship and whereas the company would sustain substantial and irreparable damage to the business and assets of the Company, customers and its third party business relationships should the Contractor disclose or use the information obtained through the Contractor’s contract services with the Company, and Whereas the Contractor may learn of or be involved with certain techniques, formulas, methods and procedures utilized by the Company in providing its services to its customers and prospects. Now, therefore, in consideration of the Contractor’s contract and business relationship with the Company and for other good and valuable consideration, the receipt and adequacy of which are hereby acknowledged, Recipient hereby agrees to be bound by the following:");
                    sb.Append("</p><br>");
                    sb.Append("<p>");
                    sb.Append("<b>DEFINED TERMS</b><br>");
                    sb.Append("1.1 “Confidential Information” means any data or information, other than Trade Secrets, that is of value to the Company and is not generally known to competitors of the Company.<br>");
                    sb.Append("1.2 “Confidential Proprietary Information” means all information relating to the Company’s marketing techniques, pricing methodologies and price lists, pricing policies and practices, business methods and contracts or contractual relations with the Company’s customers and suppliers, and identified prospective customers.<br>");
                    sb.Append("1.3 “Confidential Information” includes any and all information described in paragraphs 1.1 and that the Company obtains from or through its relationships with third parties, whether customers, suppliers, professionals or consultants, or other business relationships.<br>");
                    sb.Append("1.4 “Trade Secrets” means any and all information of the Company of a technical or non-technical nature, data, a formula, a pattern, a compilation, a program, a devise, a method, a technique, a drawing, a process, financial data, financial plans, product plans, potential customer lists or data, customer list or data, supplier lists or data, from or through which the Company derives economic value, actual or potential, from not being generally known to and not being readily ascertainable by proper means by other persons who also can obtain economic value from its disclosure or use.<br>");
                    sb.Append("1.5 “Trade Secret Information” means any and all computer software in all forms, flow charts, algorithms, coding sheets, subroutines, compilers, assemblers, design concepts and related documentation of manuals, and any and all technique and forms related to the maintenance of hardware and software products with which the Company actually is or potentially may be involved with.<br>");
                    sb.Append("</p><br>");
                    sb.Append("<p>");
                    sb.Append("The Contractor acknowledges and accepts to perform his/her duties professionally, in accordance with but not restricted to the following guidelines.<br>");
                    sb.Append("1. The Contractor shall represent him/herself as Inwinteck or the client we are representing if required.<br>");
                    sb.Append("2. The Contractor shall never directly approach Inwinteck clients to sell his/her services or goods of any kind.<br>");
                    sb.Append("3. The Contractor shall not perform any work for Inwinteck’s client outside the scope of the service call, unless so directed by Inwinteck.<br>");
                    sb.Append("4. The Contractor must inform Inwinteck dispatcher when arriving and departing the site.<br>");
                    sb.Append("5. The Contractor must have the end user sign the field service report before leaving the site.<br>");
                    sb.Append("6. The Contractor must notify Inwinteck if he/she is delayed in meeting the scheduled arrival time.<br>");
                    sb.Append("7. The Contractor must always leave the company’s clients work area exactly the way it was found.<br>");
                    sb.Append("8. The Contractor agrees to our rates<br>");
                    sb.Append("9. A special rate only applies if a project is discussed prior to any service call taking place.<br>");
                    sb.Append("10. The Contractor will never discuss payment with the end user client. All invoicing concerns must be addressed directly to Inwinteck’s management<br>");
                    sb.Append("</p><br>");
                    sb.Append("<p>");
                    sb.Append("<b>RECIPIENT OBLIGATIONS :</b><br>");
                    sb.Append("1) Contractor acknowledges that the Company owns all right title and interest, including all worldwide copyrights, patents, Confidential Information, Confidential Proprietary Information, Trade Secrets, and Trade Secret Information, and all modification, revisions and derivative works thereof, collectively, hereinafter “Confidential Business Assets”.<br>");
                    sb.Append("2) Contractor further agrees that nothing herein gives Contractor any right, title or interest in the Confidential Business Assets.<br>");
                    sb.Append("3) During the term of the business relationship between the Company and the Contractor and after the termination thereof, for a period of three (3) years from the date of termination, Recipient will not, except as expressly authorized or directed by the Company in writing use, copy, duplicate, transfer, transmit or disclose, or permit any unauthorized person access to, any Confidential Business Assets belonging to the Company or any of the Company’s related third parties, customers or actively sought prospective customers.Contractor specifically agrees, when given written notice by the Company, to honor and adhere to the terms and provisions of Confidential Agreements signed by the Company involving the services of the Contractor and will hold the Company harmless and indemnify the Company for the Contractor’s breach of any said agreement(s).<br>");
                    sb.Append("4) Upon request of the Company and in any event, upon the termination of the Contractor’s business relationship with the Company, the Contractor will deliver to the Company all memoranda, notes, records, tapes and documentation, disks, manuals, files and other documents, handwritten materials and all copies thereof in any form, concerning or containing Confidential Business Assets that are in the Recipient’s possession, whether made or compiled by the Contractor or furnished to the Contractor by the Company.<br>");
                    sb.Append("5) The Contractor acknowledges that the Contractor has acquired or will acquire specialized knowledge and experience into the Company’s business, that the Company’s reputation and relationships within the industry are and will be considered to be of great value to the Company as part of the Company assets, and that if Contractor’s thus gained knowledge, experience, reputation or relationships are used to compete with the Company, serious harm to the Company may result.Therefore, the Contractor agrees that during the actual customer visit, the contractor is not to attempt to “sell” any other services while being contracted by Inwinteck.to do specific work.<br>");
                    sb.Append("6) The Contractor and the Company hereto specifically agree that if any provision or any part of any provision of this Agreement is not be valid for any reason, such provision shall be entirely severable from, and shall have no effect upon the remainder of this Agreement.Any such invalid provision shall be subject to partial enforcement to the extent necessary to protect the interest of the Company.In the event that any of the provisions in this Agreement should ever be adjudicated to exceed the time, geographic, service or product limitations permitted by applicable law in any jurisdiction, then such provision shall be deemed reformed in such jurisdiction to the maximum time, geographic, service or product limitations permitted by applicable law.<br>");
                    sb.Append("</p><br>");
                    sb.Append("<p>");
                    sb.Append("<b>Breach:</b><br>");
                    sb.Append("The Contractor and the Company hereto acknowledge that a breach by the Contractor of any of the terms or conditions of this Agreement will result in irrevocable harm to the Company and the remedies at law for such breach may not adequately compensate the Company for damages suffered. Accordingly, the Contractor agrees that in the event of such breach, that the Company will be entitled to injunctive relief or such other equitable remedy as a court of competent jurisdiction may provide.Nothing contained herein will be construed to limit the Company’s right to any remedies at law, including the recovery of damages for breach of this Agreement.<br>");
                    sb.Append("</p><br>");
                    sb.Append("<p>");
                    sb.Append("<b> PAYMENT Terms </b><br>");
                    sb.Append("1.Invoices will only be sent after completion and delivery of all Services and/or Deliverables.<br>");
                    sb.Append("2.The FSR must be correctly completed and emailed or faxed at the closing of each call, to ensure payment.<br>");
                    sb.Append("3.The price quoted in the email before commencing of work should be same in the invoice. Any variations in pricing will not be paid.<br>");
                    sb.Append("4.Must have time sheet signed off from onsite authority with complete details.<br>");
                    sb.Append("5.Payment will be released within 30 days from the time of receipt of the invoice as per the agreed pricing terms along with job completed signed times sheet.<br>");
                    sb.Append("6.Inwinteck does not pay for travel, accommodation, travel hours, mileage, parking, tolls, tickets, or long distance phone calls incurred by the Contractor for the service call until it has been approved by Inwinteck prior attending the service call.<br>");
                    sb.Append("7.Contractor backing off at the last minute will levy a penalty.<br>");
                    sb.Append("</p><br>");
                    sb.Append("<p>");
                    sb.Append("<b>AMENDMENT</b><br>");
                    sb.Append("This Agreement may not be amended or modified by both the Contractor and the Company.<br>");
                    sb.Append("In Witness Whereof this Service Provider Agreement has been executed by the duly authorised representatives of the parties.<br>");
                    sb.Append("</p><br>");
                    sb.Append("<p>");
                    sb.Append("Name :<b>");
                    sb.Append(FEName);
                    sb.Append("</b></p>");
                    sb.Append("<p>");
                    sb.Append("Date of Acceptance : <b>");
                    sb.Append(DOA);
                    sb.Append("</b></p>");
                    StringReader sr = new StringReader(sb.ToString());

                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                        pdfDoc.Open();
                        htmlparser.Parse(sr);
                        pdfDoc.Close();
                        byte[] bytes = memoryStream.ToArray();
                        memoryStream.Close();

                        //========================================================= Email Sending 

                        ApplicationDbContext db = new ApplicationDbContext();

                        //  MailMessage mm = new MailMessage("partner@inwinteck.com", recp);
                        MailMessage mm = new MailMessage("support@inwinteck.com", recp);
                        mm.Subject = subj;
                        mm.Bcc.Add("partner@inwinteck.com, vk@inwinteck.com, contact@3sptechmind.com,partnereurope@inwinteck.com,partnermea@inwinteck.com,partneramer@inwinteck.com,support@inwinteck.com,hd@inwinteck.com");
                        //  mm.Bcc.Add("contact@3sptechmind.com");
                        mm.IsBodyHtml = true;
                        mm.Body = body;
                        mm.Attachments.Add(new Attachment(new MemoryStream(bytes), filename));
                        SmtpClient smtp = new SmtpClient();
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        smtp.Host = "smtp.office365.com";
                        smtp.EnableSsl = true;
                        // NetworkCredential NetworkCred = new NetworkCredential("partner@inwinteck.com", "Inwinteck@2577");
                        NetworkCredential NetworkCred = new NetworkCredential("support@inwinteck.com", "Inwinteck@2577");
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        //
                        try
                        {
                            mm.From = new MailAddress(From);

                            smtp.Send(mm);
                            ress = "true";
                            //EmailLog EL = new EmailLog();
                            //EL.emailid = recp;
                            //EL.type = subj;
                            //EL.date = DateTime.Now;
                            //db.EmailLog.Add(EL);
                            //EL.Status = "Send";
                            //EL.Status_Msg = "Email Send Successfully";
                            //db.SaveChanges();
                            string msg1 = "NDA" + " " + "Subject" + " " + subj + " " + "Email" + " " + recp + " " + "Status" + "Send";
                            utlity.createlog(msg1);

                        }
                        catch (Exception ex)
                        {
                            ress = ex.Message.ToString();
                            //EmailLog EL = new EmailLog();
                            //EL.emailid = recp;
                            //EL.type = subj;
                            //EL.date = DateTime.Now;
                            //db.EmailLog.Add(EL);
                            //EL.Status = "Email Not Send";
                            //EL.Status_Msg = ress;

                            string msg1 = "NDA" + " " + "Subject" + " " + subj + " " + "Email" + " " + recp + " " + "Status" + "Not Send" + " " + "Error" + " " + ress;
                            utlity.createlog(msg1);
                        }
                    }
                }
            }
            return ress;
        }

        public static string CheckEnqNo(int cust)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string ticketno;
            string year;
            string serial;
            string eutick;
            string euabb;

            try
            {
                string tn = (from s in db.Enq_IT orderby s.Id descending select s.Enq_No).FirstOrDefault();
                string euno = (from s in db.Enq_IT where s.Customer_Id == cust orderby s.Id descending select s.Enq_No).FirstOrDefault();
                if (tn != null)
                {
                    string tn1 = tn.Remove(tn.Length - 2);
                    serial = tn1.Substring(tn1.Length - 6);
                    serial = (Convert.ToInt32(serial) + 1).ToString("D6");
                    if (euno != null)
                    {
                        string tn2 = tn.Remove(tn.Length - 8);
                        eutick = tn2.Substring(tn2.Length - 3);
                        eutick = (Convert.ToInt32(eutick) + 1).ToString("D3");
                    }
                    else
                    {
                        eutick = "001";
                    }
                }
                else
                {
                    serial = "000001";
                    if (euno != null)
                    {
                        string tn2 = tn.Remove(tn.Length - 8);
                        eutick = tn2.Substring(tn2.Length - 3);
                        eutick = (Convert.ToInt32(eutick) + 1).ToString("D3");
                    }
                    else
                    {
                        eutick = "001";
                    }
                }

                year = DateTime.Now.ToString("yy");
                euabb = (from s in db.IT_Customer_Master where s.Id == cust select s.Enq_Abbreviation).FirstOrDefault();
                ticketno = euabb + eutick + serial + year;

            }
            catch (Exception ex)
            {
                ticketno = "";
            }
            return ticketno;
        }

        public static string EnqVendor(string SP, string JL, string JE, string JD, string JDs, string CR, string CD)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/EnqVendor.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{SP}", SP);

            body = body.Replace("{JL}", JL);

            body = body.Replace("{JE}", JE);

            body = body.Replace("{JD}", JD);

            body = body.Replace("{JDs}", JDs);

            body = body.Replace("{CR}", CR);

            body = body.Replace("{CD}", CD);

            return body;
        }

        public static string EnqCustomer(string Name, string Candidate, string Rate)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/EnqCustomer.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{name}", Name);

            body = body.Replace("{CN}", Candidate);

            body = body.Replace("{PR}", Rate);


            return body;
        }

        public static string sendmailEnq(string recp, string subj, string body, string From, string PathToAttachment)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string ress;

            MailMessage mm = new MailMessage("support@inwinteck.com", recp);
            mm.Bcc.Add("shiv168@gmail.com, kanchangire@gmail.com, vk@inwinteck.com, sk@inwinteck.com,contact@3sptechmind.com");
            mm.Subject = subj;
            mm.Attachments.Add(new Attachment(PathToAttachment));
            mm.IsBodyHtml = true;
            mm.Body = body;

            SmtpClient smtp = new SmtpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            smtp.Host = "smtp.office365.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential("support@inwinteck.com", "Inwinteck@2577");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            //
            try
            {
                mm.From = new MailAddress(From);

                smtp.Send(mm);
                ress = "true";
                //EmailLog EL = new EmailLog();
                //EL.emailid = recp;
                //EL.type = subj;
                //EL.date = DateTime.Now;
                //db.EmailLog.Add(EL);
                //EL.Status = "Send";
                //EL.Status_Msg = "Email Send Successfully";
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ress = ex.Message.ToString();
                //EmailLog EL = new EmailLog();
                //EL.emailid = recp;
                //EL.type = subj;
                //EL.date = DateTime.Now;
                //db.EmailLog.Add(EL);
                //EL.Status = "Email Not Send";
                //EL.Status_Msg = ress;
            }
            return ress;
        }

        public static string sendmailWebsite(string recp, string subj, string body, string From)
        {
           // ApplicationDbContext db = new ApplicationDbContext();
            string ress;

            MailMessage mm = new MailMessage("support@inwinteck.com", recp);
          //  mm.Bcc.Add("shiv168@gmail.com, kanchangire@gmail.com, vk@inwinteck.com, sk@inwinteck.com,contact@3sptechmind.com");
            mm.Subject = subj;
           // mm.Attachments.Add(new Attachment(PathToAttachment));
            mm.IsBodyHtml = true;
            mm.Body = body;

            SmtpClient smtp = new SmtpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            smtp.Host = "smtp.office365.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential("support@inwinteck.com", "Inwinteck@2577");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            //
            try
            {
                mm.From = new MailAddress("support@inwinteck.com");

                smtp.Send(mm);
                ress = "true";
                //EmailLog EL = new EmailLog();
                //EL.emailid = recp;
                //EL.type = subj;
                //EL.date = DateTime.Now;
                //db.EmailLog.Add(EL);
                //EL.Status = "Send";
                //EL.Status_Msg = "Email Send Successfully";
               // db.SaveChanges();
            }
            catch (Exception ex)
            {
                ress = ex.Message.ToString();
                //EmailLog EL = new EmailLog();
                //EL.emailid = recp;
                //EL.type = subj;
                //EL.date = DateTime.Now;
                //db.EmailLog.Add(EL);
                //EL.Status = "Email Not Send";
                //EL.Status_Msg = ress;
            }
            return ress;
        }


        public static string sendmailCareer(string recp, string subj, string body, string From, string pathResume, string pathCareer)
        {
            // ApplicationDbContext db = new ApplicationDbContext();
            string ress;

            MailMessage mm = new MailMessage("support@inwinteck.com", recp);
            //  mm.Bcc.Add("shiv168@gmail.com, kanchangire@gmail.com, vk@inwinteck.com, sk@inwinteck.com,contact@3sptechmind.com");
            mm.Subject = subj;
            if (pathResume != "")
            {
                mm.Attachments.Add(new Attachment(pathResume));
            }
            if (pathCareer != "")
            {
                mm.Attachments.Add(new Attachment(pathCareer));
            }

            mm.IsBodyHtml = true;
            mm.Body = body;

            SmtpClient smtp = new SmtpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            smtp.Host = "smtp.office365.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential("support@inwinteck.com", "Inwinteck@2577");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            //
            try
            {
                mm.From = new MailAddress("support@inwinteck.com");

                smtp.Send(mm);
                ress = "true";
                //EmailLog EL = new EmailLog();
                //EL.emailid = recp;
                //EL.type = subj;
                //EL.date = DateTime.Now;
                //db.EmailLog.Add(EL);
                //EL.Status = "Send";
                //EL.Status_Msg = "Email Send Successfully";
                // db.SaveChanges();
            }
            catch (Exception ex)
            {
                ress = ex.Message.ToString();
                //EmailLog EL = new EmailLog();
                //EL.emailid = recp;
                //EL.type = subj;
                //EL.date = DateTime.Now;
                //db.EmailLog.Add(EL);
                //EL.Status = "Email Not Send";
                //EL.Status_Msg = ress;
            }
            return ress;
        }

         public static string sendmailGreeting(string recp, string subj, string From)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/Greetings.html")))
            {
                body = reader.ReadToEnd();
            }
            ApplicationDbContext db = new ApplicationDbContext();
            string ress;

            //  MailMessage mm = new MailMessage("partner@inwinteck.com", recp);
            MailMessage mm = new MailMessage("support@inwinteck.com", recp);
            mm.Subject = subj;
            mm.IsBodyHtml = true;
            mm.Body = body;

            SmtpClient smtp = new SmtpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            smtp.Host = "smtp.office365.com";
            smtp.EnableSsl = true;
            // NetworkCredential NetworkCred = new NetworkCredential("partner@inwinteck.com", "Inwinteck@2577");
            NetworkCredential NetworkCred = new NetworkCredential("support@inwinteck.com", "Inwinteck@2577");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            //
            try
            {
                mm.From = new MailAddress(From);

                smtp.Send(mm);
                ress = "true";
                EmailLog EL = new EmailLog();
                EL.Emailid = recp;
                EL.Subject = subj;
                EL.Sent_On = DateTime.Now;
                EL.Status = "Send";
                db.EmailLogs.Add(EL);
                db.SaveChanges();

                string msg = "/Email/Log" + " " + "Recp: " + recp + " " + "subject: " + subj + " " + "From: " + From + " " + " Status: Success";

                utlity.createlog(msg);
            }
            catch (Exception ex)
            {
                ress = ex.ToString();
                string msg = "/Email/Log" + " " + "Recp: " + recp + " " + "subject: " + subj + " " + "From: " + From + " " + " Status: Error" + " Message: " + " " + ress;
                utlity.createlog(msg);

                EmailLog EL = new EmailLog();
                EL.Emailid = recp;
                EL.Subject = subj;
                EL.Sent_On = DateTime.Now;
                EL.Status = ress ;
                db.EmailLogs.Add(EL);
                db.SaveChanges();
            }
            return ress;
        }
        public static string TicketApiDetails(string Ticket_Number, string Email_Subject, string Case_No, string OEM, string TSE_Name, string Site_Name, string Street_address,string City, string Zip_Pin_Code, string Country, string State, string Dispatched_date, string Job_Description, string EU_Name, string EU_Contect, string EU_Email)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/TicketApi.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{Ticket_Number}", Ticket_Number);
            body = body.Replace("{Email_Subject}", Email_Subject);

            body = body.Replace("{Case_No}", Case_No);
            body = body.Replace("{TSE_Name}", TSE_Name);
            body = body.Replace("{OEM}", OEM);
            body = body.Replace("{Site_Name}", Site_Name);
            body = body.Replace("{Zip_Pin_Code}", Zip_Pin_Code);
            body = body.Replace("{Country}", Country);
            body = body.Replace("{City}", City);
            body = body.Replace("{State}", State);
            body = body.Replace("{Street_address}", Street_address);
            body = body.Replace("{Dispatched_date}", Dispatched_date);
            body = body.Replace("{Job_Description}", Job_Description);
            body = body.Replace("{EU_Name}", EU_Name);
            body = body.Replace("{EU_Contect}", EU_Contect);
            body = body.Replace("{EU_Email}", EU_Email);


            return body;
        }
        public static string FEInwinteckId(string country)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string InwinFEID;
            string cntcode;
            string serial;
            string fecnt;

            try
            {
                string fn = (from s in db.FE_Master_Personal where s.InwinFEID != null orderby s.Id descending select s.InwinFEID).FirstOrDefault();
                string fecnta = (from s in db.FE_Master_Personal where s.Country == country && s.InwinFEID != null orderby s.Id descending select s.InwinFEID).FirstOrDefault();
                if (fn != null)
                {
                    serial = fn.Substring(fn.Length - 5);
                    serial = (Convert.ToInt32(serial) + 1).ToString("D5");
                    if (fecnta != null)
                    {
                        string tn2 = fecnta.Remove(fecnta.Length - 5);
                        fecnt = tn2.Substring(tn2.Length - 4);
                        fecnt = (Convert.ToInt32(fecnt) + 1).ToString("D4");
                    }
                    else
                    {
                        fecnt = "0001";
                    }
                }
                else
                {
                    serial = "00001";
                    if (fecnta != null)
                    {
                        string tn2 = fecnta.Remove(fecnta.Length - 5);
                        fecnt = tn2.Substring(tn2.Length - 4);
                        fecnt = (Convert.ToInt32(fecnt) + 1).ToString("D4");
                    }
                    else
                    {
                        fecnt = "0001";
                    }
                }

                cntcode = (from s in db.Country where s.CountryName == country select s.CountryCode).FirstOrDefault();
                InwinFEID = "inwinteck" + cntcode + "partner" + fecnt + serial;

            }
            catch (Exception ex)
            {
                InwinFEID = "";
            }
            return InwinFEID;
        }

        public static string ChatLinkToUser(string Link)
        {

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/Chat/ChatLinkToUser.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Chat_Link}", Link);
            return body;
        }


        public static string SourceSupportResponse(string messageText, List<string> attachmentUrls ,string chatLink)
        {
            string body;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/Communication/SouceSupportEmailResponse.html")))
            {
                body = reader.ReadToEnd();
            }

            // Replace placeholders in the template with dynamic content
            body = body.Replace("@Model.MessageText", messageText ?? ""); // Handle null messageText
            body = body.Replace("{link}", chatLink);
            if (attachmentUrls != null && attachmentUrls.Count > 0)
            {
                string attachmentHtml = string.Join("", attachmentUrls.Select(url => $"<li class=\"attachment\"><a href=\"{url}\">{Path.GetFileName(url)}</a></li>"));
                body = body.Replace("@Model.Attachments", attachmentHtml);
            }
            else
            {
                body = body.Replace("@Model.Attachments", "<p>No attachments available.</p>");
            }

            return body;
        }

        public static string SendResponseLink(string SendResponseLink )
        {
            string body;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/Communication/SendResponseLink.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{LINK}", SendResponseLink);
            return body;
        }

        public static string SourceResponseAsEmail(string sourceResponse)
        {
            string body;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/MailTemp/Communication/SourceResponseAsEmail.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{sourceResponse}", sourceResponse);
            return body;
        }


    }

}