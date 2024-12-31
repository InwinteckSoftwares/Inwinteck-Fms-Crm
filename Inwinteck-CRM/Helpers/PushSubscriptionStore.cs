using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Inwinteck_CRM.Models;

namespace Inwinteck_CRM.Helpers
{
    public static class PushSubscriptionStore
    {
        // Adds a new subscription to the database
        public static async Task AddSubscriptionAsync(PushSubscription subscription)
        {
            using (var context = new ApplicationDbContext())
            {
                if (!context.PushSubscriptions.Any(s => s.Endpoint == subscription.Endpoint))
                {
                    context.PushSubscriptions.Add(subscription);
                    await context.SaveChangesAsync();
                }
            }
        }

        // Retrieves all subscriptions from the database
        public static async Task<List<PushSubscription>> GetAllSubscriptionsAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                return await context.PushSubscriptions.ToListAsync();
            }
        }

        // Removes a subscription from the database
        public static async Task RemoveSubscriptionAsync(PushSubscription subscription)
        {
            using (var context = new ApplicationDbContext())
            {
                var existingSubscription = await context.PushSubscriptions
                    .FirstOrDefaultAsync(s => s.Endpoint == subscription.Endpoint);

                if (existingSubscription != null)
                {
                    context.PushSubscriptions.Remove(existingSubscription);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
