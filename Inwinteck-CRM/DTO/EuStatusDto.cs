using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM.DTO
{
    public class EuStatusDto
    {
        public string Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string FE_Name { get; set; }
        public string Remark { get; set; }
    }
}