    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Http;
    using Inwinteck_CRM.App_Start;
using Serilog;

namespace Inwinteck_CRM
    {
        public class MvcApplication : System.Web.HttpApplication
        {
          protected void Application_Start()
{
    // Configure Serilog
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(logEvent => logEvent.Properties.ContainsKey("Action") &&
                                                 logEvent.Properties["Action"].ToString().Contains("CreateTicket"))
            .WriteTo.File(HttpContext.Current.Server.MapPath("~/App_Data/Logs/CreateTicket/log-.txt"), rollingInterval: RollingInterval.Day))
        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(logEvent => logEvent.Properties.ContainsKey("Action") &&
                                                 logEvent.Properties["Action"].ToString().Contains("EditTicket"))
            .WriteTo.File(HttpContext.Current.Server.MapPath("~/App_Data/Logs/EditTicket/log-.txt"), rollingInterval: RollingInterval.Day))
        .WriteTo.Console() // Optional for debugging
        .CreateLogger();

    Log.Information("Application Started");

    // Existing MVC setup
    AreaRegistration.RegisterAllAreas();
    GlobalConfiguration.Configure(WebApiConfig.Register);
    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
}


        //----For Js file versioning------------//
        protected void Application_BeginRequest()
        {
            HttpContext.Current.Items["Version"] = "1.0.3";
        }


        protected void Application_Error(object sender , EventArgs e)
        {
            Exception exception= Server.GetLastError();
            Log.Error(exception, "An Unhandled Exception occured");
        }

        protected void Application_End()
        {
            Log.CloseAndFlush();
        }
    }
}
