using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM.Models
{
    [Table("ChatMessage", Schema = "dbo")]
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string TicketId { get; set; }
        public string MessageType { get; set; }

    }
}