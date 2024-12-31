using Inwinteck_CRM.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inwinteck_CRM.Hubs
{
    public class TicketNotificationHub : Hub
    {
        public static void NotifyNewTicket(Ticket newTicket)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TicketNotificationHub>();
            context.Clients.All.receiveNewTicketNotification(newTicket);
        }
    }
}
