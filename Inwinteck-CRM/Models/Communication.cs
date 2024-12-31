using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM.Models
{
    [Table("Communication")]
    public class Communication
    {
        [Key]
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Username { get; set; }
        public string MessageText { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}