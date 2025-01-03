using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inwinteck_CRM.Models
{
    [Table("QualityHotRequirement")]
    public class QualityHotRequirement
    {
        [Key]
        public int Id { get; set; }
        public int Ticket_id { get; set; }
        [Range(0, 25, ErrorMessage = "Sourcing value must be between 0 and 25.")]
        public decimal? Sourcing { get; set; }

        [Range(0, 25, ErrorMessage = "Certification value must be between 0 and 25.")]
        public decimal? Certification { get; set; }


        [Range(0, 25, ErrorMessage = "Charges value must be between 0 and 25.")]
        public decimal? Charges { get; set; }

        [Range(0, 25, ErrorMessage = "Meeting SLA value must be between 0 and 25.")]
        public decimal? MeetingSLA { get; set; }

        [Range(0, 100, ErrorMessage = "Meeting SLA value must be between 0 and 100.")]
        public decimal? TotalHotRequirement { get; set; }
    
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsScoreUpdated { get; set; }
    }


}
