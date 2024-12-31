using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM.Models
{
    [Table("QualityMarksBraekdown")]
    public class QualityMarksBraekdown
    {
        [Key]
        public int id { get; set; }

        public int? Ticket_id { get; set; }

        public string Greet_FE_Score { get; set; }

        public string GD_Name_And_Handover_Score { get; set; }

        public string Pregame_Manual_Site_Contact_Score { get; set; }

        public string Greet_TSE_Score { get; set; }

        public string Comm_Verif_Serial_Number_Score { get; set; }

        public string FE_Following_Instruction_Score { get; set; }

        public string Parts_Details_Score { get; set; }

        public string Closer_Form_Score { get; set; }

        public string Ticket_Creation_Score { get; set; }

        public string Thank_You_Score { get; set; }

        public string Total_Score { get; set; }
        public string Additional_Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        //------------handler----------------------//
        public string Handler1Name { get; set; }
        public int? Handler1Score { get; set; }
        public string Remark1 { get; set; }

        public string Handler2Name { get; set; }
        public int? Handler2Score { get; set; }
        public string Remark2 { get; set; }

        public string Handler3Name { get; set; }
        public int? Handler3Score { get; set; }
        public string Remark3 { get; set; }

        public string Handler4Name { get; set; }
        public int? Handler4Score { get; set; }
        public string Remark4 { get; set; }
    }

}