using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Security;
using Inwinteck_CRM.Models;
using Inwinteck_CRM.viewModel;
using System.Web.Http;
using ClosedXML.Excel;
using Serilog;
using System.Net;

namespace Inwinteck_CRM.Controllers
{
    [System.Web.Http.Authorize]
    public class ReportController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext db = new ApplicationDbContext();

        public ReportController()
        {
        }

        public ReportController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult FEReport(int pageNo = 0, string Country = "", string City = "", DateTime? dorFrom = null, DateTime? dorTo = null, string feFristName = "", string feLastName = "")
        {
            int pageSize = 10;
            int pgc = 0;
            int pageCount = 0;
            List<FE_Master_Personal> li = new List<FE_Master_Personal>();


            if (Country != "" || City != "" || dorFrom != null || dorTo != null || feFristName != "" || feLastName != "")
            {
                if (Country != "")
                {
                    if (City != "" && dorFrom != null && dorTo != null && feFristName != "" && feLastName != "")
                    {
                        
                        pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country) 
                        && x.City.Contains(City) && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo && x.First_Name.Contains(feFristName) && 
                        x.Last_Name.Contains(feLastName)).OrderByDescending(x => x.Id).Count();
                        pageCount = pgc / pageSize;
                        li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City) && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo && x.First_Name.Contains(feFristName) &&
                        x.Last_Name.Contains(feLastName)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.City = City;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                        ViewBag.feFristName = feFristName;
                        ViewBag.feLastName = feLastName;
                    }
                    else if (City != "" && dorFrom != null && dorTo != null && feFristName != "")
                    {

                        pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City) && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo && x.First_Name.Contains(feFristName)).OrderByDescending(x => x.Id).Count();
                        pageCount = pgc / pageSize;
                        li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City) && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo && x.First_Name.Contains(feFristName)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.City = City;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                        ViewBag.feFristName = feFristName;
                    }
                    else if (City != "" && dorFrom != null && dorTo != null)
                    {

                        pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City) && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo).OrderByDescending(x => x.Id).Count();
                        pageCount = pgc / pageSize;
                        li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City) && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.City = City;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                    }
                    else if (City != "")
                    {

                        pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City)).OrderByDescending(x => x.Id).Count();
                        pageCount = pgc / pageSize;
                        li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.City = City;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                    }
                    else if (dorFrom != null)
                    {

                        pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo).OrderByDescending(x => x.Id).Count();
                        pageCount = pgc / pageSize;
                        li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                    }
                    else if (feFristName != "")
                    {

                        pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.First_Name.Contains(feFristName)).OrderByDescending(x => x.Id).Count();
                        pageCount = pgc / pageSize;
                        li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.First_Name.Contains(feFristName)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.feFristName = feFristName;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                    }
                    else if (feLastName != "")
                    {

                        pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.Last_Name.Contains(feLastName)).OrderByDescending(x => x.Id).Count();
                        pageCount = pgc / pageSize;
                        li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.Last_Name.Contains(feLastName)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.feLastName = feLastName;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                    }
                    else
                    {
                        pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)).OrderByDescending(x => x.Id).Count();
                        pageCount = pgc / pageSize;
                        li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                    }
                   
                }
                else if (City != "")
                {
                    pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.City.Contains(City)).OrderByDescending(x => x.Id).Count();
                    pageCount = pgc / pageSize;
                    li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.City.Contains(City)).OrderByDescending(x => x.Id).ToList();
                    ViewBag.Country = Country;
                    ViewBag.City = City;
                    ViewBag.dorFrom = dorFrom.ToString();
                    ViewBag.dorTo = dorTo.ToString();
                    ViewBag.feFristName = feFristName;
                    ViewBag.feLastName = feLastName;
                }
                else if (dorFrom != null)
                {
                    pgc = (from s in db.FE_Master_Personal where s.CreatedOn >= dorFrom && s.CreatedOn <= dorTo orderby s.Id descending select s).OrderByDescending(x => x.Id).Count();
                    pageCount = pgc / pageSize;
                    li = (from s in db.FE_Master_Personal where s.CreatedOn >= dorFrom && s.CreatedOn <= dorTo orderby s.Id descending select s).OrderByDescending(x => x.Id).ToList();
                    ViewBag.Country = Country;
                    ViewBag.City = City;
                    ViewBag.dorFrom = dorFrom.ToString();
                    ViewBag.dorTo = dorTo.ToString();
                    ViewBag.feFristName = feFristName;
                    ViewBag.feLastName = feLastName;
                }
                else if (feFristName != "")
                {
                    pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.First_Name.Contains(feFristName)).OrderByDescending(x => x.Id).Count();
                    pageCount = pgc / pageSize;
                    li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.First_Name.Contains(feFristName)).OrderByDescending(x => x.Id).ToList();
                    ViewBag.Country = Country;
                    ViewBag.City = City;
                    ViewBag.dorFrom = dorFrom.ToString();
                    ViewBag.dorTo = dorTo.ToString();
                    ViewBag.feFristName = feFristName;
                    ViewBag.feLastName = feLastName;
                }
                else if (feLastName != "")
                {
                    pgc = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Last_Name.Contains(feLastName)).OrderByDescending(x => x.Id).Count();
                    pageCount = pgc / pageSize;
                    li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Last_Name.Contains(feLastName)).OrderByDescending(x => x.Id).ToList();
                    ViewBag.Country = Country;
                    ViewBag.City = City;
                    ViewBag.dorFrom = dorFrom.ToString();
                    ViewBag.dorTo = dorTo.ToString();
                    ViewBag.feFristName = feFristName;
                    ViewBag.feLastName = feLastName;
                }
            }
            else
            {
                pgc = db.FE_Master_Personal.Count();
                pageCount = pgc / pageSize;

                li = (from s in db.FE_Master_Personal orderby s.Id descending select s).Skip(pageNo * pageSize).Take(pageSize).ToList();
                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }
          
            return View(li);
        }


    //[System.Web.Http.Authorize(Roles = "Admin")]
    //public async Task<ActionResult> TicketReport()
    //{
    //    var ticketDetails = new List<ticketDetailsViewModel>();

    //    // Fetch data in parallel using separate DbContext instances
    //    // This ensures thread safety and allows us to fetch data concurrently.
    //    var ticketDataListTask = GetTicketsAsync();
    //    var ticketHistoryListTask = GetTicketHistoriesAsync();
    //    var euMasterTask = GetEUMastersAsync();
    //    var feMasterPersonalTask = GetFEMastersAsync();
    //    var headerDescriptionsTask = GetHeaderDescriptionsAsync();

    //    // Wait for all the tasks to complete
    //    await Task.WhenAll(ticketDataListTask, ticketHistoryListTask, euMasterTask, feMasterPersonalTask, headerDescriptionsTask);

    //    // Retrieve the results from the tasks
    //    var ticketDataList = await ticketDataListTask;
    //    var ticketHistoryList = await ticketHistoryListTask;
    //    var euMasters = await euMasterTask;
    //    var feMasters = await feMasterPersonalTask;
    //    var headerDescriptions = await headerDescriptionsTask;

    //    // Create dictionaries for quick lookup of related data
    //    var ticketHistoryDict = ticketHistoryList.ToDictionary(th => th.Ticket_no);
    //    var euMasterDict = euMasters.ToDictionary(eu => eu.Id);
    //    var feMasterDict = feMasters.ToDictionary(fe => fe.Id);
    //    var headerDescriptionDict = headerDescriptions.ToDictionary(hd => hd.id);

    //    // Iterate through the list of tickets and populate the ticket details view model
    //    foreach (var ticketData in ticketDataList)
    //    {
    //        // Try to get the ticket history for the current ticket
    //        ticketHistoryDict.TryGetValue(ticketData.Id, out var ticketHistory);

    //        // Try to get the customer name from the EU_Master table
    //        var customerName = euMasterDict.TryGetValue(ticketData.EU_ID, out var euMaster) ? euMaster.Customer_Name : null;

    //        // Try to get the Field Engineer details from the FE_Master_Personal table
    //        var feMasterPersonal = feMasterDict.TryGetValue(ticketData.FE_ID, out var fePersonal) ? fePersonal : null;

    //        // Try to get the status description from the HeaderDescription table
    //        var ticketStatus = headerDescriptionDict.TryGetValue(ticketData.Status, out var statusData) ? statusData.header_description : null;

    //        // Try to get the OEM description from the HeaderDescription table
    //        var oemValue = headerDescriptionDict.TryGetValue(ticketData.OEM, out var oemData) ? oemData.header_description : null;

    //        // Try to get the cancellation reason description from the HeaderDescription table
    //        var cancellationReason = headerDescriptionDict.TryGetValue(ticketData.Cancel_Reason ?? 0, out var cancellationData) ? cancellationData.header_description : null;

    //        // Calculate the total working hours if both check-in and check-out times are available
    //        DateTime? checkedInTime = ticketData.In_Time;
    //        DateTime? checkedOutTime = ticketData.Out_Time;
    //        TimeSpan? totalWorkingHours = checkedInTime.HasValue && checkedOutTime.HasValue ? checkedOutTime - checkedInTime : (TimeSpan?)null;

    //        // Create the Ticket model with the necessary data
    //        var ticket = new Ticket
    //        {
    //            Ticket_No = ticketData.Ticket_No,
    //            Case_No = ticketData.Case_No,
    //            Country = ticketData.Country,
    //            City = ticketData.City,
    //            Dispatch_Date = ticketData.Dispatch_Date,
    //            In_Time = checkedInTime,
    //            Out_Time = checkedOutTime,
    //            Total_Hours = totalWorkingHours?.ToString(@"hh\:mm"),
    //            CreatedOn = ticketData.CreatedOn,
    //            Cancel_Reason = ticketData.Cancel_Reason
    //        };

    //        // Create the EU_Master model
    //        var euMasterModel = new EU_Master
    //        {
    //            Customer_Name = customerName,
    //        };

    //        // Create the FE_Master_Personal model
    //        var feMasterModel = new FE_Master_Personal
    //        {
    //            First_Name = feMasterPersonal?.First_Name,
    //            Last_Name = feMasterPersonal?.Last_Name
    //        };

    //        // Create the Ticket_History model
          
    //        // Populate the ticketDetailsViewModel with the gathered data
    //        var ticketValues = new ticketDetailsViewModel
    //        {
    //            Ticket = ticket,
    //            EU_Master = euMasterModel,
    //            FE_Master_Personal = feMasterModel,
    //            Ticket_Status = ticketStatus,
    //            OEM = oemValue,
    //            cancellationReason = cancellationReason
    //        };

    //        // Add the populated view model to the list
    //        ticketDetails.Add(ticketValues);
    //    }

    //    // Return the view with the ticket details
    //    return View(ticketDetails);
    //}

    // Method to fetch ticket data with a new DbContext instance
    private async Task<List<Ticket>> GetTicketsAsync()
    {
        using (var context = new ApplicationDbContext())
        {
            return await context.Ticket.OrderByDescending(t => t.CreatedOn).ToListAsync();
        }
    }

    // Method to fetch ticket history data with a new DbContext instance
    private async Task<List<Ticket_History>> GetTicketHistoriesAsync()
    {
        using (var context = new ApplicationDbContext())
        {
            return await context.Ticket_History
                .GroupBy(th => th.Ticket_no)
                .Select(g => g.OrderByDescending(th => th.CreatedOn).FirstOrDefault())
                .ToListAsync();
        }
    }

    // Method to fetch EU_Master data with a new DbContext instance
    private async Task<List<EU_Master>> GetEUMastersAsync()
    {
        using (var context = new ApplicationDbContext())
        {
            return await context.EU_Master.ToListAsync();
        }
    }

    // Method to fetch FE_Master_Personal data with a new DbContext instance
    private async Task<List<FE_Master_Personal>> GetFEMastersAsync()
    {
        using (var context = new ApplicationDbContext())
        {
            return await context.FE_Master_Personal.ToListAsync();
        }
    }

    // Method to fetch HeaderDescription data with a new DbContext instance
    private async Task<List<HeaderDescription>> GetHeaderDescriptionsAsync()
    {
        using (var context = new ApplicationDbContext())
        {
            return await context.HeaderDescription.ToListAsync();
        }
    }



    public ActionResult exportFEReport(int pageNo = 0, string Country = "", string City = "", DateTime? dorFrom = null, DateTime? dorTo = null, string feFristName = "", string feLastName = "")
        {
            var gv = new GridView();
            List<FE_Master_Personal> li = new List<FE_Master_Personal>();

            if (Country != "" || City != "" || dorFrom != null || dorTo != null || feFristName != "" || feLastName != "")
            {
                if (Country != "")
                {
                    if (City != "" && dorFrom != null && dorTo != null && feFristName != "" && feLastName != "")
                    {

                        gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City) && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo && x.First_Name.Contains(feFristName) &&
                        x.Last_Name.Contains(feLastName)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.City = City;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                        ViewBag.feFristName = feFristName;
                        ViewBag.feLastName = feLastName;
                    }
                    else if (City != "" && dorFrom != null && dorTo != null && feFristName != "")
                    {
                        gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City) && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo && x.First_Name.Contains(feFristName)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.City = City;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                        ViewBag.feFristName = feFristName;
                    }
                    else if (City != "" && dorFrom != null && dorTo != null)
                    {
                        gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City) && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.City = City;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                    }
                    else if (City != "")
                    {

                        gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.City.Contains(City)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.City = City;
                    }
                    else if (dorFrom != null)
                    {

                        gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.CreatedOn >= dorFrom && x.CreatedOn <= dorTo).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.dorFrom = dorFrom.ToString();
                        ViewBag.dorTo = dorTo.ToString();
                    }
                    else if (feFristName != "")
                    {

                        gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.First_Name.Contains(feFristName)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.feFristName = feFristName;
                    }
                    else if (feLastName != "")
                    {
                        gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)
                        && x.Last_Name.Contains(feLastName)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                        ViewBag.feLastName = feLastName;
                    }
                    else
                    {
                        gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Country.Contains(Country)).OrderByDescending(x => x.Id).ToList();
                        ViewBag.Country = Country;
                    }

                }
                else if (City != "")
                {
                    gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.City.Contains(City)).OrderByDescending(x => x.Id).ToList();
                    ViewBag.Country = Country;
                    ViewBag.City = City;
                    ViewBag.dorFrom = dorFrom.ToString();
                    ViewBag.dorTo = dorTo.ToString();
                    ViewBag.feFristName = feFristName;
                    ViewBag.feLastName = feLastName;
                }
                else if (dorFrom != null)
                {
                    gv.DataSource = (from s in db.FE_Master_Personal where s.CreatedOn >= dorFrom && s.CreatedOn <= dorTo orderby s.Id descending select s).OrderByDescending(x => x.Id).ToList();
                    ViewBag.Country = Country;
                    ViewBag.City = City;
                    ViewBag.dorFrom = dorFrom.ToString();
                    ViewBag.dorTo = dorTo.ToString();
                    ViewBag.feFristName = feFristName;
                    ViewBag.feLastName = feLastName;
                }
                else if (feFristName != "")
                {
                    gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.First_Name.Contains(feFristName)).OrderByDescending(x => x.Id).ToList();
                    ViewBag.Country = Country;
                    ViewBag.City = City;
                    ViewBag.dorFrom = dorFrom.ToString();
                    ViewBag.dorTo = dorTo.ToString();
                    ViewBag.feFristName = feFristName;
                    ViewBag.feLastName = feLastName;
                }
                else if (feLastName != "")
                {
                    gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).Where(x => x.Last_Name.Contains(feLastName)).OrderByDescending(x => x.Id).ToList();
                    ViewBag.Country = Country;
                    ViewBag.City = City;
                    ViewBag.dorFrom = dorFrom.ToString();
                    ViewBag.dorTo = dorTo.ToString();
                    ViewBag.feFristName = feFristName;
                    ViewBag.feLastName = feLastName;
                }
            }
            else
            {

                gv.DataSource = (from s in db.FE_Master_Personal orderby s.Id descending select s).ToList();
            }
            
            
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=FEMaster.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("FEReport", "Report" ,new { Country = Country,City=City,dorFrom=dorFrom,dorTo=dorTo,feFristName=feFristName,feLastName=feLastName});
        }

        [System.Web.Http.HttpPost]
        public ActionResult DownloadExcel([System.Web.Http.FromBody] DateTime date)
        {
            try
            {
                // Log entry to the method
                Log.Information("DownloadExcel called with date: {Date}", date);

                // Validate input date
                if (date == default(DateTime))
                {
                    Log.Warning("DownloadExcel received an invalid date: {Date}", date);
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid date parameter.");
                }

                // Retrieve ticket data
                var ticketData = db.Ticket
                    .AsEnumerable()
                    .Where(t => t.CreatedOn.Date == date.Date)
                    .Join(
                        db.TicketFeCharges,
                        ticket => ticket.Id,
                        feCharge => feCharge.Ticket_Id,
                        (ticket, feCharge) => new { ticket, feCharge }
                    )
                    .Join(
                        db.FE_Master_Personal, // Join for primary FE
                        combined => combined.ticket.FE_ID,
                        fe => fe.Id,
                        (combined, fe) => new { combined.ticket, combined.feCharge, fe, FE2_Id = combined.ticket.FE_ID_2 }
                    )
                    .GroupJoin(
                        db.FE_Master_Personal, // Join for FE2
                        combined => combined.FE2_Id,
                        fe2 => fe2.Id,
                        (combined, fe2) => new { combined.ticket, combined.feCharge, combined.fe, FE2 = fe2.FirstOrDefault() }
                    )
                    .Select(data => new TicketData
                    {
                        Case_No = data.ticket.Case_No,
                        Zip_Pin_Code = data.ticket.Zip_Pin_Code,
                        Country = data.ticket.Country,
                        City = data.ticket.City,
                        State = data.ticket.State,
                        Street_Address = data.ticket.Street_Address,
                        Dispatch_Date = data.ticket.Dispatch_Date,
                        SLA = data.ticket.SLA,
                        ScopeOfWork = data.ticket.Job_Description,

                        // Ticket_FE_Charges fields
                        Travel_Amount_1 = (decimal?)data.feCharge.Travel_Amount_1,
                        Travel_Amount_2 = (decimal?)data.feCharge.Travel_Amount_2,
                        Per_Hour_1 = (decimal?)data.feCharge.Per_Hour_1,
                        Per_Job_1 = (decimal?)data.feCharge.Per_Job_1,
                        Per_Hour_2 = (decimal?)data.feCharge.Per_Hour_2,
                        Per_Job_2 = (decimal?)data.feCharge.Per_Job_2,

                        // FE Name
                        First_Name = data.fe.First_Name,
                        Last_Name = data.fe.Last_Name,

                        // FE2 Name
                        FE2_First_Name = data.FE2?.First_Name,
                        FE2_Last_Name = data.FE2?.Last_Name
                    })
                    .ToList();

                if (!ticketData.Any())
                {
                    Log.Information("No data found for the given date: {Date}", date);
                    return new HttpStatusCodeResult(HttpStatusCode.NoContent, "No data available for the selected date.");
                }

                // Generate Excel file
                var excelFile = GenerateExcelFile(ticketData);
                Log.Information("Excel file generated successfully for date: {Date}, Records: {RecordCount}", date, ticketData.Count);

                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TicketData.xlsx");
            }
            catch (Exception ex)
            {
                // Log exception details
                Log.Error(ex, "An error occurred in DownloadExcel for date: {Date}", date);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while generating the Excel file.");
            }
        }


        public byte[] GenerateExcelFile(List<TicketData> ticketData)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Tickets");

                // Add headers to the Excel sheet
                worksheet.Cell(1, 1).Value = "Case No";
                worksheet.Cell(1, 2).Value = "Zip/Pin Code";
                worksheet.Cell(1, 3).Value = "Country";
                worksheet.Cell(1, 4).Value = "City";
                worksheet.Cell(1, 5).Value = "State";
                worksheet.Cell(1, 6).Value = "Street Address";
                worksheet.Cell(1, 7).Value = "Dispatch Date";
                worksheet.Cell(1, 8).Value = "In Time"; 
                worksheet.Cell(1, 9).Value = "FE Name";
                worksheet.Cell(1, 10).Value = "FE 2";
                worksheet.Cell(1, 11).Value = "SLA";
                worksheet.Cell(1, 12).Value = "Scope of Work";
                worksheet.Cell(1, 13).Value = "Travel Amount 1";
                worksheet.Cell(1, 14).Value = "Travel Amount 2";
                worksheet.Cell(1, 15).Value = "Per Hour 1";
                worksheet.Cell(1, 16).Value = "Per Job 1";
                worksheet.Cell(1, 17).Value = "Per Hour 2";
                worksheet.Cell(1, 18).Value = "Per Job 2";

                // Populate rows with ticket data
                for (int i = 0; i < ticketData.Count; i++)
                {
                    var row = i + 2; 

                    worksheet.Cell(row, 1).Value = ticketData[i].Case_No;
                    worksheet.Cell(row, 2).Value = ticketData[i].Zip_Pin_Code;
                    worksheet.Cell(row, 3).Value = ticketData[i].Country;
                    worksheet.Cell(row, 4).Value = ticketData[i].City;
                    worksheet.Cell(row, 5).Value = ticketData[i].State;
                    worksheet.Cell(row, 6).Value = ticketData[i].Street_Address;
                    worksheet.Cell(row, 7).Value = ticketData[i].Dispatch_Date;
                    worksheet.Cell(row, 8).Value = ticketData[i].In_Time; // Populate In_Time
                    worksheet.Cell(row, 9).Value = $"{ticketData[i].First_Name} {ticketData[i].Last_Name}";
                    worksheet.Cell(row, 10).Value = $"{ticketData[i].FE2_First_Name} {ticketData[i].FE2_Last_Name}";
                    worksheet.Cell(row, 11).Value = GetSLADescription(ticketData[i].SLA);
                    worksheet.Cell(row, 12).Value = ticketData[i].ScopeOfWork;
                    worksheet.Cell(row, 13).Value = ticketData[i].Travel_Amount_1;
                    worksheet.Cell(row, 14).Value = ticketData[i].Travel_Amount_2;
                    worksheet.Cell(row, 15).Value = ticketData[i].Per_Hour_1;
                    worksheet.Cell(row, 16).Value = ticketData[i].Per_Job_1;
                    worksheet.Cell(row, 17).Value = ticketData[i].Per_Hour_2;
                    worksheet.Cell(row, 18).Value = ticketData[i].Per_Job_2;

                    // Calculate time difference and apply conditional formatting
                    if (ticketData[i].Dispatch_Date.HasValue && ticketData[i].In_Time.HasValue)
                    {
                        var timeDifference = ticketData[i].In_Time.Value - ticketData[i].Dispatch_Date.Value;
                        if (timeDifference.TotalMinutes > 15)
                        {
                            // Apply red fill to SLA cell if late
                            worksheet.Cell(row, 12).Style.Fill.BackgroundColor = XLColor.Red;
                        }
                    }
                }

                // Auto-fit the columns for better readability
                worksheet.Columns().AdjustToContents();

                // Save the workbook to a memory stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray(); // Return the stream as a byte array
                }
            }
        }




        private string GetSLADescription(int? sla)
        {
            switch (sla)
            {
                case 1357:
                    return "2hrs";
                case 1358:
                    return "4hrs";
                case 1359:
                    return "8hrs";
                case 1360:
                    return "NBD";
                case 1361:
                    return "Scheduled";
                case 1598:
                    return "Same Day";
                default:
                    return "Unknown"; 
            }
        }

    }
}