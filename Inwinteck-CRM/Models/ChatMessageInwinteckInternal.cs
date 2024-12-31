using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Inwinteck_CRM.Models
{

    [Table("ChatMessageInwinteckInternal", Schema = "dbo")]
    public class ChatMessageInwinteckInternal
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public DateTime Timestamp { get; set; }

        public string MessageType { get; set; }

        public string SenderUserId { get; set; }  

        public string RecipientUserId { get; set; }


    }
}