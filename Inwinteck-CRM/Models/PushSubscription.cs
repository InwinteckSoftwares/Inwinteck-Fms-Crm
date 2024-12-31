// Models/PushSubscription.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inwinteck_CRM.Models
{
    [Table("PushSubscriptions")]
    public class PushSubscription
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        public string Endpoint { get; set; }

        [Required]
        public string P256DH { get; set; }

        [Required]
        public string Auth { get; set; }

        [Required]
        public string UserId { get; set; } // Associates subscription with a user
    }
}
