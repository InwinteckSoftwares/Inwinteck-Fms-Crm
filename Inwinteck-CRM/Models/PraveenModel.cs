using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM.Models
{
    public class PraveenModel
    {

    }

    public class Contact
    { 
        public string comp { get; set; }
        public string name { get; set; }
        public string desig { get; set; }
        public string email { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string location { get; set; }
        public string mobile { get; set; }
        public string requirement { get; set; }
        public string msg { get; set; }
    }

    public class Career
    {
        public string name { get; set; }
        public string emailid { get; set; }
        public string mobile { get; set; }
        public string cover { get; set; }
        public string resume { get; set; }
        public string msg { get; set; }
    }

    public class Website_Carrer_Enq
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Resume { get; set; }
        public string Cover_Letter { get; set; }
        public DateTime Enq_Date { get; set; }
    }


}