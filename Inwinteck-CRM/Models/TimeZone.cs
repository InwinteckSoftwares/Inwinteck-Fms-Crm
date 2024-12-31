using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inwinteck_CRM.Models
{
    [Table("TimeZone")]
    public class TimeZone
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Ticket_Id { get; set; }

        public DateTime? CaseDtIndia { get; set; }
        public DateTime? CaseDtUs { get; set; }
        public DateTime? CaseDtLocal { get; set; }

        public DateTime? DispDtIndia { get; set; }
        public DateTime? DispDtUs { get; set; }
        public DateTime? DispDtLocal { get; set; }

        public DateTime? InTimeIndia { get; set; }
        public DateTime? InTimeUs { get; set; }
        public DateTime? InTimeLocal { get; set; }

        public DateTime? OutTimeIndia { get; set; }
        public DateTime? OutTimeUs { get; set; }
        public DateTime? OutTimeLocal { get; set; }

        public DateTime? InTimeIndia2 { get; set; }
        public DateTime? InTimeUs2 { get; set; }
        public DateTime? InTimeLocal2 { get; set; }

        public DateTime? OutTimeIndia2 { get; set; }
        public DateTime? OutTimeUs2 { get; set; }
        public DateTime? OutTimeLocal2 { get; set; }

        //[StringLength(100)]
        //public string SLA { get; set; }
        // Navigation property
        public ICollection<Ticket> Tickets { get; set; }
    }
}