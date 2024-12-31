using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM.Models
{
    public class Ticket_Eu_Selection
    {
        public int Id { get; set; }
        public int Ticket_no { get; set; }
        public int Eu_ID { get; set; }
        public int Fe_Id { get; set; }
        public int? Fe_Id_2 { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public string Remark { get; set; }
    }
}