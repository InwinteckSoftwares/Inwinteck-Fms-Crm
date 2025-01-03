using Inwinteck_CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM.viewModel
{
    public class TicketQualityViewModel
    {
        public Ticket Ticket { get; set; }
        public QualityMarksBraekdown QualityMarksBraekdown { get; set; }
        public QualityHotRequirement QualityHotRequirement { get; set; }
    }
}