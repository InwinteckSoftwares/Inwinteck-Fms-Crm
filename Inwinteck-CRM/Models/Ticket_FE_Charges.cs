    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    namespace Inwinteck_CRM.Models
    {
        [Table("Ticket_FE_Charges")]
        public class Ticket_FE_Charges
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public int Ticket_Id { get; set; }

            [ForeignKey("Ticket_Id")]
            public virtual Ticket Ticket { get; set; }
            public string Currency { get; set; } 
            public int? FE_ID { get; set; }
            public bool? Travel_Charge_1 { get; set; }
            public double? Travel_Amount_1 { get; set; }
            public string Charge_Type_1 { get; set; }
            public double? Per_Hour_1 { get; set; }
            public double? Per_Job_1 { get; set; }
            public int? FE_ID_2 { get; set; }
            public bool? Travel_Charge_2 { get; set; }
            public double? Travel_Amount_2 { get; set; }
            public string Charge_Type_2 { get; set; }
            public double? Per_Hour_2 { get; set; }
            public double? Per_Job_2 { get; set; }
            public double? SentTravelCharge { get; set; }
            public double? SentTravelCharge2 { get; set; }

        }
    }