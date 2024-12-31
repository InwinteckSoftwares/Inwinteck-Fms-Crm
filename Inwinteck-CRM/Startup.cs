using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;

[assembly: OwinStartup(typeof(Inwinteck_CRM.Startup))]
namespace Inwinteck_CRM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
    {
        // Call the ConfigureAuth method
        ConfigureAuth(app);

        // Allow cross-origin requests (CORS)
        app.UseCors(CorsOptions.AllowAll);

        // Create a HubConfiguration instance to apply custom settings
        var hubConfiguration = new HubConfiguration
        {
            EnableDetailedErrors = true // Enable detailed errors for easier debugging
        };

        // Set global configuration options
        GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(40); // Default is 110 seconds
        GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(30); // Default is 30 seconds
        GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(9); // Default is 09 seconds

        // Map SignalR with the custom HubConfiguration
        app.MapSignalR("/signalr", hubConfiguration);
    }
}
}