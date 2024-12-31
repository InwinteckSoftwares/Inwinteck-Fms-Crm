using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inwinteck_CRM.Models
{
    public class UserConnection
    {
        [Key] // This marks the Id as the primary key
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string ConnectionId { get; set; }
    }

}
