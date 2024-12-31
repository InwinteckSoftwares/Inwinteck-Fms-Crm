using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inwinteck_CRM.Models
{
    public class generalresponse
    {
        public int statuscode { get; set; }
        public string statusmessage { get; set; }
    }

    public class apiTicket
    {
        public string Email_Subject { get; set; }
        public string Case_No { get; set; }
        public string OEM { get; set; }
        public string Site_Name { get; set; }
        public string Zip_Pin_Code { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Street_Address { get; set; }
       
        public DateTime Dispatch_Date { get; set; }
        public string Job_Description { get; set; }
        public string EU_Name { get; set; }
        public string EU_Contact { get; set; }
        public string EU_Email { get; set; }
        public string Certification_Name { get; set; }
        public string TSE_Name { get; set; }
    }

    public class apiTicketUpdate
    {
        public string Case_No { get; set; }
        public string OEM { get; set; }
        public string Site_Name { get; set; }
        public string Zip_Pin_Code { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Street_Address { get; set; }
        public DateTime Dispatch_Date { get; set; }
        public string Job_Description { get; set; }
        public string EU_Name { get; set; }
        public string EU_Contact { get; set; }
        public string EU_Email { get; set; }
        public string Certification_Name { get; set; }

        public string Ticket_No { get; set; }

        public string Comments { get; set; }
    }
}