using Inwinteck_CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM.viewModel
{
    public class ticketDetailsViewModel
    {
        public Ticket Ticket { get; set; }
        public EU_Master EU_Master { get; set; }
        public FE_Master_Personal FE_Master_Personal { get; set; }
        public  Ticket_History Ticket_History { get; set; }
        public string Ticket_Status { get; set; }
        public string OEM { get; set; }
        public string Reschedule { get; set; }
        public string cancellationReason { get; set; }
    }
}