using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM.Models
{
    public class TimeZoneApiResponse
    {
        public string timeZoneId { get; set; }
        public string timeZoneName { get; set; }
        public int dstOffset { get; set; }
        public int rawOffset { get; set; }
    }

}