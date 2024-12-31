using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inwinteck_CRM.Models
{
    public class TicketData
    {
        public int Id { get; set; }
        public string Case_No { get; set; }
        public int OEM { get; set; }
        public string Zip_Pin_Code { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Street_Address { get; set; }
        public DateTime? Dispatch_Date { get; set; }
        public int FE_ID { get; set; }
        public int? SLA { get; set; }
        public string ScopeOfWork { get; set; }
        public DateTime? In_Time { get; set; }

        // Fields from Ticket_FE_Charges
        public decimal? Travel_Amount_1 { get; set; }
        public decimal? Travel_Amount_2 { get; set; }
        public decimal? Per_Hour_1 { get; set; }
        public decimal? Per_Job_1 { get; set; }
        public decimal? Per_Hour_2 { get; set; }
        public decimal? Per_Job_2 { get; set; }
        public DateTime? FE_Charges_Date { get; set; }

        // Fields for FE and FE2 Names
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string FE2_First_Name { get; set; }
        public string FE2_Last_Name { get; set; }
    }



}