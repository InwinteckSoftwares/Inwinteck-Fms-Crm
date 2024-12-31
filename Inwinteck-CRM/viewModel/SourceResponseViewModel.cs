using Inwinteck_CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM.viewModel
{
    public class SourceResponseViewModel
    {
        public string TicketId { get; set; }
        public string Username { get; set; }
        public string MessageText { get; set; }
        public List<Communication> Communications { get; set; }
        public string EmailSubject { get; set; }
        public string MessageId { get; set; }
    }

}