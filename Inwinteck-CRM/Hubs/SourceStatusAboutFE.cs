using Inwinteck_CRM.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inwinteck_CRM.Hubs
{
    public class SourceStatusAboutFE : Hub
    {
        public static void SendStatusUpdate(Ticket_Eu_Selection ticketEuSelection)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<SourceStatusAboutFE>();
            context.Clients.All.updateStatus(ticketEuSelection);
        }
    }
}
