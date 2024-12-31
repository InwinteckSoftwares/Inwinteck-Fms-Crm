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
using System.Web.Script.Serialization;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net;
using System.Web.Security;
using Inwinteck_CRM.Models;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using System.Data;
using System.Configuration;
using System.Web.Routing;

namespace Inwinteck_CRM.Controllers
{
    [Authorize]
    public class MasterController : Controller
    {
        // GET: Master

        //string url = "http://localhost:1957/Upload/";
        string url = "https://fms.inwinteck.com/Upload/";
        //  string url = "http://inwinteckcrm.3sptechmind.com/Upload/";
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext db = new ApplicationDbContext();

        public MasterController()
        {
        }

        public MasterController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        //-------------- Country Master Start -------------------------
        public ActionResult Country(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
            // To check user access right
            string uri1 = Request.Url.AbsolutePath;

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            ViewBag.Permission = cm;
            //End user access right

            List<CountryMaster> CM = new List<CountryMaster>();

            int pageSize = 10;
            int pgc = 0;
            int pageCount = 0;
            if (searchtype != "" && searchtext != "")
            {
                if (searchtype == "country")
                {
                    pgc = db.Country.Where(x => x.CountryName.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    CM = (from c in db.Country
                          orderby c.CountryName
                          select c).Where(x => x.CountryName.StartsWith(searchtext)).ToList();
                }
                ViewBag.Search = searchtext;
            }
            else
            {
                pgc = db.Country.Count();
                pageCount = pgc / pageSize;

                CM = (from s in db.Country orderby s.CountryName select s).Skip(pageNo * pageSize).Take(pageSize).ToList();
                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }

            return View(CM);
        }


        public ActionResult addCountry()
        {
            // To check user access right
            string uri1 = "/Master/Country";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addCountry(CountryMaster contM)
        {
            try
            {
                if (contM != null)
                {
                    contM.CreateDate = DateTime.Now;
                    contM.CreatedBy = User.Identity.GetUserId();
                    db.Country.Add(contM);
                    db.SaveChanges();
                    TempData["message"] = "Data Saved !!";
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("addCountry", "Master");
        }
        public ActionResult editCountry(int Id)
        {

            // To check user access right
            string uri1 = "/Master/Country";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            CountryMaster CM = new CountryMaster();
            CM = (from s in db.Country where s.Id == Id select s).FirstOrDefault();
            return View(CM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult editCountry(CountryMaster CM)
        {
            try
            {
                if (CM != null)
                {
                    CM.ModifiedBy = User.Identity.GetUserId();
                    CM.ModifiedOn = DateTime.Now;
                    db.Entry(CM).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["message"] = "Data Saved !!";
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return View(CM);
        }

        public ActionResult statusCountry(int id)
        {
            // To check user access right
            string uri1 = "/Master/Country";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            CountryMaster sa = new CountryMaster();
            sa = (from s in db.Country where s.Id == id select s).FirstOrDefault();
            if (sa.Status == 1)
            {
                sa.Status = 0;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            else
            {
                sa.Status = 1;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.Message = string.Format("Deactivated");
            return RedirectToAction("Country", "Master");
        }
        public ActionResult viewCountry(int Id)
        {
            // To check user access right
            string uri1 = "/Master/Country";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_view == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            CountryMaster CM = new CountryMaster();
            CM = (from s in db.Country where s.Id == Id select s).FirstOrDefault();
            return View(CM);
        }

        //-------------- Country Master End -------------------------

        //===================== Country Dial Code ------------------------------
        public ActionResult CountryCode(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
            // To check user access right
            string uri1 = "/Master/CountryCode";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            ViewBag.Permission = cm;
            // End user access right
            List<Country_Dialing_Code> CM = new List<Country_Dialing_Code>();

            int pageSize = 10;
            int pgc = 0;
            int pageCount = 0;
            if (searchtype != "" && searchtext != "")
            {
                if (searchtype == "country")
                {
                    pgc = db.Country_Dialing_Code.Where(x => x.Country.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    CM = (from c in db.Country_Dialing_Code
                          orderby c.Country
                          select c).Where(x => x.Country.StartsWith(searchtext)).ToList();
                }
                ViewBag.Search = searchtext;
            }
            else
            {
                pgc = db.Country_Dialing_Code.Count();
                pageCount = pgc / pageSize;

                CM = (from s in db.Country_Dialing_Code orderby s.Country select s).Skip(pageNo * pageSize).Take(pageSize).ToList();
                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }

            return View(CM);
        }


        public ActionResult addCountryCode()
        {
            // To check user access right
            string uri1 = "/Master/CountryCode";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            List<SelectListItem> country = new List<SelectListItem>();
            country = (from c in db.Country where c.Status == 1 orderby c.CountryName select new SelectListItem { Text = c.CountryName, Value = c.CountryName }).ToList();
            ViewBag.country = country;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addCountryCode(Country_Dialing_Code contM)
        {
            try
            {
                if (contM != null)
                {
                    contM.CreatedOn = DateTime.Now;
                    contM.CreatedBy = User.Identity.GetUserId();
                    db.Country_Dialing_Code.Add(contM);
                    db.SaveChanges();
                    TempData["message"] = "Data Saved !!";
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("addCountryCode", "Master");
        }
        public ActionResult editCountryCode(int Id)
        {

            // To check user access right
            string uri1 = "/Master/CountryCode";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            Country_Dialing_Code CM = new Country_Dialing_Code();
            CM = (from s in db.Country_Dialing_Code where s.Id == Id select s).FirstOrDefault();
            List<SelectListItem> country = new List<SelectListItem>();
            country = (from c in db.Country where c.Status == 1 orderby c.CountryName select new SelectListItem { Text = c.CountryName, Value = c.CountryName }).ToList();
            ViewBag.country = country;
            return View(CM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult editCountryCode(Country_Dialing_Code CM)
        {
            try
            {
                if (CM != null)
                {
                    CM.ModifiedBy = User.Identity.GetUserId();
                    CM.ModifiedOn = DateTime.Now;
                    db.Entry(CM).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["message"] = "Data Saved !!";
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("editCountryCode", "Master", new { Id = CM.Id });
        }

        public ActionResult statusCountryCode(int id)
        {
            // To check user access right
            string uri1 = "/Master/CountryCode";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            Country_Dialing_Code sa = new Country_Dialing_Code();
            sa = (from s in db.Country_Dialing_Code where s.Id == id select s).FirstOrDefault();
            if (sa.Status == 1)
            {
                sa.Status = 0;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            else
            {
                sa.Status = 1;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.Message = string.Format("Deactivated");
            return RedirectToAction("CountryCode", "Master");
        }
        public ActionResult viewCountryCode(int Id)
        {
            // To check user access right
            string uri1 = "/Master/CountryCode";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_view == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            Country_Dialing_Code CM = new Country_Dialing_Code();
            CM = (from s in db.Country_Dialing_Code where s.Id == Id select s).FirstOrDefault();
            List<SelectListItem> country = new List<SelectListItem>();
            country = (from c in db.Country where c.Status == 1 orderby c.CountryName select new SelectListItem { Text = c.CountryName, Value = c.CountryName }).ToList();
            ViewBag.country = country;
            return View(CM);
        }
        //----------------------- Country Dial Code Ends ---------------------------
        //===================== Country Currency ------------------------------
        public ActionResult CountryCurrency(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
            // To check user access right
            string uri1 = "/Master/CountryCurrency";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            ViewBag.Permission = cm;
            // End user access right
            List<Currency_Master> CM = new List<Currency_Master>();

            int pageSize = 10;
            int pgc = 0;
            int pageCount = 0;
            if (searchtype != "" && searchtext != "")
            {
                if (searchtype == "country")
                {
                    pgc = db.Currency_Master.Where(x => x.Country.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    CM = (from c in db.Currency_Master
                          orderby c.Country
                          select c).Where(x => x.Country.StartsWith(searchtext)).ToList();
                }
                ViewBag.Search = searchtext;
            }
            else
            {
                pgc = db.Currency_Master.Count();
                pageCount = pgc / pageSize;

                CM = (from s in db.Currency_Master orderby s.Country select s).Skip(pageNo * pageSize).Take(pageSize).ToList();
                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }

            return View(CM);
        }


        public ActionResult addCountryCurrency()
        {
            // To check user access right
            string uri1 = "/Master/CountryCurrency";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            List<SelectListItem> country = new List<SelectListItem>();
            country = (from c in db.Country where c.Status == 1 orderby c.CountryName select new SelectListItem { Text = c.CountryName, Value = c.CountryName }).ToList();
            ViewBag.country = country;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addCountryCurrency(Currency_Master contM)
        {
            try
            {
                if (contM != null)
                {
                    contM.CreatedOn = DateTime.Now;
                    contM.CreatedBy = User.Identity.GetUserId();
                    db.Currency_Master.Add(contM);
                    db.SaveChanges();
                    TempData["message"] = "Data Saved !!";
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("addCountryCurrency", "Master");
        }
        public ActionResult editCountryCurrency(int Id)
        {

            // To check user access right
            string uri1 = "/Master/CountryCurrency";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            Currency_Master CM = new Currency_Master();
            CM = (from s in db.Currency_Master where s.Id == Id select s).FirstOrDefault();
            List<SelectListItem> country = new List<SelectListItem>();
            country = (from c in db.Country where c.Status == 1 orderby c.CountryName select new SelectListItem { Text = c.CountryName, Value = c.CountryName }).ToList();
            ViewBag.country = country;
            return View(CM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult editCountryCurrency(Currency_Master CM)
        {
            try
            {
                if (CM != null)
                {
                    CM.ModifiedBy = User.Identity.GetUserId();
                    CM.ModifiedOn = DateTime.Now;
                    db.Entry(CM).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["message"] = "Data Saved !!";
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("editCountryCurrency", "Master", new { Id = CM.Id });
        }

        public ActionResult statusCountryCurrency(int id)
        {
            // To check user access right
            string uri1 = "/Master/CountryCurrency";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            Currency_Master sa = new Currency_Master();
            sa = (from s in db.Currency_Master where s.Id == id select s).FirstOrDefault();
            if (sa.Status == 1)
            {
                sa.Status = 0;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            else
            {
                sa.Status = 1;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.Message = string.Format("Deactivated");
            return RedirectToAction("CountryCurrency", "Master");
        }
        public ActionResult viewCountryCurrency(int Id)
        {
            // To check user access right
            string uri1 = "/Master/CountryCurrency";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_view == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            Currency_Master CM = new Currency_Master();
            CM = (from s in db.Currency_Master where s.Id == Id select s).FirstOrDefault();
            List<SelectListItem> country = new List<SelectListItem>();
            country = (from c in db.Country where c.Status == 1 orderby c.CountryName select new SelectListItem { Text = c.CountryName, Value = c.CountryName }).ToList();
            ViewBag.country = country;
            return View(CM);
        }
        //----------------------- Country Currency Ends ---------------------------

        //-------------- Pincode Master Start -------------------------

        public ActionResult Pincode(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
            // To check user access right
            string uri1 = "/Master/Pincode";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            ViewBag.Permission = cm;
            // access right ends 
            int pageSize = 10;
            int pgc = 0;
            int pageCount = 0;
            List<PinCodeMaster> li = new List<PinCodeMaster>();
            if (searchtype != "" && searchtext != "")
            {
                if (searchtype == "country")
                {
                    pgc = db.PinCode.Where(x => x.Country.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.PinCode
                          orderby c.Country
                          select c).Where(x => x.Country.StartsWith(searchtext)).ToList();
                }
                else if (searchtype == "state")
                {
                    pgc = db.PinCode.Where(x => x.state.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.PinCode
                          orderby c.state
                          select c).Where(x => x.state.StartsWith(searchtext)).ToList();
                }
                else if (searchtype == "city")
                {
                    pgc = db.PinCode.Where(x => x.city.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.PinCode
                          orderby c.city
                          select c).Where(x => x.city.StartsWith(searchtext)).ToList();
                }
                else if (searchtype == "code")
                {
                    pgc = db.PinCode.Where(x => x.pincode.ToString().Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.PinCode
                          orderby c.city
                          select c).Where(x => x.pincode.ToString().StartsWith(searchtext)).ToList();
                }

                ViewBag.Search = searchtext;
            }
            else
            {
                pgc = db.PinCode.Count();
                pageCount = pgc / pageSize;

                li = (from s in db.PinCode orderby s.state select s).Skip(pageNo * pageSize).Take(pageSize).ToList();
                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }

            return View(li);
        }

        public ActionResult addPincode()
        {
            // To check user access right
            string uri1 = "/Master/Pincode";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            List<SelectListItem> country = new List<SelectListItem>();
            country = (from c in db.Country where c.Status == 1 orderby c.CountryName select new SelectListItem { Text = c.CountryName, Value = c.CountryName }).ToList();
            ViewBag.country = country;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addPincode(PinCodeMaster contM)
        {
            try
            {
                if (contM != null)
                {
                    contM.CreateDate = DateTime.Now;
                    contM.CreatedBy = User.Identity.GetUserId();
                    db.PinCode.Add(contM);
                    db.SaveChanges();
                    TempData["message"] = "Data Saved !!";
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("addPincode", "Master");
        }

        public ActionResult editPincode(int Id)
        {
            // To check user access right
            string uri1 = "/Master/Pincode";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            PinCodeMaster edp = new PinCodeMaster();
            edp = (from s in db.PinCode where s.id == Id select s).FirstOrDefault();
            List<SelectListItem> country = new List<SelectListItem>();
            country = (from c in db.Country where c.Status == 1 orderby c.CountryName select new SelectListItem { Text = c.CountryName, Value = c.CountryName }).ToList();
            ViewBag.country = country;
            return View(edp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult editPincode(PinCodeMaster sa)
        {
            try
            {
                if (sa != null)
                {

                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = "Data Saved !!";
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("editPincode", "Master", new { Id = sa.id });
        }
        public ActionResult statusPincode(int id)
        {
            // To check user access right
            string uri1 = "/Master/Pincode";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            PinCodeMaster sa = new PinCodeMaster();
            sa = (from s in db.PinCode where s.id == id select s).FirstOrDefault();
            if (sa.status == 1)
            {
                sa.status = 0;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            else
            {
                sa.status = 1;
            }
            db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.Message = string.Format("Deactivated");
            return RedirectToAction("Pincode", "Master");
        }
        public ActionResult viewPincode(int Id)
        {
            // To check user access right
            string uri1 = "/Master/Pincode";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_view == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 

            PinCodeMaster edp = new PinCodeMaster();
            edp = (from s in db.PinCode where s.id == Id select s).FirstOrDefault();
            List<SelectListItem> country = new List<SelectListItem>();
            country = (from c in db.Country where c.Status == 1 orderby c.CountryName select new SelectListItem { Text = c.CountryName, Value = c.CountryName }).ToList();
            ViewBag.country = country;
            return View(edp);
        }
        //-------------- Pincode Master End -------------------------
        //-------------- Group Header Master Start -------------------------
        public ActionResult groupheader(int pageNo = 0, string searchtype = "")
        {
            // To check user access right
            string uri1 = "/Master/groupheader";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            ViewBag.Permission = cm;
            // access right ends
            List<grouplist> li = new List<grouplist>();
            int pageSize = 10;
            int pgc = 0;
            int pageCount = 0;
            if (searchtype != "")
            {

                pgc = (from s in db.HeaderDescription join a in db.GroupHeader on s.header_id equals a.id orderby s.id select new grouplist { value = a.value, Description = s.header_description, id = s.id, Status = s.Status }).Where(x => x.value.Contains(searchtype)).Count();
                pageCount = pgc / pageSize;
                li = (from s in db.HeaderDescription join a in db.GroupHeader on s.header_id equals a.id orderby s.id select new grouplist { value = a.value, Description = s.header_description, id = s.id, Status = s.Status }).Where(x => x.value.StartsWith(searchtype)).ToList();



            }
            else
            {
                pgc = (from s in db.HeaderDescription join a in db.GroupHeader on s.header_id equals a.id orderby s.id select new grouplist { value = a.value, Description = s.header_description, id = s.id, Status = s.Status }).Count();
                pageCount = pgc / pageSize;

                li = (from s in db.HeaderDescription join a in db.GroupHeader on s.header_id equals a.id orderby s.id select new grouplist { value = a.value, Description = s.header_description, id = s.id, Status = s.Status }).Skip(pageNo * pageSize).Take(pageSize).ToList();


                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }
            List<SelectListItem> gh = new List<SelectListItem>();
            gh = (from c in db.GroupHeader select new SelectListItem { Text = c.value, Value = SqlFunctions.StringConvert((double)c.id).TrimStart() }).ToList();
            ViewBag.gh = gh;
            return View(li);
        }

        public ActionResult addgroupheader()
        {
            // To check user access right
            string uri1 = "/Master/groupheader";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            List<SelectListItem> li = new List<SelectListItem>();
            li = (from s in db.GroupHeader orderby s.value select new SelectListItem { Text = s.value, Value = SqlFunctions.StringConvert((double)s.id).TrimStart() }).ToList();
            ViewBag.drp = li;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addgroupheader(HeaderDescription sa)
        {
            try
            {
                if (sa != null)
                {
                    sa.CreatedOn = DateTime.Now;
                    sa.CreatedBy = User.Identity.GetUserId();
                    db.HeaderDescription.Add(sa);
                    db.SaveChanges();
                    TempData["message"] = "Data Saved !!";
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("addgroupheader", "Master");
        }
        public ActionResult editgroupheader(int id)
        {
            // To check user access right
            string uri1 = "/Master/groupheader";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            HeaderDescription sa = new HeaderDescription();
            List<SelectListItem> li = new List<SelectListItem>();
            li = (from s in db.GroupHeader orderby s.value select new SelectListItem { Text = s.value, Value = SqlFunctions.StringConvert((double)s.id).TrimStart() }).ToList();
            ViewBag.drp = li;
            sa = (from s in db.HeaderDescription where s.id == id select s).FirstOrDefault();
            return View(sa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult editgroupheader(HeaderDescription sa)
        {
            try
            {
                if (sa != null)
                {
                    sa.ModifiedOn = DateTime.Now;
                    sa.ModifiedBy = User.Identity.GetUserId();
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = "Data Saved !!";
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("editgroupheader", "Master", new { Id = sa.id });
        }

        public ActionResult viewgroupheader(int Id)
        {
            // To check user access right
            string uri1 = "/Master/groupheader";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_view == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            HeaderDescription sa = new HeaderDescription();
            List<SelectListItem> li = new List<SelectListItem>();
            li = (from s in db.GroupHeader select new SelectListItem { Text = s.value, Value = SqlFunctions.StringConvert((double)s.id).TrimStart() }).ToList();
            ViewBag.drp = li;
            sa = (from s in db.HeaderDescription where s.id == Id select s).FirstOrDefault();
            return View(sa);
        }
        public ActionResult statusgroupheader(int id)
        {
            // To check user access right
            string uri1 = "/Master/groupheader";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            HeaderDescription sa = new HeaderDescription();
            sa = (from s in db.HeaderDescription where s.id == id select s).FirstOrDefault();
            if (sa.Status == 1)
            {
                sa.ModifiedOn = DateTime.Now;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.Status = 0;
                TempData["message"] = "Deactivated";
            }
            else
            {
                sa.ModifiedOn = DateTime.Now;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.Status = 1;
                TempData["message"] = "Activated";
            }
            db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("groupheader", "Master");
        }
        //========================================== Header Description END ======================================================
        // [Encrypt]

        [Authorize(Roles = "Admin,Quality,Sr.Help Desk Manager,Help Desk Manager")]
        // [OutputCache(Duration = 600, VaryByParam = "*")]
        public async Task<ActionResult> FEMaster()
        {
            // Create separate instances of DbContext
            using (var dbContext1 = new ApplicationDbContext())
            using (var dbContext2 = new ApplicationDbContext())
            {
                // Perform database calls in parallel using separate DbContext instances
                var feListTask = dbContext1.Database.SqlQuery<FE_Master_Personal_list>("EXEC sp_GetFEMasterPersonalList").ToListAsync();
                var countsTask = dbContext2.Database.SqlQuery<FEMasterCounts>("EXEC sp_GetFEMasterPersonalCounts").FirstOrDefaultAsync();

                await Task.WhenAll(feListTask, countsTask);

                var feList = feListTask.Result;
                var counts = countsTask.Result;

                if (counts != null)
                {
                    ViewBag.Activated = counts.Activated;
                    ViewBag.DeActivated = counts.DeActivated;
                    ViewBag.BlackListed = counts.BlackListed;
                    ViewBag.CertiAct = counts.CertiAct;
                    ViewBag.NotInterested = counts.NotInterested;
                    ViewBag.CertiDeAct = feList.Count - counts.CertiAct;
                }

                return View(feList);
            }
        }


        public ActionResult FEPersonal(string Id)
        {

            int id = UrlEncryptionHelper.Decrypt(Id);
            int per = 25;
            ManageFePersonal MP = new ManageFePersonal();
       
            ViewBag.Language = (from c in db.HeaderDescription where c.header_id == 1 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).OrderBy(x => x.Text).ToList();
            ViewBag.Identification = (from c in db.HeaderDescription where c.header_id == 2 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Chat_mode = (from c in db.HeaderDescription where c.header_id == 3 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Citizen = (from c in db.HeaderDescription where c.header_id == 8 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.FE_Type = (from c in db.HeaderDescription where c.header_id == 9 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.DialCode = (from c in db.Country_Dialing_Code where c.Status == 1 select new SelectListItem { Text = c.Code + " " + c.Country, Value = c.Code.Trim() }).ToList();
            ViewBag.Currency = (from s in db.Currency_Master
                                where
   s.Country == (from a in db.FE_Master_Personal where a.Id == id select a.Country).FirstOrDefault() && s.Currency != "USD"
                                select new SelectListItem { Text = s.Currency, Value = SqlFunctions.StringConvert((double)s.Id).Trim() }).ToList();
            ViewBag.Certification = (from c in db.Certification_Master where c.Status == 1 select new SelectListItem { Text = c.Certification_Name, Value = SqlFunctions.StringConvert((double)c.Id).Trim() }).ToList();
            ViewBag.Exp = (from c in db.HeaderDescription where c.header_id == 12 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Skill = (from c in db.HeaderDescription where c.header_id == 18 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();


            MP.IV = new List<IdentificationInsertView>();

            MP.FE = (from s in db.FE_Master_Personal where s.Id == id select s).FirstOrDefault();
            MP.IF = (from s in db.FE_Master_Identification
                     join hd in db.HeaderDescription on s.ID_Type equals hd.id
                     where s.FE_ID == id
                     select new IdentificationInsertFill
                     {
                         FE_ID = s.FE_ID,
                         ID_Number = s.ID_Number,
                         ID_Type = hd.header_description,
                         Upload = s.ID_Upload,
                         IdType = s.ID_Type
                     }).ToList();
            ViewBag.Edit = "Yes";

            if (MP.FE.NDA_Accept == 1)
            {
                per = 60;
            }

            if (MP.FE.latitude != null)
            {
                TempData["geo"] = "Yes";
            }
            else
            {
                TempData["geo"] = null;
            }
            //========================================== Charges & Services ===========================================================
            int cntc = (from s in db.FE_Master_Charges where s.FE_ID == id select s).Count();

            if (cntc != 0)
            {
                MP.FEC = (from s in db.FE_Master_Charges where s.FE_ID == id select s).FirstOrDefault();
                MP.ICS = (from c in db.FE_Master_serviceArea where c.FE_ID == id select new InsertServiceArea { FE_ID = c.FE_ID, Country = c.Country, ZipCode_pincode = c.ZipCode_pincode }).ToList();
                ViewBag.ChargEdit = "Yes";
                ViewBag.ChargesPer = 1;
                per = per + 15;
            }
            else
            {
                MP.FEC = (from s in db.FE_Master_Charges where s.FE_ID == id select s).FirstOrDefault();
                MP.ICS = (from c in db.FE_Master_serviceArea where c.FE_ID == id select new InsertServiceArea { FE_ID = c.FE_ID, Country = c.Country, ZipCode_pincode = c.ZipCode_pincode }).ToList();
                ViewBag.ChargesPer = 0;
                per = per + 0;
            }

            //============================================== Financial ===============================================================

            int cntf = (from s in db.FE_Master_Financial where s.FE_ID == id select s).Count();

            if (cntf != 0)
            {
                MP.FEF = (from s in db.FE_Master_Financial where s.FE_ID == id select s).FirstOrDefault();

                ViewBag.Editf = "Yes";
            }
            else
            {
            }

            //=============================================== Skill ==================================================================

           
            int cnts = (from s in db.FE_Master_Skill where s.FE_ID == id select s).Count();
            if (cnts != 0)
            {
                MP.ICCE = (from c in db.FE_Master_Certification join mc in db.Certification_Master on c.Certification_Name equals mc.Id where c.FE_ID == id select new InsertCertificateView { Id = c.Certification_Name, FE_ID = c.FE_ID, Certification_Upload = c.Certification_Upload, Certification_Name = mc.Certification_Name }).ToList();

                MP.FES = (from s in db.FE_Master_Skill where s.FE_ID == id select s).FirstOrDefault();
                MP.ISV = (from s in db.FE_Master_Skill_Data join a in db.HeaderDescription on s.Skill equals a.id.ToString() join b in db.HeaderDescription on s.Exp equals b.id.ToString() where s.FE_ID == id select new InsertSkillsView { Id = s.Id, FE_ID = s.FE_ID, Skill_Name = a.header_description, Exp_Upload = b.header_description }).ToList();
                ViewBag.EditS = "Yes";
                ViewBag.SkillPer = 1;
                per = per + 15;
            }
            else
            {
                per = per + 0;
                ViewBag.SkillPer = 0;
                MP.ICCE = (from c in db.FE_Master_Certification join mc in db.Certification_Master on c.Certification_Name equals mc.Id where c.FE_ID == id select new InsertCertificateView { Id = c.Certification_Name, FE_ID = c.FE_ID, Certification_Upload = c.Certification_Upload, Certification_Name = mc.Certification_Name }).ToList();
                MP.ISV = (from s in db.FE_Master_Skill_Data join a in db.HeaderDescription on s.Skill equals a.id.ToString() join b in db.HeaderDescription on s.Exp equals b.id.ToString() where s.FE_ID == id select new InsertSkillsView { Id = s.Id, FE_ID = s.FE_ID, Skill_Name = a.header_description, Exp_Upload = b.header_description }).ToList();
            }
            ViewBag.Percentage = per;
            ViewBag.ResumePer = (from s in db.FE_Master_Skill where s.FE_ID == id select s.Bio_Data).FirstOrDefault();

            //------------------------------------------------------------- Activity Log 

            ViewBag.TicketReceived = (from s in db.Ticket where s.FE_ID == id select s).Count();

            ViewBag.TicketExecuted = (from s in db.Ticket where s.FE_ID == id && s.Status == 20 select s).Count();

            ViewBag.TicketDenied = (from s in db.Ticket_FE_Selection where s.FE_ID == id && s.Status == "Reject" select s).Count();


            FECSAT fc = new FECSAT();

            fc = db.Database.SqlQuery<FECSAT>("getFECSAT @id", new SqlParameter("@id", id)).FirstOrDefault();

            ViewBag.CSAT = fc.CSAT + "/5" + " [" + fc.Count + "]";

            //MP.blackcertificate = (from s in db.FE_Blacklist where s.FE_ID == id select s.Certificate_Id).ToList();
            // ViewBag.Blacklist=(from s in db.FE_Master_Personal where s.Id==id select s.Blacklist).ToList();
            ViewBag.blCert = (from s in db.FE_Blacklist where s.FE_ID == id select s.Certificate_Id).ToList();
            //------------------------------------------------------------- Activity Log Ends
            //string blCertString = string.Join(",", ViewBag.blCert);
            //ViewBag.blCertString = blCertString;

            return View(MP);
        }
        

        public ActionResult addFEPersonal()
        {
            ViewBag.Language = (from c in db.HeaderDescription where c.header_id == 1 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();
            ViewBag.Identification = (from c in db.HeaderDescription where c.header_id == 2 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Chat_mode = (from c in db.HeaderDescription where c.header_id == 3 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Citizen = (from c in db.HeaderDescription where c.header_id == 8 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.FE_Type = (from c in db.HeaderDescription where c.header_id == 9 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.DialCode = (from c in db.Country_Dialing_Code where c.Status == 1 select new SelectListItem { Text = c.Code + " " + c.Country, Value = c.Code.Trim() }).ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addFEPersonal(FE_Master_Personal sa, string Others, string Storage, string Networking, string Server, string Laptop, string Desktop)
        {
            try
            {

                int cnt = (from s in db.FE_Master_Personal where s.Email == sa.Email select s).Count();
                int cnt1 = (from s in db.FE_Master_Personal where s.Phone_Number == sa.Phone_Number select s).Count();
                if (cnt > 0)
                {
                    TempData["message"] = "Your Email Id is already Register with us.For any query please reach us at partner1@inwinteck.com !!";
                    TempData["error"] = "Yes";
                    return RedirectToAction("addFEPersonal", "Master");

                }
                else if (cnt1 > 0)
                {
                    TempData["message"] = "Your Phone number is already Register with us.For any query please reach us at partner1@inwinteck.com !!";
                    TempData["error"] = "Yes";
                    return RedirectToAction("addFEPersonal", "Master");
                }

                sa.InwinFEID = utlity.FEInwinteckId(sa.Country);
                sa.CreatedBy = "27b3bb6b-9331-4470-8026-d02e9125bab4";
                sa.CreatedOn = DateTime.Now;
                //added by rohit for by default blacklist value =0 which means not blacklisted. on 25-09-23
                sa.Blacklist = 0;
                sa.FeInterest = 1;
                db.FE_Master_Personal.Add(sa);
               
               
                db.SaveChanges();



                // User Creation

                string password = Membership.GeneratePassword(8, 1);
                try
                {
                    UserRegistrationView model = new UserRegistrationView();
                    model.Password = password;
                    var user = new ApplicationUser { UserName = sa.Email, Email = sa.Email };

                    user.Name = sa.First_Name + " " + sa.Last_Name;
                    user.CreatedBy = User.Identity.GetUserId();
                    user.CreatedOn = DateTime.Now;
                    user.ChangePassword = 1;
                    user.Status = 1;

                    IdentityResult result = UserManager.Create(user, model.Password);
                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(user.Id, "Field Engineer");
                        ModelState.Clear();
                        TempData["message"] = "User Created !!";
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            TempData["message"] = error;
                        }
                    }

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Error Occured While Saving !!" + ex.ToString();
                }


                string fe_type;
                if (sa.FE_Type == 345)
                {
                    fe_type = "<b class='font-montserrat color-dark'>Company</b>.<br/> Company Name : <b class='font-montserrat color-dark'>" + sa.Company_Name + "</b>";
                }
                else
                {
                    fe_type = "<b class='font-montserrat color-dark'>Freelancer</b>";
                }
                string body = utlity.WelcomeFE(sa.First_Name + " " + sa.Last_Name, password, sa.Email, fe_type, sa.City, sa.Country);
                utlity.sendmailAcc(sa.Email, "Welcome Partner", body, "Register <support@inwinteck.com>");

                TempData["error"] = "No";
                TempData["login"] = sa.Email;
                TempData["message"] = "Thank you for initiating the registration.  Password has been sent to your email id.  Kindly login for completing the registration ";

            }
            catch (Exception ex)
            {
                TempData["error"] = "Yes";
                TempData["message"] = "FE Not Created !! ";
            }
            return RedirectToAction("addFEPersonal", "Master");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditFEPersonal(ManageFePersonal sa, HttpPostedFileBase pht, HttpPostedFileBase Contract, HttpPostedFileBase BD, string addSkill, string addFin, string addChar, List<InsertSkills> IV, List<InsertCertificate> IVC, List<IdentificationInsertView> IVI, string[] BlCertificate_Id)
        {
            try
            {
                string change = " <table style='width: 100 %; height: 40px;padding-left:20px; border:1px;'><tr style='background: rgb(131,194, 51);'><td>Fields</td><td>Old Data<td>New Data</td></tr>";
                string pic = "";
                string cont = "";
                string bd = "";

                //------------------------------------------------------------------- Email Log Starts

                FE_Master_Personal_Old FEMPOld = new FE_Master_Personal_Old();
                FEMPOld = (from s in db.FE_Master_Personal
                           where s.Id == sa.FE.Id
                           select new FE_Master_Personal_Old
                           {
                               Alt_Email = s.Alt_Email,
                               Alt_Phone_Number = s.Alt_Phone_Number,
                               Alt_Chat_Mode = s.Alt_Chat_Mode,
                               Alt_Chat_Mode_1 = s.Alt_Chat_Mode_1,
                               Alt_Chat_Mode_2 = s.Alt_Chat_Mode_2,
                               Alt_Phone_Number_1 = s.Alt_Phone_Number_1,
                               Alt_Phone_Number_2 = s.Alt_Phone_Number_2,
                               Alt_Phone_Number_Code = s.Alt_Phone_Number_Code,
                               Alt_Phone_Number_Code_1 = s.Alt_Phone_Number_Code_1,
                               Alt_Phone_Number_Code_2 = s.Alt_Phone_Number_Code_2,
                               Language_Spoken = s.Language_Spoken,
                               Language_Spoken_1 = s.Language_Spoken_1,
                               Language_Spoken_2 = s.Language_Spoken_2,
                               Manager_Name = s.Manager_Name,
                               Manager_Email = s.Manager_Email,
                               Manager_Phone_Number = s.Manager_Phone_Number
                           }).FirstOrDefault();

                if (FEMPOld.Alt_Email != sa.FE.Alt_Email)
                {
                    change = change + "<tr><td> Alternate Email </td><td>" + FEMPOld.Alt_Email + "</td><td>" + sa.FE.Alt_Email + "</td></tr>";
                }

                if (FEMPOld.Alt_Phone_Number_Code != sa.FE.Alt_Phone_Number_Code)
                {
                    change = change + "<tr><td> Alternate Phone Number code </td><td>" + FEMPOld.Alt_Phone_Number_Code + "</td><td>" + sa.FE.Alt_Phone_Number_Code + "</td></tr>";
                }

                if (FEMPOld.Alt_Phone_Number != sa.FE.Alt_Phone_Number)
                {
                    change = change + "<tr><td> Alternate Phone Number </td><td>" + FEMPOld.Alt_Phone_Number + "</td><td>" + sa.FE.Alt_Phone_Number + "</td></tr>";
                }

                if (FEMPOld.Alt_Chat_Mode != sa.FE.Alt_Chat_Mode)
                {
                    string alpnco = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Alt_Chat_Mode select o.header_description).FirstOrDefault();

                    string alpncn = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Alt_Chat_Mode select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Alternate Phone Number Code </td><td>" + alpnco + "</td><td>" + alpncn + "</td></tr>";
                }

                //-------------------------------------------------------------------

                if (FEMPOld.Alt_Phone_Number_Code_1 != sa.FE.Alt_Phone_Number_Code_1)
                {
                    change = change + "<tr><td> Alternate Phone Number code </td><td>" + FEMPOld.Alt_Phone_Number_Code_1 + "</td><td>" + sa.FE.Alt_Phone_Number_Code_1 + "</td></tr>";
                }

                if (FEMPOld.Alt_Phone_Number_1 != sa.FE.Alt_Phone_Number_1)
                {
                    change = change + "<tr><td> Alternate Phone Number </td><td>" + FEMPOld.Alt_Phone_Number_1 + "</td><td>" + sa.FE.Alt_Phone_Number_1 + "</td></tr>";
                }

                if (FEMPOld.Alt_Chat_Mode_1 != sa.FE.Alt_Chat_Mode_1)
                {
                    string alpnco1 = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Alt_Chat_Mode_1 select o.header_description).FirstOrDefault();

                    string alpncn1 = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Alt_Chat_Mode_1 select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Alternate Phone Number Code </td><td>" + alpnco1 + "</td><td>" + alpncn1 + "</td></tr>";
                }

                //-------------------------------------------------------------------

                if (FEMPOld.Alt_Phone_Number_Code_2 != sa.FE.Alt_Phone_Number_Code_2)
                {
                    change = change + "<tr><td> Alternate Phone Number code </td><td>" + FEMPOld.Alt_Phone_Number_Code_2 + "</td><td>" + sa.FE.Alt_Phone_Number_Code_2 + "</td></tr>";
                }

                if (FEMPOld.Alt_Phone_Number_2 != sa.FE.Alt_Phone_Number_2)
                {
                    change = change + "<tr><td> Alternate Phone Number </td><td>" + FEMPOld.Alt_Phone_Number_2 + "</td><td>" + sa.FE.Alt_Phone_Number_2 + "</td></tr>";
                }

                if (FEMPOld.Alt_Chat_Mode_2 != sa.FE.Alt_Chat_Mode_2)
                {
                    string alpnco2 = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Alt_Chat_Mode_2 select o.header_description).FirstOrDefault();

                    string alpncn2 = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Alt_Chat_Mode_2 select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Alternate Phone Number Code </td><td>" + alpnco2 + "</td><td>" + alpncn2 + "</td></tr>";
                }

                //------------------------------------------------------------------- Languag Log

                if (FEMPOld.Language_Spoken != sa.FE.Language_Spoken)
                {
                    string lano = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Language_Spoken select o.header_description).FirstOrDefault();

                    string lann = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Language_Spoken select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Language </td><td>" + lano + "</td><td>" + lann + "</td></tr>";
                }

                if (FEMPOld.Language_Spoken_1 != sa.FE.Language_Spoken_1)
                {
                    string lano1 = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Language_Spoken_1 select o.header_description).FirstOrDefault();

                    string lann1 = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Language_Spoken_1 select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Language </td><td>" + lano1 + "</td><td>" + lann1 + "</td></tr>";
                }

                if (FEMPOld.Language_Spoken_2 != sa.FE.Language_Spoken_2)
                {
                    string lano2 = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Language_Spoken_2 select o.header_description).FirstOrDefault();

                    string lann2 = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Language_Spoken_2 select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Language </td><td>" + lano2 + "</td><td>" + lann2 + "</td></tr>";
                }

                if (FEMPOld.Manager_Name != sa.FE.Manager_Name)
                {
                    change = change + "<tr><td> Manager Name </td><td>" + FEMPOld.Manager_Name + "</td><td>" + sa.FE.Manager_Name + "</td></tr>";
                }

                if (FEMPOld.Manager_Phone_Number != sa.FE.Manager_Phone_Number)
                {
                    change = change + "<tr><td> Manager Phone Number </td><td>" + FEMPOld.Manager_Phone_Number + "</td><td>" + sa.FE.Manager_Phone_Number + "</td></tr>";
                }

                if (FEMPOld.Manager_Email != sa.FE.Manager_Email)
                {
                    change = change + "<tr><td> Manager Email </td><td>" + FEMPOld.Manager_Email + "</td><td>" + sa.FE.Manager_Email + "</td></tr>";
                }

                //------------------------------------------------------------------- Email Log Peronal Ends

                if (pht != null)
                {

                    pic = System.IO.Path.GetFileName(pht.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Photo"), fileName);
                    pht.SaveAs(pathtest);

                    sa.FE.Photo = url + "Photo/" + fileName;
                }

                if (Contract != null)
                {

                    cont = System.IO.Path.GetFileName(Contract.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + cont;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Contract"), fileName);
                    Contract.SaveAs(pathtest);

                    sa.FE.Signature = url + "Contract/" + fileName;
                }

                //if (sa.FE.NDA_Acceptance_Date == null)
                //{
                //    sa.FE.NDA_Accept = 1;
                //    sa.FE.Status = 1;
                //    sa.FE.NDA_Acceptance_Date = DateTime.Now;
                //}
                sa.FE.ModifiedBy = User.Identity.GetUserId();
                sa.FE.ModifiedOn = DateTime.Now;
                db.Entry(sa.FE).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                if (sa.FE.Other_detail != null)
                {
                    FE_Master_Other_Detail FEMOD = new FE_Master_Other_Detail();

                    FEMOD.FE_ID = sa.FE.Id;
                    FEMOD.Other_Details = sa.FE.Other_detail;
                    FEMOD.CreatedBy = sa.FE.CreatedBy;
                    FEMOD.CreatedOn = sa.FE.CreatedOn;
                    db.FE_Master_Other_Detail.Add(FEMOD);
                    db.SaveChanges();
                }



                if (addChar == "Yes")
                {
                    sa.FEC.CreatedBy = User.Identity.GetUserId();
                    sa.FEC.CreatedOn = DateTime.Now;
                    db.FE_Master_Charges.Add(sa.FEC);
                    db.SaveChanges();
                }
                else
                {
                    //------------------------------------------------------------------- Email Log Charges Starts

                    FE_Master_Charges_Old FECO = new FE_Master_Charges_Old();
                    FECO = (from s in db.FE_Master_Charges
                            where s.FE_ID == sa.FE.Id
                            select new FE_Master_Charges_Old
                            {
                                Currency = s.Currency,
                                Charges_Business_Hour = s.Charges_Business_Hour,
                                Charges_Non_Business_Hour = s.Charges_Non_Business_Hour,
                                Charge_Day = s.Charge_Day,
                                Charge_Job = s.Charge_Job,
                                Charge_Month = s.Charge_Month,
                                Minimum_Hrs = s.Minimum_Hrs,
                                Other_detail = s.Other_detail,
                                //Travel_Charge = s.Travel_Charge
                            }).FirstOrDefault();

                    if (FECO.Currency != sa.FEC.Currency)
                    {
                        change = change + "<tr><td> Currency </td> <td>" + FECO.Currency + "</td><td>" + sa.FEC.Currency + "</td></tr>";
                    }

                    if (FECO.Charges_Business_Hour != sa.FEC.Charges_Business_Hour)
                    {
                        change = change + "<tr><td> Charges Per Business Hour </td> <td>" + FECO.Charges_Business_Hour + "</td><td>" + sa.FEC.Charges_Business_Hour + "</td></tr>";
                    }

                    if (FECO.Charges_Non_Business_Hour != sa.FEC.Charges_Non_Business_Hour)
                    {
                        change = change + "<tr><td> Charges Per Non Business Hour </td> <td>" + FECO.Charges_Non_Business_Hour + "</td><td>" + sa.FEC.Charges_Non_Business_Hour + "</td></tr>";
                    }

                    if (FECO.Charge_Job != sa.FEC.Charge_Job)
                    {
                        change = change + "<tr><td> Charges Per job </td> <td>" + FECO.Charge_Job + "</td><td>" + sa.FEC.Charge_Job + "</td></tr>";
                    }

                    if (FECO.Charge_Month != sa.FEC.Charge_Month)
                    {
                        change = change + "<tr><td> Charges Per Month </td> <td>" + FECO.Charge_Month + "</td><td>" + sa.FEC.Charge_Month + "</td></tr>";
                    }

                    if (FECO.Minimum_Hrs != sa.FEC.Minimum_Hrs)
                    {
                        change = change + "<tr><td> Minimum Hrs </td> <td>" + FECO.Minimum_Hrs + "</td><td>" + sa.FEC.Minimum_Hrs + "</td></tr>";
                    }

                    //if (FECO.Travel_Charge != sa.FEC.Travel_Charge)
                    //{
                    //    change = change + "<tr><td> Travel Charge </td> <td>" + FECO.Travel_Charge + "</td><td>" + sa.FEC.Travel_Charge + "</td></tr>";
                    //}

                    if (FECO.Other_detail != sa.FEC.Other_detail)
                    {
                        change = change + "<tr><td> Charge Other Detail </td> <td>" + FECO.Other_detail + "</td><td>" + sa.FEC.Other_detail + "</td></tr>";
                    }

                    //------------------------------------------------------------------- Email Log Charges Ends

                    sa.FEC.ModifiedBy = User.Identity.GetUserId();
                    sa.FEC.ModifiedOn = DateTime.Now;
                    db.Entry(sa.FEC).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }




                if (addSkill == "Yes")
                {
                    if (BD != null)
                    {

                        bd = System.IO.Path.GetFileName(BD.FileName);
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + bd;
                        string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Resume"), fileName);
                        BD.SaveAs(pathtest);

                        sa.FES.Bio_Data = url + "Resume/" + fileName;
                    }

                    sa.FES.CreatedBy = User.Identity.GetUserId();
                    sa.FES.CreatedOn = DateTime.Now;
                    db.FE_Master_Skill.Add(sa.FES);
                    db.SaveChanges();
                }
                else
                {
                    if (BD != null)
                    {

                        bd = System.IO.Path.GetFileName(BD.FileName);
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + bd;
                        string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Resume"), fileName);
                        BD.SaveAs(pathtest);

                        sa.FES.Bio_Data = url + "Resume/" + fileName;
                    }

                    sa.FES.ModifiedBy = User.Identity.GetUserId();
                    sa.FES.ModifiedOn = DateTime.Now;
                    db.Entry(sa.FES).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }


                if (sa.FES.Others != null)
                {
                    FE_Master_Certification_Extra_Detail FEMCD = new FE_Master_Certification_Extra_Detail();

                    FEMCD.FE_ID = sa.FES.Id;
                    FEMCD.Other_Details = sa.FES.Others;
                    FEMCD.CreatedBy = sa.FES.CreatedBy;
                    FEMCD.CreatedOn = sa.FES.CreatedOn;
                    db.FE_Master_Certification_Extra_Detail.Add(FEMCD);
                    db.SaveChanges();
                }

                if (IV[0].Exp != null)
                {
                    FE_Master_Skill_Data FeID = new FE_Master_Skill_Data();

                    //Loop and insert records.
                    foreach (InsertSkills iv in IV)
                    {
                        // change = change + "<tr><td> Skill </td><td> New Entery </td><td>" + (from s in db.HeaderDescription where s.id = iv.Skill select s.H) + "</td></tr>";
                        FeID.FE_ID = sa.FE.Id;
                        FeID.Skill = iv.Skill;
                        FeID.Exp = iv.Exp;
                        FeID.CreatedBy = User.Identity.GetUserId();
                        FeID.CreatedOn = DateTime.Now;
                        db.FE_Master_Skill_Data.Add(FeID);
                        db.SaveChanges();
                    }
                }



                if (IVI[0].ID_Type > 0)
                {
                    FE_Master_Identification FeID = new FE_Master_Identification();

                    //Loop and insert records.
                    foreach (IdentificationInsertView iv in IVI)
                    {
                        FeID.FE_ID = sa.FE.Id;
                        FeID.ID_Number = iv.ID_Number;
                        FeID.ID_Type = iv.ID_Type;
                        string pici = "IDType_";
                        if (iv.ID_Upload != null)
                        {

                            pici = System.IO.Path.GetFileName(iv.ID_Upload.FileName);
                            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pici;
                            string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/ID"), fileName);
                            iv.ID_Upload.SaveAs(pathtest);

                            FeID.ID_Upload = url + "ID/" + fileName;
                        }
                        //FeID.ID_Upload = utlity.ConvertToImage(iv.ID_Upload);
                        FeID.CreatedBy = User.Identity.GetUserId();
                        FeID.CreatedOn = DateTime.Now;
                        db.FE_Master_Identification.Add(FeID);
                        db.SaveChanges();
                    }
                }

                if (IVC[0].Certification_Name > 0)
                {
                    FE_Master_Certification FeID = new FE_Master_Certification();

                    //Loop and insert records.
                    foreach (InsertCertificate iv in IVC)
                    {
                        FeID.FE_ID = sa.FE.Id;
                        FeID.Certification_Name = iv.Certification_Name;

                        string picc = "IDType_";
                        if (iv.Certification_Upload != null)
                        {

                            picc = System.IO.Path.GetFileName(iv.Certification_Upload.FileName);
                            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + picc;
                            string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Certificate"), fileName);
                            iv.Certification_Upload.SaveAs(pathtest);

                            FeID.Certification_Upload = url + "Certificate/" + fileName;
                        }
                        //FeID.ID_Upload = utlity.ConvertToImage(iv.ID_Upload);
                        FeID.CreatedBy = User.Identity.GetUserId();
                        FeID.CreatedOn = DateTime.Now;
                        db.FE_Master_Certification.Add(FeID);
                        db.SaveChanges();
                    }
                }
                if( BlCertificate_Id != null)
                {
                    FE_BlackList FeBl = new FE_BlackList();
                    string connectionString2 = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString2))
                    {
                        connection.Open();
                        // First, delete existing data for the given FE_ID initially and then insert certificates
                        using (SqlCommand deleteCmd = new SqlCommand("DeleteFE_BlackList", connection))
                        {
                            deleteCmd.CommandType = CommandType.StoredProcedure;
                            deleteCmd.Parameters.AddWithValue("@FE_ID", sa.FE.Id);

                            deleteCmd.ExecuteNonQuery();
                        }
                    }
                        foreach (string name in BlCertificate_Id)
                    {
                        FeBl.FE_ID = sa.FE.Id;
                        FeBl.Certificate_Id = int.Parse(name);

                        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;                    
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            
                            using (SqlCommand cmd = new SqlCommand("InsertFE_BlackList", connection))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@FE_ID", FeBl.FE_ID);
                                cmd.Parameters.AddWithValue("@Certificate_Id", FeBl.Certificate_Id);

                                
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    FE_Master_Personal sabl = db.FE_Master_Personal.SingleOrDefault(p => p.Id == sa.FE.Id); 

                    if (sabl != null)
                    {
                        sabl.Blacklist = 1;
                        sabl.Status = 2;
                        db.SaveChanges();
                    }
                    

                }
                else
                {
                    string connectionString2 = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


                    using (SqlConnection connection = new SqlConnection(connectionString2))
                    {
                        connection.Open();
                        //  delete existing data for the given FE_ID if there are no certificates selected.
                        using (SqlCommand deleteCmd = new SqlCommand("DeleteFE_BlackList", connection))
                        {
                            deleteCmd.CommandType = CommandType.StoredProcedure;
                            deleteCmd.Parameters.AddWithValue("@FE_ID", sa.FE.Id);

                            deleteCmd.ExecuteNonQuery();
                        }
                    }

                    FE_Master_Personal sabl = db.FE_Master_Personal.SingleOrDefault(p => p.Id == sa.FE.Id); 

                    if (sabl != null)
                    {
                        sabl.Blacklist = 0;
                        sabl.Status = 1;
                        db.SaveChanges();
                    }

                }



                if (addFin == "Yes")
                {
                    //TempData["message"] = "Thank you for completing the registeration.Welcome aboard.Copy of signed NDA sent to your registered email id.";
                    TempData["message"] = "Congratulations and Thank you for completing the registration & activating the account. Welcome Aboard,Partner ! ";
                    string fe_type;
                    if (sa.FE.FE_Type == 345)
                    {
                        fe_type = "<b class='font-montserrat color-dark'>Company</b>.<br/> Company Name : <b class='font-montserrat color-dark'>" + sa.FE.Company_Name + "</b>";
                    }
                    else
                    {
                        fe_type = "<b class='font-montserrat color-dark'>Freelancer</b>";
                    }
                    string body = utlity.WelcomeAbord(sa.FE.First_Name + " " + sa.FE.Last_Name, fe_type, sa.FE.City, sa.FE.Country);
                    string filename = "Inwinteck_NDA_" + sa.FE.Id + ".pdf";
                    utlity.NDAPDF(sa.FE.Email, "Welcome Aboard", body, "Welcome Aboard<support@inwinteck.com>", sa.FE.First_Name + " " + sa.FE.Last_Name, sa.FE.NDA_Acceptance_Date.Value.ToString("dd-MM-yyyy HH:mm:ss"), filename);

                }
                else
                {
                    change = change + "</table>";
                    TempData["message"] = "Details Updated !!";
                    //  string body = utlity.Changelog(sa.FE.First_Name + " " + sa.FE.Last_Name, change);

                    //   utlity.sendmailAcc(sa.FE.Email, "Data Updated", body, "Data Updated<support@inwinteck.com>");
                }



            }
            catch (Exception ex)
            {
                TempData["message"] = "Details Not Updated !! ";
            }
            return RedirectToAction("FEPersonal", "Master", new { id = UrlEncryptionHelper.Encrypt(sa.FE.Id) });
        }
        public ActionResult InsertIdentification(List<IdentificationInsertView> IV)
        {

            if (IV != null)
            {
                int nid = 0;

                FE_Master_Identification FeID = new FE_Master_Identification();

                //Loop and insert records.
                foreach (IdentificationInsertView iv in IV)
                {
                    FeID.FE_ID = iv.FE_ID;
                    nid = iv.FE_ID;
                    FeID.ID_Number = iv.ID_Number;
                    FeID.ID_Type = iv.ID_Type;
                    string pic = "IDType_";
                    if (iv.ID_Upload != null)
                    {

                        pic = System.IO.Path.GetFileName(iv.ID_Upload.FileName);
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                        string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/ID"), fileName);
                        iv.ID_Upload.SaveAs(pathtest);

                        FeID.ID_Upload = url + "ID/" + fileName;
                    }
                    //FeID.ID_Upload = utlity.ConvertToImage(iv.ID_Upload);
                    FeID.CreatedBy = User.Identity.GetUserId();
                    FeID.CreatedOn = DateTime.Now;
                    db.FE_Master_Identification.Add(FeID);
                    db.SaveChanges();
                }
                TempData["message"] = "Data Updated !! ";
                return RedirectToAction("FEPersonal", "Master", new { id = nid });
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEPersonal", "Master");
            }
        }
        public ActionResult DeleteIdentification(int FeId, int IdType)
        {
            string change = " <table style='width: 100 %; height: 40px;padding-left:20px; border:1px;'><tr style='background: rgb(131,194, 51);'><td>Fields</td><td>Old Data<td>New Data</td></tr>";
            if (FeId > 0 && IdType > 0)
            {
                int nid = FeId;

                FE_Master_Identification FI = new FE_Master_Identification();
                FE_Master_Personal sa = new FE_Master_Personal();
                sa = (from s in db.FE_Master_Personal where s.Id == FeId select s).FirstOrDefault();
                FI = (from c in db.FE_Master_Identification where c.FE_ID == FeId && c.ID_Type == IdType select c).FirstOrDefault();
                string IDtpe = (from s in db.HeaderDescription where s.id == FI.ID_Type select s.header_description).FirstOrDefault();
                if (FI != null)
                {
                    db.Entry(FI).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";

                    change = change + "<tr><td> ID Type </td><td>" + IDtpe + "</td><td> Data Deleted</td></tr></table>";
                    string body = utlity.Changelog(sa.First_Name + " " + sa.Last_Name, change);

                    utlity.sendmailAcc(sa.Email, "Data Deleted", body, "Data Deleted<support@inwinteck.com>");



                    return RedirectToAction("FEPersonal", "Master", new { id = FeId });
                }


            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEPersonal", "Master", new { id = FeId });
            }

            return View();
        }

        public ActionResult FECharges(int id)
        {
            // To check user access right
            string uri1 = "/Master/FEMaster";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            ManageFeCharges MC = new ManageFeCharges();

            // access right ends
            //FE_Master_Charges sa = new FE_Master_Charges();

            int cnt = (from s in db.FE_Master_Charges where s.FE_ID == id select s).Count();
            ViewBag.Currency = (from s in db.Currency_Master where s.Country == (from a in db.FE_Master_Personal where a.Id == id select a.Country).FirstOrDefault() select s.Currency).FirstOrDefault();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();
            ViewBag.FE = id;
            if (cnt != 0)
            {
                MC.FE = (from s in db.FE_Master_Charges where s.FE_ID == id select s).FirstOrDefault();
                MC.IC = (from c in db.FE_Master_serviceArea where c.FE_ID == id select new InsertServiceArea { FE_ID = c.FE_ID, Country = c.Country, ZipCode_pincode = c.ZipCode_pincode }).ToList();
                ViewBag.Edit = "Yes";
                return View(MC);
            }
            else
            {
                MC.FE = (from s in db.FE_Master_Charges where s.FE_ID == id select s).FirstOrDefault();
                MC.IC = (from c in db.FE_Master_serviceArea where c.FE_ID == id select new InsertServiceArea { FE_ID = c.FE_ID, Country = c.Country, ZipCode_pincode = c.ZipCode_pincode }).ToList();
                return View(MC);
            }

        }
        public ActionResult InsertFeCharges(List<InsertServiceArea> IV)
        {
            int nid = 0;

            if (IV != null)
            {


                FE_Master_serviceArea FeID = new FE_Master_serviceArea();

                //Loop and insert records.
                foreach (InsertServiceArea iv in IV)
                {
                    FeID.FE_ID = iv.FE_ID;
                    nid = iv.FE_ID;
                    FeID.Country = iv.Country;
                    FeID.ZipCode_pincode = iv.ZipCode_pincode;

                    //FeID.ID_Upload = utlity.ConvertToImage(iv.ID_Upload);
                    FeID.CreatedBy = User.Identity.GetUserId();
                    FeID.CreatedOn = DateTime.Now;
                    db.FE_Master_serviceArea.Add(FeID);
                    db.SaveChanges();
                }
                TempData["message"] = "Data Updated !! ";
                return RedirectToAction("FEPersonal", "Master", new { id = nid });
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEPersonal", "Master", new { id = nid });
            }
        }
        public ActionResult DeleteFeCharges(int FeId, string Country)
        {

            if (FeId > 0 && Country != "")
            {
                int nid = FeId;

                FE_Master_serviceArea FI = new FE_Master_serviceArea();
                FI = (from c in db.FE_Master_serviceArea where c.FE_ID == FeId && c.Country == Country select c).FirstOrDefault();
                if (FI != null)
                {
                    db.Entry(FI).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    return RedirectToAction("FEPersonal", "Master", new { id = FeId });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEPersonal", "Master", new { id = FeId });
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addFECharges(FE_Master_Charges sa)
        {
            try
            {

                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.FE_Master_Charges.Add(sa);
                db.SaveChanges();


                TempData["link"] = "Yes";
                TempData["message"] = "FE Charges Added !!";

            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Charges Not Created !! ";
            }
            return RedirectToAction("FECharges", "Master", new { id = sa.FE_ID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditFECharges(FE_Master_Charges sa)
        {
            try
            {

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "FE Charges Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Charges Not Updated !! ";
            }
            return RedirectToAction("FECharges", "Master", new { id = sa.FE_ID });
        }
        public ActionResult FEFinancial(int id)
        {
            // To check user access right
            string uri1 = "/Master/FEMaster";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends
            FE_Master_Financial sa = new FE_Master_Financial();

            int cnt = (from s in db.FE_Master_Financial where s.FE_ID == id select s).Count();
            if (cnt != 0)
            {
                sa = (from s in db.FE_Master_Financial where s.FE_ID == id select s).FirstOrDefault();

                ViewBag.Edit = "Yes";
                return View(sa);
            }
            else
            {
                ViewBag.FE = id;
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addFEFinancial(FE_Master_Financial sa)
        {
            try
            {

                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.FE_Master_Financial.Add(sa);
                db.SaveChanges();


                TempData["link"] = "Yes";
                TempData["message"] = "FE Financia Added !!";

            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Financia Not Created !! ";
            }
            return RedirectToAction("FEFinancial", "Master", new { id = sa.FE_ID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditFEFinancial(FE_Master_Financial sa)
        {
            try
            {

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "FE Financia Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Financia Not Updated !! ";
            }
            return RedirectToAction("FEFinancial", "Master", new { id = sa.FE_ID });
        }
        public ActionResult FESkill(int? id)
        {
            // To check user access right
            string uri1 = "/Master/FEMaster";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();
            ManageSkill MS = new ManageSkill();
            MS.IV = new List<Models.InsertCertificate>();
            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends
            //FE_Master_Skill sa = new FE_Master_Skill();

            int cnt = (from s in db.FE_Master_Skill where s.FE_ID == id select s).Count();
            ViewBag.FE = id;
            if (cnt != 0)
            {
                MS.IC = (from c in db.FE_Master_Certification join mc in db.Certification_Master on c.Certification_Name equals mc.Id where c.FE_ID == id select new InsertCertificateView { Id = c.Certification_Name, FE_ID = c.FE_ID, Certification_Upload = c.Certification_Upload, Certification_Name = mc.Certification_Name }).ToList();
                if (id.HasValue)
                {
                    MS.Id = id.Value;
                }

                MS.FE = (from s in db.FE_Master_Skill where s.FE_ID == id select s).FirstOrDefault();

                ViewBag.Edit = "Yes";
                ViewBag.Certification = (from c in db.Certification_Master where c.Status == 1 select new SelectListItem { Text = c.Certification_Name, Value = SqlFunctions.StringConvert((double)c.Id).Trim() }).ToList();
                ViewBag.Exp = (from c in db.HeaderDescription where c.header_id == 12 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
                return View(MS);
            }
            else
            {
                if (id.HasValue)
                {
                    MS.Id = id.Value;
                }
                MS.IC = (from c in db.FE_Master_Certification join mc in db.Certification_Master on c.Certification_Name equals mc.Id where c.FE_ID == id select new InsertCertificateView { Id = c.Certification_Name, FE_ID = c.FE_ID, Certification_Upload = c.Certification_Upload, Certification_Name = mc.Certification_Name }).ToList();

                ViewBag.Certification = (from c in db.Certification_Master where c.Status == 1 select new SelectListItem { Text = c.Certification_Name, Value = SqlFunctions.StringConvert((double)c.Id).Trim() }).ToList();
                ViewBag.Exp = (from c in db.HeaderDescription where c.header_id == 12 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
                return View(MS);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addFESkill(FE_Master_Skill sa, HttpPostedFileBase pht)
        {
            try
            {
                string pic = "";


                if (pht != null)
                {

                    pic = System.IO.Path.GetFileName(pht.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Resume"), fileName);
                    pht.SaveAs(pathtest);

                    sa.Bio_Data = url + "Resume/" + fileName;
                }

                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.FE_Master_Skill.Add(sa);
                db.SaveChanges();


                TempData["link"] = "Yes";
                TempData["message"] = "FE Skill Added !!";

            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Skill Not Created !! ";
            }
            return RedirectToAction("FESkill", "Master", new { id = sa.FE_ID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditFESkill(FE_Master_Skill sa, HttpPostedFileBase pht)
        {
            try
            {
                string pic = "";
                string ct = "";
                if (pht != null)
                {

                    pic = System.IO.Path.GetFileName(pht.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Resume"), fileName);
                    pht.SaveAs(pathtest);

                    sa.Bio_Data = url + "Resume/" + fileName;
                }


                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "FE Skill Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Skill Not Updated !! ";
            }
            return RedirectToAction("FESkill", "Master", new { id = sa.FE_ID });
        }

        public ActionResult InsertCertificate(List<InsertCertificate> IV)
        {

            if (IV != null)
            {
                int nid = 0;

                FE_Master_Certification FeID = new FE_Master_Certification();

                //Loop and insert records.
                foreach (InsertCertificate iv in IV)
                {
                    FeID.FE_ID = iv.FE_ID;
                    nid = iv.FE_ID;
                    FeID.Certification_Name = iv.Certification_Name;

                    string pic = "IDType_";
                    if (iv.Certification_Upload != null)
                    {

                        pic = System.IO.Path.GetFileName(iv.Certification_Upload.FileName);
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;

                        string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Certificate"), fileName);
                        iv.Certification_Upload.SaveAs(pathtest);

                        FeID.Certification_Upload = url + "Certificate/" + fileName;
                    }
                    //FeID.ID_Upload = utlity.ConvertToImage(iv.ID_Upload);
                    FeID.CreatedBy = User.Identity.GetUserId();
                    FeID.CreatedOn = DateTime.Now;
                    db.FE_Master_Certification.Add(FeID);
                    db.SaveChanges();
                }
                TempData["message"] = "Data Updated !! ";
                return RedirectToAction("FEPersonal", "Master", new { id = nid });
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEPersonal", "Master");
            }
        }
        public ActionResult DeleteCertificate(int FeId, int IdType)
        {
            string change = " <table style='width: 100 %; height: 40px;padding-left:20px; border:1px;'><tr style='background: rgb(131,194, 51);'><td>Fields</td><td>Old Data<td>New Data</td></tr>";

            if (FeId > 0 && IdType > 0)
            {
                int nid = FeId;
                FE_Master_Personal sa = new FE_Master_Personal();
                sa = (from s in db.FE_Master_Personal where s.Id == FeId select s).FirstOrDefault();

                FE_Master_Certification FI = new FE_Master_Certification();
                FI = (from c in db.FE_Master_Certification where c.FE_ID == FeId && c.Certification_Name == IdType select c).FirstOrDefault();
                string certiname = (from s in db.Certification_Master where s.Id == FI.Certification_Name select s.Certification_Name).FirstOrDefault();
                if (FI != null)
                {
                    db.Entry(FI).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    change = change + "<tr><td> Certification Name </td><td>" + certiname + "</td><td> Data Deleted</td></tr></table>";
                    string body = utlity.Changelog(sa.First_Name + " " + sa.Last_Name, change);

                    utlity.sendmailAcc(sa.Email, "Data Deleted", body, "Data Deleted<support@inwinteck.com>");
                    return RedirectToAction("FEPersonal", "Master", new { id = UrlEncryptionHelper.Encrypt(FeId) });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEPersonal", "Master", new { id = UrlEncryptionHelper.Encrypt(FeId) });
            }

            return View();
        }
        public ActionResult FEService(int id)
        {
            // To check user access right
            //string uri1 = "/Master/FEService";

            //int bid = utlity.GetBussinessId(uri1);
            //string ucheckid = User.Identity.GetUserId();

            //checkpermission cm = new checkpermission();

            //cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            //if (cm == null)
            //{
            //    return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            //}
            //if (cm.record_insert == false)
            //{
            //    return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            //}
            // access right ends
            FE_Master_serviceArea sa = new FE_Master_serviceArea();

            int cnt = (from s in db.FE_Master_serviceArea where s.FE_ID == id select s).Count();
            if (cnt != 0)
            {
                sa = (from s in db.FE_Master_serviceArea where s.FE_ID == id select s).FirstOrDefault();

                ViewBag.Edit = "Yes";
                return View(sa);
            }
            else
            {
                ViewBag.FE = id;
                ViewBag.Certification = (from c in db.Certification_Master where c.Status == 1 select new SelectListItem { Text = c.Certification_Name, Value = SqlFunctions.StringConvert((double)c.Id).Trim() }).ToList();
                ViewBag.Exp = (from c in db.HeaderDescription where c.header_id == 12 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addFEService(FE_Master_Skill sa)
        {

            return RedirectToAction("FEService", "Master", new { id = sa.FE_ID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditFEService(FE_Master_Skill sa, HttpPostedFileBase pht, string[] Cert)
        {
            try
            {
                string pic = "";
                string ct = "";
                if (pht != null)
                {

                    pic = System.IO.Path.GetFileName(pht.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Resume"), fileName);
                    pht.SaveAs(pathtest);

                    sa.Bio_Data = url + "Resume/" + fileName;
                }

                foreach (var i in Cert)
                {
                    ct = ct + i + ",";
                }

                ct = ct.Remove(ct.Length - 1);
                sa.Certification = ct;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "FE Skill Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Skill Not Updated !! ";
            }
            return RedirectToAction("FEService", "Master", new { id = sa.FE_ID });
        }

        public ActionResult InsertSkills(List<InsertSkills> IV)
        {


            int nid = 0;

            FE_Master_Skill_Data FeID = new FE_Master_Skill_Data();

            //Loop and insert records.
            foreach (InsertSkills iv in IV)
            {
                FeID.FE_ID = iv.FE_ID;
                nid = iv.FE_ID;
                FeID.Skill = iv.Skill;
                FeID.Exp = iv.Exp;
                FeID.CreatedBy = User.Identity.GetUserId();
                FeID.CreatedOn = DateTime.Now;
                db.FE_Master_Skill_Data.Add(FeID);
                db.SaveChanges();
            }
            TempData["message"] = "Data Updated !! ";
            return RedirectToAction("FEPersonal", "Master", new { id = nid });

        }
        public ActionResult DeleteSkills(int FeId, int IdType)
        {

            if (FeId > 0 && IdType > 0)
            {
                int nid = FeId;

                FE_Master_Skill_Data FI = new FE_Master_Skill_Data();
                FI = (from c in db.FE_Master_Skill_Data where c.Id == IdType select c).FirstOrDefault();
                if (FI != null)
                {
                    db.Entry(FI).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    return RedirectToAction("FEPersonal", "Master", new { id = FeId });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEPersonal", "Master", new { id = FeId });
            }

            return View();
        }
        public ActionResult statusFEMaster(int id)
        {
            // To check user access right
            string uri1 = "/Master/FEMaster";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            FE_Master_Personal sa = new FE_Master_Personal();
            sa = (from s in db.FE_Master_Personal where s.Id == id select s).FirstOrDefault();
            if (sa.Status == 1)
            {
                sa.Status = 0;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            else
            {
                sa.Status = 1;
            }
            db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.Message = string.Format("Deactivated");
            return RedirectToAction("FEMaster", "Master");
        }


        public ActionResult EUMaster(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
            // To check user access right
            string uri1 = "/Master/EUMaster";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            ViewBag.Permission = cm;
            // access right ends 
            int pageSize = 10;
            int pgc = 0;
            int pageCount = 0;
            List<EU_Master> li = new List<EU_Master>();
            if (searchtype != "" && searchtext != "")
            {
                if (searchtype == "Name")
                {
                    pgc = db.EU_Master.Where(x => x.Customer_Name.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.EU_Master
                          orderby c.Customer_Name
                          select c).Where(x => x.Customer_Name.StartsWith(searchtext)).ToList();
                }
                else if (searchtype == "Email")
                {
                    pgc = db.EU_Master.Where(x => x.Customer_Email.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.EU_Master
                          orderby c.Customer_Email
                          select c).Where(x => x.Customer_Email.StartsWith(searchtext)).ToList();
                }
                ViewBag.Search = searchtext;

            }
            else
            {
                pgc = db.EU_Master.Count();
                pageCount = pgc / pageSize;

                li = (from s in db.EU_Master orderby s.Customer_Name select s).Skip(pageNo * pageSize).Take(pageSize).ToList();
                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }

            return View(li);
        }
        public ActionResult addEUMaster()
        {
            // To check user access right
            string uri1 = "/Master/EUMaster";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 

            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addEUMaster(EU_Master sa)
        {
            try
            {
                int cnt = (from s in db.EU_Master where s.Customer_Email == sa.Customer_Email select s).Count();
                int cnt1 = (from s in db.EU_Master where s.Ticket_Abv == sa.Ticket_Abv select s).Count();
                if (cnt > 0)
                {
                    TempData["message"] = "EU already Register with us !!";
                    return RedirectToAction("addEUMaster", "Master");
                }
                else if (cnt1 > 0)
                {
                    TempData["message"] = "Ticket Abbreviation already Taken !!";
                    return RedirectToAction("addEUMaster", "Master");
                }
                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.EU_Master.Add(sa);
                db.SaveChanges();


                TempData["link"] = "Yes";
                TempData["message"] = "EU Master Added !!";

            }
            catch (Exception ex)
            {
                TempData["message"] = "EU Master Not Created !! ";
            }
            return RedirectToAction("EditEUMaster", "Master", new { id = sa.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult OfficeEUMaster(EU_Master_Branch sa, int OID)
        {
            if (OID != 0)
            {
                try
                {
                    sa.Id = OID;
                    sa.CreatedBy = (from s in db.EU_Master_Branch where s.Id == OID select s.CreatedBy).FirstOrDefault();
                    sa.CreatedOn = (from s in db.EU_Master_Branch where s.Id == OID select s.CreatedOn).FirstOrDefault();
                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    TempData["message"] = "EU Master's Office Updated !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "EU Master's Office Not Updated !! ";
                }
            }
            else
            {
                try
                {
                    sa.CreatedBy = User.Identity.GetUserId();
                    sa.CreatedOn = DateTime.Now;
                    db.EU_Master_Branch.Add(sa);
                    db.SaveChanges();


                    TempData["message"] = "EU Master's Office Added !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "EU Master's Office Not Created !! ";
                }
            }

            return RedirectToAction("EditEUMaster", "Master", new { id = sa.EU_ID });
        }

        public ActionResult deleteOffice(int OD, int ED)
        {

            if (OD > 0)
            {

                EU_Master_Branch sa = new EU_Master_Branch();
                sa = (from c in db.EU_Master_Branch where c.Id == OD select c).FirstOrDefault();
                if (sa != null)
                {
                    db.Entry(sa).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    return RedirectToAction("EditEUMaster", "Master", new { id = ED });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("EditEUMaster", "Master", new { id = ED });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ContactEUMaster(EU_Master_Contacts sa, int Contact_ID, int Contact_EU_ID)
        {
            if (Contact_ID != 0)
            {
                try
                {
                    sa.Id = Contact_ID;
                    sa.CreatedBy = (from s in db.EU_Master_Contacts where s.Id == Contact_ID select s.CreatedBy).FirstOrDefault();
                    sa.CreatedOn = (from s in db.EU_Master_Contacts where s.Id == Contact_ID select s.CreatedOn).FirstOrDefault();
                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    TempData["message"] = "EU Master's Contact Updated !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "EU Master's Contact Not Updated !! ";
                }
            }
            else
            {
                try
                {
                    sa.CreatedBy = User.Identity.GetUserId();
                    sa.CreatedOn = DateTime.Now;
                    db.EU_Master_Contacts.Add(sa);
                    db.SaveChanges();


                    TempData["message"] = "EU Master's Contact Added !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "EU Master's Contact Not Created !! ";
                }
            }

            return RedirectToAction("EditEUMaster", "Master", new { id = Contact_EU_ID });
        }

        public ActionResult deleteContact(int OC)
        {
            int od = (from s in db.EU_Master_Contacts where s.Id == OC select s.Office_ID).FirstOrDefault();
            int ED = (from s in db.EU_Master_Branch where s.Id == od select s.EU_ID).FirstOrDefault();
            if (OC > 0)
            {

                EU_Master_Contacts sa = new EU_Master_Contacts();
                sa = (from c in db.EU_Master_Contacts where c.Id == OC select c).FirstOrDefault();
                if (sa != null)
                {
                    db.Entry(sa).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    return RedirectToAction("EditEUMaster", "Master", new { id = ED });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("EditEUMaster", "Master", new { id = ED });
            }

            return View();
        }
        public ActionResult EditEUMaster(int id)
        {
            // To check user access right
            string uri1 = "/Master/EUMaster";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends
            EU_Master sa = new EU_Master();
            sa = (from s in db.EU_Master where s.Id == id select s).FirstOrDefault();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();
            ViewBag.Office = (from s in db.EU_Master_Branch where s.EU_ID == sa.Id select s).ToList();
            ViewBag.Office_Name = (from c in db.EU_Master_Branch where c.EU_ID == sa.Id select new SelectListItem { Text = c.Office, Value = SqlFunctions.StringConvert((double)c.Id).TrimStart() }).ToList();

            return View(sa);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditEUMaster(EU_Master sa)
        {
            try
            {

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "EU Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "EU Not Updated !! ";
            }
            return RedirectToAction("EditEUMaster", "Master", new { id = sa.Id });
        }
        public ActionResult viewEUMaster(int id)
        {
            // To check user access right
            string uri1 = "/Master/EUMaster";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_view == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends
            EU_Master sa = new EU_Master();
            sa = (from s in db.EU_Master where s.Id == id select s).FirstOrDefault();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();

            return View(sa);
        }

        public ActionResult statusEUMaster(int id)
        {
            // To check user access right
            string uri1 = "/Master/EUMaster";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            EU_Master sa = new EU_Master();
            sa = (from s in db.EU_Master where s.Id == id select s).FirstOrDefault();
            if (sa.Status == 1)
            {
                sa.Status = 0;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            else
            {
                sa.Status = 1;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EUMaster", "Master");
        }

        //===================== Certification Master Code ------------------------------
        public ActionResult Certification(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
            // To check user access right
            string uri1 = "/Master/Certification";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            ViewBag.Permission = cm;
            // End user access right
            List<certification_detail> CM = new List<certification_detail>();

            int pageSize = 10;
            int pgc = 0;
            int pageCount = 0;
            if (searchtype != "" && searchtext != "")
            {

                ViewBag.Search = searchtext;
            }
            else
            {
                pgc = db.Certification_Master.Count();
                pageCount = pgc / pageSize;

                // CM = (from s in db.Country_Dialing_Code orderby s.Country select s).Skip(pageNo * pageSize).Take(pageSize).ToList();
                CM = (from s in db.Certification_Master
                      join a in db.HeaderDescription on s.OEM equals a.id
                      orderby s.Id descending
                      select new certification_detail { Id = s.Id, OEM = a.header_description, Certification_Name = s.Certification_Name, Status = s.Status }).ToList();
                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }

            return View(CM);
        }


        public ActionResult addCertification()
        {
            // To check user access right
            string uri1 = "/Master/Certification";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            ViewBag.OEM = (from c in db.HeaderDescription where c.header_id == 4 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addCertification(Certification_Master cert)
        {
            try
            {
                if (cert != null)
                {
                    cert.CreatedOn = DateTime.Now;
                    cert.CreatedBy = User.Identity.GetUserId();
                    db.Certification_Master.Add(cert);
                    db.SaveChanges();
                    TempData["message"] = "Data Saved !!";
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("addCertification", "Master");
        }
        public ActionResult editCertification(int Id)
        {

            // To check user access right
            string uri1 = "/Master/Certification";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            Certification_Master CM = new Certification_Master();
            CM = (from s in db.Certification_Master where s.Id == Id select s).FirstOrDefault();
            ViewBag.OEM = (from c in db.HeaderDescription where c.header_id == 4 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            return View(CM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult editCertification(Certification_Master CM)
        {
            try
            {
                if (CM != null)
                {
                    CM.ModifiedBy = User.Identity.GetUserId();
                    CM.ModifiedOn = DateTime.Now;
                    db.Entry(CM).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["message"] = "Data Saved !!";
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("editCertification", "Master", new { Id = CM.Id });
        }

        public ActionResult statusCertification(int id)
        {
            // To check user access right
            string uri1 = "/Master/Certification";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            Certification_Master sa = new Certification_Master();
            sa = (from s in db.Certification_Master where s.Id == id select s).FirstOrDefault();
            if (sa.Status == 1)
            {
                sa.Status = 0;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            else
            {
                sa.Status = 1;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.Message = string.Format("Deactivated");
            return RedirectToAction("Certification", "Master");
        }
        public ActionResult viewCertification(int Id)
        {
            // To check user access right
            string uri1 = "/Master/Certification";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_view == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // End user access right
            Certification_Master CM = new Certification_Master();
            CM = (from s in db.Certification_Master where s.Id == Id select s).FirstOrDefault();
            ViewBag.OEM = (from c in db.HeaderDescription where c.header_id == 4 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            return View(CM);
        }
        //----------------------- Certification Master Ends ---------------------------

        //----------------------- EU Ratesheet Starts ---------------------------

        public ActionResult EU_Ratesheet(int id, int? eid)
        {
            EditEU_Ratecard sa = new EditEU_Ratecard();
            ViewBag.EU = (from s in db.EU_Master where s.Id == id select s.Customer_Name).FirstOrDefault();
            ViewBag.id = id;
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();

            sa.ERC = (from s in db.EU_Rate_Card where s.EU_ID == id select s).ToList();

            if (sa.ERC.Count > 0)
            {
                ViewBag.List = "Show";


            }

            if (eid != null)
            {
                sa.EERC = (from s in db.EU_Rate_Card where s.Id == eid select s).FirstOrDefault();
                ViewBag.Edit = "Show";
            }

            return View(sa);
        }


        [HttpPost]
        public ActionResult addEU_Ratesheet(EU_Rate_Card sa)
        {
            int cnt = (from s in db.EU_Rate_Card where s.EU_ID == sa.EU_ID && s.Country == sa.Country select s).Count();
            try
            {
                if (cnt > 0)
                {

                    TempData["message"] = "You have already entered rate for this country !!";
                }
                else
                {
                    sa.CreatedOn = DateTime.Now;
                    sa.CreatedBy = User.Identity.GetUserId();
                    db.EU_Rate_Card.Add(sa);
                    db.SaveChanges();
                    TempData["message"] = "Data Saved !!";
                }


            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("EU_Ratesheet", "Master", new { id = sa.EU_ID });
        }

        [HttpPost]
        public ActionResult editEU_Ratesheet(EU_Rate_Card sa)
        {
            try
            {
                sa.ModifiedOn = DateTime.Now;
                sa.ModifiedBy = User.Identity.GetUserId();
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["message"] = "Data Updated !!";



            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("EU_Ratesheet", "Master", new { id = sa.EU_ID });
        }
        public ActionResult DeleteEU_Ratesheet(int id, int euid)
        {
            EU_Rate_Card sa = new EU_Rate_Card();
            try
            {

                sa = (from s in db.EU_Rate_Card where s.Id == id select s).FirstOrDefault();
                db.Entry(sa).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                TempData["message"] = "Rate of " + sa.Country + " Deleted !!";
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }

            return RedirectToAction("EU_Ratesheet", "Master", new { id = euid });
        }

        //----------------------- EU Ratesheet Ends ---------------------------

        //----------------------- FE Profile Starts ---------------------------
        [HttpGet]
        public ActionResult BulksFEPersonal()
        {


            var fe = (from s in db.blkFEMaster select s).ToList();
            int cc = 0;
            foreach (blkFEMaster fp in fe)
            {
                try
                {
                    int cnt = (from s in db.FE_Master_Personal where s.Email.Trim() == fp.EmailId.Trim() select s).Count();
                    int cnts = (from s in db.Users where s.Email.Trim() == fp.EmailId.Trim() select s).Count();
                    if (cnt > 0)
                    {

                        string msg1 = "/BlukUpload/FEMaster/log" + " " + "Company: " + fp.Company + " " + "No: " + cc + " " + " Status: Error :: FE already Registered";
                        utlity.createlog(msg1);


                        blkFEMaster blkFI = new blkFEMaster();
                        blkFI = (from c in db.blkFEMaster where c.EmailId == fp.EmailId select c).FirstOrDefault();


                        if (blkFI != null)
                        {
                            db.Entry(blkFI).State = System.Data.Entity.EntityState.Deleted;
                            db.SaveChanges();
                        }

                    }
                    else if (cnts > 0)
                    {
                        string msg1 = "/BlukUpload/FEMaster/log" + " " + "Company: " + fp.Company + " " + "No: " + cc + " " + " Status: Error :: User Email ID Already Registered";
                        utlity.createlog(msg1);

                        blkFEMaster blkFI = new blkFEMaster();
                        blkFI = (from c in db.blkFEMaster where c.EmailId == fp.EmailId select c).FirstOrDefault();


                        if (blkFI != null)
                        {
                            db.Entry(blkFI).State = System.Data.Entity.EntityState.Deleted;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        string strAddress = fp.Address;
                        string key = "AIzaSyBvhQbrULBXqk9wPb9RjTeacVk3A3g-ci8";

                        string url = "https://maps.google.com/maps/api/geocode/xml?address=" + strAddress + "&key=" + key;

                        string[] lantylngo = utlity.GetdtLatLong(url);

                        string lang = lantylngo[0];
                        string lng = lantylngo[1];

                        if (lang == "None")
                        {

                            string msg = "/BlukUpload/FEMaster/log" + " " + "Company: " + fp.Company + " Status: Error In Address";
                            utlity.createlog(msg);
                        }
                        else
                        {
                            FE_Master_Personal sa = new FE_Master_Personal();
                            sa.First_Name = fp.Company.Trim();
                            sa.Country = fp.Country.Trim();
                            sa.City = fp.City.Trim();
                            sa.Street_Address = fp.Address.Trim();
                            sa.Company_Name = fp.Company.Trim();
                            sa.FE_Type = 345;
                            sa.Phone_Number = fp.Contact.Trim();
                            sa.Email = fp.EmailId.Trim();
                            sa.Status = 0;
                            sa.latitude = lang;
                            sa.longitude = lng;
                            sa.InwinFEID = utlity.FEInwinteckId(fp.Country);
                            sa.CreatedBy = "27b3bb6b-9331-4470-8026-d02e9125bab4";
                            sa.CreatedOn = fp.Date;
                            db.FE_Master_Personal.Add(sa);
                            db.SaveChanges();


                            // User Creation

                            string password = Membership.GeneratePassword(8, 1);
                            try
                            {
                                UserRegistrationView model = new UserRegistrationView();
                                model.Password = password;
                                var user = new ApplicationUser { UserName = sa.Email.Trim(), Email = sa.Email.Trim() };

                                user.Name = sa.First_Name + " " + sa.Last_Name;
                                user.CreatedBy = User.Identity.GetUserId();
                                user.CreatedOn = DateTime.Now;
                                user.ChangePassword = 1;
                                user.Status = 1;

                                IdentityResult result = UserManager.Create(user, model.Password);
                                if (result.Succeeded)
                                {
                                    UserManager.AddToRole(user.Id, "Field Engineer");
                                    ModelState.Clear();
                                    TempData["message"] = "User Created !!";
                                }
                                else
                                {
                                    foreach (var error in result.Errors)
                                    {
                                        TempData["message"] = error;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                TempData["message"] = "Error Occured While Saving !!" + ex.ToString();
                            }

                            cc = cc + 1;
                            string msg = "/BlukUpload/FEMaster/log" + " " + "Company: " + fp.Company + " " + "No: " + cc + " " + " Status: Success";
                            utlity.createlog(msg);

                            blkFEMaster blkFI = new blkFEMaster();
                            blkFI = (from c in db.blkFEMaster where c.EmailId == fp.EmailId select c).FirstOrDefault();


                            if (blkFI != null)
                            {
                                db.Entry(blkFI).State = System.Data.Entity.EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    string msg = "/BlukUpload/FEMaster/log" + " " + "Company: " + fp.Company + " " + "No: " + cc + " " + " Status: Error :: " + ex.InnerException.ToString();
                    utlity.createlog(msg);
                }
            }



            return View();
        }




        [HttpGet]
        public ActionResult BulksFEPersonalID()
        {
            try
            {
                var fe = (from s in db.FE_Master_Personal where s.InwinFEID == null && s.Country != null select s).ToList();

                foreach (FE_Master_Personal fp in fe)
                {

                    try
                    {
                        FE_Master_Personal fp1 = new FE_Master_Personal();

                        fp1 = (from s in db.FE_Master_Personal where s.Id == fp.Id select s).FirstOrDefault();
                        if (fp1 != null)
                        {
                            fp1.InwinFEID = utlity.FEInwinteckId(fp1.Country);
                            fp1.ModifiedBy = User.Identity.GetUserId();
                            fp1.ModifiedOn = DateTime.Now;
                            db.Entry(fp1).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                    catch (Exception ex)
                    {
                        TempData["message"] = "Error Occured While Saving !!" + ex.ToString();
                    }

                }


            }
            catch (Exception ex)
            {
            }
            return View();
        }

        [HttpGet]
        public ActionResult BulksFEPersonallatlng()
        {
            try
            {
                var fe = (from s in db.FE_Master_Personal where s.longitude == null && s.latitude == null select s).ToList();

                foreach (FE_Master_Personal fp in fe)
                {

                    try
                    {
                        string strAddress = fp.Street_Address + "," + fp.Country;
                        string key = "AIzaSyBvhQbrULBXqk9wPb9RjTeacVk3A3g-ci8";

                        string url = "https://maps.google.com/maps/api/geocode/xml?address=" + strAddress + "&key=" + key;

                        string[] lantylngo = utlity.GetdtLatLong(url);

                        string lang = lantylngo[0];
                        string lng = lantylngo[1];
                        FE_Master_Personal fp1 = new FE_Master_Personal();

                        fp1 = (from s in db.FE_Master_Personal where s.Id == fp.Id select s).FirstOrDefault();
                        if (fp1 != null)
                        {
                            fp1.latitude = lang;
                            fp1.longitude = lng;
                            fp1.ModifiedBy = User.Identity.GetUserId();
                            fp1.ModifiedOn = DateTime.Now;
                            db.Entry(fp1).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                    catch (Exception ex)
                    {
                        TempData["message"] = "Error Occured While Saving !!" + ex.ToString();
                    }

                }


            }
            catch (Exception ex)
            {
            }
            return View();
        }

        public ActionResult FEProfile(string Id)
        {

            int id = UrlEncryptionHelper.Decrypt(Id);
            int per = 25;
            ManageFePersonal MP = new ManageFePersonal();
            // To check user access right
            string uri1 = "/Master/FEProfile";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends

            ViewBag.Language = (from c in db.HeaderDescription where c.header_id == 1 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).OrderBy(x => x.Text).ToList();
            ViewBag.Identification = (from c in db.HeaderDescription where c.header_id == 2 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Chat_mode = (from c in db.HeaderDescription where c.header_id == 3 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Citizen = (from c in db.HeaderDescription where c.header_id == 8 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.FE_Type = (from c in db.HeaderDescription where c.header_id == 9 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.DialCode = (from c in db.Country_Dialing_Code where c.Status == 1 select new SelectListItem { Text = c.Code + " " + c.Country, Value = c.Code.Trim() }).ToList();
            ViewBag.Currency = (from s in db.Currency_Master
                                where
                                s.Country == (from a in db.FE_Master_Personal where a.Id == id select a.Country).FirstOrDefault() && s.Currency != "USD"
                                select new SelectListItem { Text = s.Currency, Value = SqlFunctions.StringConvert((double)s.Id).Trim() }).ToList();
            ViewBag.Certification = (from c in db.Certification_Master where c.Status == 1 select new SelectListItem { Text = c.Certification_Name, Value = SqlFunctions.StringConvert((double)c.Id).Trim() }).ToList();
            ViewBag.Exp = (from c in db.HeaderDescription where c.header_id == 12 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Skill = (from c in db.HeaderDescription where c.header_id == 18 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();


            MP.IV = new List<IdentificationInsertView>();

            MP.FE = (from s in db.FE_Master_Personal where s.Id == id select s).FirstOrDefault();
            MP.IF = (from s in db.FE_Master_Identification
                     join hd in db.HeaderDescription on s.ID_Type equals hd.id
                     where s.FE_ID == id
                     select new IdentificationInsertFill
                     {
                         FE_ID = s.FE_ID,
                         ID_Number = s.ID_Number,
                         ID_Type = hd.header_description,
                         Upload = s.ID_Upload,
                         IdType = s.ID_Type
                     }).ToList();
            ViewBag.Edit = "Yes";

            if (MP.FE.NDA_Accept == 1)
            {
                per = 60;
            }

            if (MP.FE.latitude != null)
            {
                TempData["geo"] = "Yes";
            }
            else
            {
                TempData["geo"] = null;
            }
            //========================================== Charges & Services ===========================================================
            int cntc = (from s in db.FE_Master_Charges where s.FE_ID == id select s).Count();

            if (cntc != 0)
            {
                MP.FEC = (from s in db.FE_Master_Charges where s.FE_ID == id select s).FirstOrDefault();
                MP.ICS = (from c in db.FE_Master_serviceArea where c.FE_ID == id select new InsertServiceArea { FE_ID = c.FE_ID, Country = c.Country, ZipCode_pincode = c.ZipCode_pincode }).ToList();
                ViewBag.ChargEdit = "Yes";
                ViewBag.ChargesPer = 1;
                per = per + 15;
            }
            else
            {
                MP.FEC = (from s in db.FE_Master_Charges where s.FE_ID == id select s).FirstOrDefault();
                MP.ICS = (from c in db.FE_Master_serviceArea where c.FE_ID == id select new InsertServiceArea { FE_ID = c.FE_ID, Country = c.Country, ZipCode_pincode = c.ZipCode_pincode }).ToList();
                ViewBag.ChargesPer = 0;
                per = per + 0;
            }

            //============================================== Financial ===============================================================

            int cntf = (from s in db.FE_Master_Financial where s.FE_ID == id select s).Count();

            if (cntf != 0)
            {
                MP.FEF = (from s in db.FE_Master_Financial where s.FE_ID == id select s).FirstOrDefault();

                ViewBag.Editf = "Yes";
            }
            else
            {
            }

            //=============================================== Skill ==================================================================

            int cnts = (from s in db.FE_Master_Skill where s.FE_ID == id select s).Count();
            if (cnts != 0)
            {
                MP.ICCE = (from c in db.FE_Master_Certification join mc in db.Certification_Master on c.Certification_Name equals mc.Id where c.FE_ID == id select new InsertCertificateView { Id = c.Certification_Name, FE_ID = c.FE_ID, Certification_Upload = c.Certification_Upload, Certification_Name = mc.Certification_Name }).ToList();

                MP.FES = (from s in db.FE_Master_Skill where s.FE_ID == id select s).FirstOrDefault();
                MP.ISV = (from s in db.FE_Master_Skill_Data join a in db.HeaderDescription on s.Skill equals a.id.ToString() join b in db.HeaderDescription on s.Exp equals b.id.ToString() where s.FE_ID == id select new InsertSkillsView { Id = s.Id, FE_ID = s.FE_ID, Skill_Name = a.header_description, Exp_Upload = b.header_description }).ToList();
                ViewBag.EditS = "Yes";
                ViewBag.SkillPer = 1;
                per = per + 15;
            }
            else
            {
                per = per + 0;
                ViewBag.SkillPer = 0;
                MP.ICCE = (from c in db.FE_Master_Certification join mc in db.Certification_Master on c.Certification_Name equals mc.Id where c.FE_ID == id select new InsertCertificateView { Id = c.Certification_Name, FE_ID = c.FE_ID, Certification_Upload = c.Certification_Upload, Certification_Name = mc.Certification_Name }).ToList();
                MP.ISV = (from s in db.FE_Master_Skill_Data join a in db.HeaderDescription on s.Skill equals a.id.ToString() join b in db.HeaderDescription on s.Exp equals b.id.ToString() where s.FE_ID == id select new InsertSkillsView { Id = s.Id, FE_ID = s.FE_ID, Skill_Name = a.header_description, Exp_Upload = b.header_description }).ToList();
            }
            ViewBag.Percentage = per;


            ViewBag.ResumePer = (from s in db.FE_Master_Skill where s.FE_ID == id select s.Bio_Data).FirstOrDefault();

            //------------------------------------------------------------- Activity Log 
            var ticketDetails = db.Ticket.Where((Func<Ticket, bool>)(s => s.FE_ID == id)).OrderByDescending(s => s.CreatedOn).ToList(); //(Func<Ticket, bool>): This is a cast to a delegate type. Func<Ticket, bool> is a delegate that takes a Ticket object as input and returns a bool
            ViewBag.TicketReceivedDetails = ticketDetails;
            ViewBag.TicketReceived = ticketDetails.Count;
            ViewBag.TicketNoOfReceived = ticketDetails.Select(t => t.Ticket_No).ToList();


            ViewBag.TicketExecuted = (from s in db.Ticket where s.FE_ID == id && s.Status == 20 select s).OrderByDescending(s => s.CreatedOn).Count();
            ViewBag.TicketExecutedNos = (from s in db.Ticket where s.FE_ID == id && s.Status == 20 select s.Ticket_No).ToList();

            ViewBag.TicketDenied = (from s in db.Ticket_FE_Selection where s.FE_ID == id && s.Status == "Reject" select s).Count();
           // ViewBag.TicketDeniedNos = (from s in db.Ticket_FE_Selection where s.FE_ID == id && s.Status == "Reject" select s.Ticket_No).ToList();


            FECSAT fc = new FECSAT();

            fc = db.Database.SqlQuery<FECSAT>("getFECSAT @id", new SqlParameter("@id", id)).FirstOrDefault();

            ViewBag.CSAT = fc.CSAT + "/5" + " [" + fc.Count + "]";

            //------------------------------------------------------------- Activity Log Ends


            return View(MP);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditFEProfile(ManageFePersonal sa, HttpPostedFileBase pht, HttpPostedFileBase Contract, HttpPostedFileBase BD, string addSkill, string addFin, string addChar, List<InsertSkills> IV, List<InsertCertificate> IVC, List<IdentificationInsertView> IVI)
        {
            try
            {
                string change = " <table style='width: 100 %; height: 40px;padding-left:20px; border:1px;'><tr style='background: rgb(131,194, 51);'><td>Changes</td><td>Old Data<td>New Data</td></tr>";
                string changeem = " <table style='width: 100 %; height: 40px;padding-left:20px; border:1px;'><tr style='background: rgb(131,194, 51);'><td>Changes</td><td>Old Data<td>New Data</td></tr>";
                string pic = "";
                string cont = "";
                string bd = "";

                //------------------------------------------------------------------- Email Log Starts

                FE_Master_Personal_Old FEMPOld = new FE_Master_Personal_Old();
                FEMPOld = (from s in db.FE_Master_Personal
                           where s.Id == sa.FE.Id
                           select new FE_Master_Personal_Old
                           {
                               Alt_Email = s.Alt_Email,
                               Alt_Phone_Number = s.Alt_Phone_Number,
                               Alt_Chat_Mode = s.Alt_Chat_Mode,
                               Alt_Chat_Mode_1 = s.Alt_Chat_Mode_1,
                               Alt_Chat_Mode_2 = s.Alt_Chat_Mode_2,
                               Alt_Phone_Number_1 = s.Alt_Phone_Number_1,
                               Alt_Phone_Number_2 = s.Alt_Phone_Number_2,
                               Alt_Phone_Number_Code = s.Alt_Phone_Number_Code,
                               Alt_Phone_Number_Code_1 = s.Alt_Phone_Number_Code_1,
                               Alt_Phone_Number_Code_2 = s.Alt_Phone_Number_Code_2,
                               Language_Spoken = s.Language_Spoken,
                               Language_Spoken_1 = s.Language_Spoken_1,
                               Language_Spoken_2 = s.Language_Spoken_2,
                               Manager_Name = s.Manager_Name,
                               Manager_Email = s.Manager_Email,
                               Manager_Phone_Number = s.Manager_Phone_Number
                           }).FirstOrDefault();

                if (FEMPOld.Alt_Email != sa.FE.Alt_Email)
                {
                    change = change + "<tr><td> Alternate Email </td><td>" + FEMPOld.Alt_Email + "</td><td>" + sa.FE.Alt_Email + "</td></tr>";
                }

                if (FEMPOld.Alt_Phone_Number_Code != sa.FE.Alt_Phone_Number_Code)
                {
                    change = change + "<tr><td> Alternate Phone Number code </td><td>" + FEMPOld.Alt_Phone_Number_Code + "</td><td>" + sa.FE.Alt_Phone_Number_Code + "</td></tr>";
                }

                if (FEMPOld.Alt_Phone_Number != sa.FE.Alt_Phone_Number)
                {
                    change = change + "<tr><td> Alternate Phone Number </td><td>" + FEMPOld.Alt_Phone_Number + "</td><td>" + sa.FE.Alt_Phone_Number + "</td></tr>";
                }

                if (FEMPOld.Alt_Chat_Mode != sa.FE.Alt_Chat_Mode)
                {
                    string alpnco = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Alt_Chat_Mode select o.header_description).FirstOrDefault();

                    string alpncn = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Alt_Chat_Mode select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Alternate Phone Number Code </td><td>" + alpnco + "</td><td>" + alpncn + "</td></tr>";
                }

                //-------------------------------------------------------------------

                if (FEMPOld.Alt_Phone_Number_Code_1 != sa.FE.Alt_Phone_Number_Code_1)
                {
                    change = change + "<tr><td> Alternate Phone Number code </td><td>" + FEMPOld.Alt_Phone_Number_Code_1 + "</td><td>" + sa.FE.Alt_Phone_Number_Code_1 + "</td></tr>";
                }

                if (FEMPOld.Alt_Phone_Number_1 != sa.FE.Alt_Phone_Number_1)
                {
                    change = change + "<tr><td> Alternate Phone Number </td><td>" + FEMPOld.Alt_Phone_Number_1 + "</td><td>" + sa.FE.Alt_Phone_Number_1 + "</td></tr>";
                }

                if (FEMPOld.Alt_Chat_Mode_1 != sa.FE.Alt_Chat_Mode_1)
                {
                    string alpnco1 = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Alt_Chat_Mode_1 select o.header_description).FirstOrDefault();

                    string alpncn1 = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Alt_Chat_Mode_1 select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Alternate Phone Number Code </td><td>" + alpnco1 + "</td><td>" + alpncn1 + "</td></tr>";
                }

                //-------------------------------------------------------------------

                if (FEMPOld.Alt_Phone_Number_Code_2 != sa.FE.Alt_Phone_Number_Code_2)
                {
                    change = change + "<tr><td> Alternate Phone Number code </td><td>" + FEMPOld.Alt_Phone_Number_Code_2 + "</td><td>" + sa.FE.Alt_Phone_Number_Code_2 + "</td></tr>";
                }

                if (FEMPOld.Alt_Phone_Number_2 != sa.FE.Alt_Phone_Number_2)
                {
                    change = change + "<tr><td> Alternate Phone Number </td><td>" + FEMPOld.Alt_Phone_Number_2 + "</td><td>" + sa.FE.Alt_Phone_Number_2 + "</td></tr>";
                }

                if (FEMPOld.Alt_Chat_Mode_2 != sa.FE.Alt_Chat_Mode_2)
                {
                    string alpnco2 = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Alt_Chat_Mode_2 select o.header_description).FirstOrDefault();

                    string alpncn2 = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Alt_Chat_Mode_2 select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Alternate Phone Number Code </td><td>" + alpnco2 + "</td><td>" + alpncn2 + "</td></tr>";
                }

                //------------------------------------------------------------------- Languag Log

                if (FEMPOld.Language_Spoken != sa.FE.Language_Spoken)
                {
                    string lano = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Language_Spoken select o.header_description).FirstOrDefault();

                    string lann = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Language_Spoken select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Language </td><td>" + lano + "</td><td>" + lann + "</td></tr>";
                }

                if (FEMPOld.Language_Spoken_1 != sa.FE.Language_Spoken_1)
                {
                    string lano1 = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Language_Spoken_1 select o.header_description).FirstOrDefault();

                    string lann1 = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Language_Spoken_1 select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Language </td><td>" + lano1 + "</td><td>" + lann1 + "</td></tr>";
                }

                if (FEMPOld.Language_Spoken_2 != sa.FE.Language_Spoken_2)
                {
                    string lano2 = (from o in db.HeaderDescription where o.id.ToString() == FEMPOld.Language_Spoken_2 select o.header_description).FirstOrDefault();

                    string lann2 = (from o in db.HeaderDescription where o.id.ToString() == sa.FE.Language_Spoken_2 select o.header_description).FirstOrDefault();

                    change = change + "<tr><td> Language </td><td>" + lano2 + "</td><td>" + lann2 + "</td></tr>";
                }

                if (FEMPOld.Manager_Name != sa.FE.Manager_Name)
                {
                    change = change + "<tr><td> Manager Name </td><td>" + FEMPOld.Manager_Name + "</td><td>" + sa.FE.Manager_Name + "</td></tr>";
                }

                if (FEMPOld.Manager_Phone_Number != sa.FE.Manager_Phone_Number)
                {
                    change = change + "<tr><td> Manager Phone Number </td><td>" + FEMPOld.Manager_Phone_Number + "</td><td>" + sa.FE.Manager_Phone_Number + "</td></tr>";
                }

                if (FEMPOld.Manager_Email != sa.FE.Manager_Email)
                {
                    change = change + "<tr><td> Manager Email </td><td>" + FEMPOld.Manager_Email + "</td><td>" + sa.FE.Manager_Email + "</td></tr>";
                }

                //------------------------------------------------------------------- Email Log Peronal Ends

                if (pht != null)
                {
                    change = change + "<tr><td> Profile Photo </td><td> New Entry </td><td> Profile Photo Uploaded</td></tr>";
                    pic = System.IO.Path.GetFileName(pht.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Photo"), fileName);
                    pht.SaveAs(pathtest);

                    sa.FE.Photo = url + "Photo/" + fileName;
                }

                if (Contract != null)
                {

                    cont = System.IO.Path.GetFileName(Contract.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + cont;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Contract"), fileName);
                    Contract.SaveAs(pathtest);

                    sa.FE.Signature = url + "Contract/" + fileName;
                }

                if (sa.FE.NDA_Acceptance_Date == null)
                {
                    sa.FE.NDA_Accept = 1;
                    sa.FE.Status = 1;
                    sa.FE.NDA_Acceptance_Date = DateTime.Now;
                }
                sa.FE.ModifiedBy = User.Identity.GetUserId();
                sa.FE.ModifiedOn = DateTime.Now;
                db.Entry(sa.FE).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                if (sa.FE.Other_detail != null)
                {
                    FE_Master_Other_Detail FEMOD = new FE_Master_Other_Detail();
                    change = change + "<tr><td> Other Detail </td><td> New Entry </td><td>" + sa.FE.Other_detail + "</td></tr>";
                    FEMOD.FE_ID = sa.FE.Id;
                    FEMOD.Other_Details = sa.FE.Other_detail;
                    FEMOD.CreatedBy = sa.FE.CreatedBy;
                    FEMOD.CreatedOn = sa.FE.CreatedOn;
                    db.FE_Master_Other_Detail.Add(FEMOD);
                    db.SaveChanges();
                }



                if (addChar == "Yes")
                {
                    sa.FEC.CreatedBy = User.Identity.GetUserId();
                    sa.FEC.CreatedOn = DateTime.Now;
                    db.FE_Master_Charges.Add(sa.FEC);
                    db.SaveChanges();
                }
                else
                {
                    //------------------------------------------------------------------- Email Log Charges Starts

                    FE_Master_Charges_Old FECO = new FE_Master_Charges_Old();
                    FECO = (from s in db.FE_Master_Charges
                            where s.FE_ID == sa.FE.Id
                            select new FE_Master_Charges_Old
                            {
                                Currency = s.Currency,
                                Charges_Business_Hour = s.Charges_Business_Hour,
                                Charges_Non_Business_Hour = s.Charges_Non_Business_Hour,
                                Charge_Day = s.Charge_Day,
                                Charge_Job = s.Charge_Job,
                                Charge_Month = s.Charge_Month,
                                Minimum_Hrs = s.Minimum_Hrs,
                                Other_detail = s.Other_detail,
                                //Travel_Charge = s.Travel_Charge
                            }).FirstOrDefault();

                    if (FECO.Currency != sa.FEC.Currency)
                    {
                        change = change + "<tr><td> Currency </td> <td>" + FECO.Currency + "</td><td>" + sa.FEC.Currency + "</td></tr>";
                    }

                    if (FECO.Charges_Business_Hour != sa.FEC.Charges_Business_Hour)
                    {
                        change = change + "<tr><td> Charges Per Business Hour </td> <td>" + FECO.Charges_Business_Hour + "</td><td>" + sa.FEC.Charges_Business_Hour + "</td></tr>";
                    }

                    if (FECO.Charges_Non_Business_Hour != sa.FEC.Charges_Non_Business_Hour)
                    {
                        change = change + "<tr><td> Charges Per Non Business Hour </td> <td>" + FECO.Charges_Non_Business_Hour + "</td><td>" + sa.FEC.Charges_Non_Business_Hour + "</td></tr>";
                    }

                    if (FECO.Charge_Job != sa.FEC.Charge_Job)
                    {
                        change = change + "<tr><td> Charges Per job </td> <td>" + FECO.Charge_Job + "</td><td>" + sa.FEC.Charge_Job + "</td></tr>";
                    }

                    if (FECO.Charge_Month != sa.FEC.Charge_Month)
                    {
                        change = change + "<tr><td> Charges Per Month </td> <td>" + FECO.Charge_Month + "</td><td>" + sa.FEC.Charge_Month + "</td></tr>";
                    }

                    if (FECO.Minimum_Hrs != sa.FEC.Minimum_Hrs)
                    {
                        change = change + "<tr><td> Minimum Hrs </td> <td>" + FECO.Minimum_Hrs + "</td><td>" + sa.FEC.Minimum_Hrs + "</td></tr>";
                    }

                    //if (FECO.Travel_Charge != sa.FEC.Travel_Charge)
                    //{
                    //    change = change + "<tr><td> Travel Charge </td> <td>" + FECO.Travel_Charge + "</td><td>" + sa.FEC.Travel_Charge + "</td></tr>";
                    //}

                    if (FECO.Other_detail != sa.FEC.Other_detail)
                    {
                        change = change + "<tr><td> Charge Other Detail </td> <td>" + FECO.Other_detail + "</td><td>" + sa.FEC.Other_detail + "</td></tr>";
                    }

                    //------------------------------------------------------------------- Email Log Charges Ends

                    sa.FEC.ModifiedBy = User.Identity.GetUserId();
                    sa.FEC.ModifiedOn = DateTime.Now;
                    db.Entry(sa.FEC).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }




                if (addSkill == "Yes")
                {
                    if (BD != null)
                    {
                        change = change + "<tr><td> Resume </td><td> New Entry </td><td> Resume Uploaded </td></tr>";
                        bd = System.IO.Path.GetFileName(BD.FileName);
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + bd;
                        string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Resume"), fileName);
                        BD.SaveAs(pathtest);

                        sa.FES.Bio_Data = url + "Resume/" + fileName;
                    }

                    sa.FES.CreatedBy = User.Identity.GetUserId();
                    sa.FES.CreatedOn = DateTime.Now;
                    db.FE_Master_Skill.Add(sa.FES);
                    db.SaveChanges();
                }
                else
                {
                    if (BD != null)
                    {
                        change = change + "<tr><td> Resume </td><td> New Entry </td><td> Resume Uploaded </td></tr>";
                        bd = System.IO.Path.GetFileName(BD.FileName);
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + bd;
                        string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Resume"), fileName);
                        BD.SaveAs(pathtest);

                        sa.FES.Bio_Data = url + "Resume/" + fileName;
                    }

                    sa.FES.ModifiedBy = User.Identity.GetUserId();
                    sa.FES.ModifiedOn = DateTime.Now;
                    db.Entry(sa.FES).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                if (IV[0].Exp != null)
                {
                    FE_Master_Skill_Data FeID = new FE_Master_Skill_Data();

                    //Loop and insert records.
                    foreach (InsertSkills iv in IV)
                    {
                        string skill = (from s in db.HeaderDescription where s.id.ToString() == iv.Skill select s.header_description).FirstOrDefault();
                        change = change + "<tr><td> Skill </td><td> New Entry </td><td>" + skill + "</td></tr>";
                        FeID.FE_ID = sa.FE.Id;
                        FeID.Skill = iv.Skill;
                        FeID.Exp = iv.Exp;
                        FeID.CreatedBy = User.Identity.GetUserId();
                        FeID.CreatedOn = DateTime.Now;
                        db.FE_Master_Skill_Data.Add(FeID);
                        db.SaveChanges();
                    }
                }



                if (IVI[0].ID_Type > 0)
                {
                    FE_Master_Identification FeID = new FE_Master_Identification();

                    //Loop and insert records.
                    foreach (IdentificationInsertView iv in IVI)
                    {
                        string iden = (from s in db.HeaderDescription where s.id == iv.ID_Type select s.header_description).FirstOrDefault();
                        change = change + "<tr><td> Identification </td><td> New Entry </td><td>" + iden + "</td></tr>";

                        FeID.FE_ID = sa.FE.Id;
                        FeID.ID_Number = iv.ID_Number;
                        FeID.ID_Type = iv.ID_Type;
                        string pici = "IDType_";
                        if (iv.ID_Upload != null)
                        {

                            pici = System.IO.Path.GetFileName(iv.ID_Upload.FileName);
                            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pici;
                            string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/ID"), fileName);
                            iv.ID_Upload.SaveAs(pathtest);

                            FeID.ID_Upload = url + "ID/" + fileName;
                        }
                        //FeID.ID_Upload = utlity.ConvertToImage(iv.ID_Upload);
                        FeID.CreatedBy = User.Identity.GetUserId();
                        FeID.CreatedOn = DateTime.Now;
                        db.FE_Master_Identification.Add(FeID);
                        db.SaveChanges();
                    }
                }

                if (IVC[0].Certification_Name > 0)
                {
                    FE_Master_Certification FeID = new FE_Master_Certification();

                    //Loop and insert records.
                    foreach (InsertCertificate iv in IVC)
                    {
                        string cert = (from s in db.Certification_Master where s.Id == iv.Certification_Name select s.Certification_Name).FirstOrDefault();
                        change = change + "<tr><td> Certification </td><td> New Entry </td><td>" + cert + "</td></tr>";
                        FeID.FE_ID = sa.FE.Id;
                        FeID.Certification_Name = iv.Certification_Name;

                        string picc = "IDType_";
                        if (iv.Certification_Upload != null)
                        {

                            picc = System.IO.Path.GetFileName(iv.Certification_Upload.FileName);
                            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + picc;
                            string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Certificate"), fileName);
                            iv.Certification_Upload.SaveAs(pathtest);

                            FeID.Certification_Upload = url + "Certificate/" + fileName;
                        }
                        //FeID.ID_Upload = utlity.ConvertToImage(iv.ID_Upload);
                        FeID.CreatedBy = User.Identity.GetUserId();
                        FeID.CreatedOn = DateTime.Now;
                        db.FE_Master_Certification.Add(FeID);
                        db.SaveChanges();
                    }
                }

                if (sa.FES.Others != null)
                {
                    FE_Master_Certification_Extra_Detail FEMCD = new FE_Master_Certification_Extra_Detail();
                    change = change + "<tr><td> Extra Certification Detail </td><td> New Entry </td><td>" + sa.FES.Others + "</td></tr>";
                    FEMCD.FE_ID = sa.FES.Id;
                    FEMCD.Other_Details = sa.FES.Others;
                    FEMCD.CreatedBy = sa.FES.CreatedBy;
                    FEMCD.CreatedOn = sa.FES.CreatedOn;
                    db.FE_Master_Certification_Extra_Detail.Add(FEMCD);
                    db.SaveChanges();
                }


                if (addFin == "Yes")
                {
                    //TempData["message"] = "Thank you for completing the registeration.Welcome aboard.Copy of signed NDA sent to your registered email id.";
                    TempData["message"] = "Congratulations and Thank you for completing the registration & activating the account. Welcome Aboard,Partner ! ";
                    string fe_type;
                    if (sa.FE.FE_Type == 345)
                    {
                        fe_type = "<b class='font-montserrat color-dark'>Company</b>.<br/> Company Name : <b class='font-montserrat color-dark'>" + sa.FE.Company_Name + "</b>";
                    }
                    else
                    {
                        fe_type = "<b class='font-montserrat color-dark'>Freelancer</b>";
                    }
                    string body = utlity.WelcomeAbord(sa.FE.First_Name + " " + sa.FE.Last_Name, fe_type, sa.FE.City, sa.FE.Country);
                    string filename = "Inwinteck_NDA_" + sa.FE.Id + ".pdf";
                    utlity.NDAPDF(sa.FE.Email, "Welcome Aboard", body, "Welcome Aboard<support@inwinteck.com>", sa.FE.First_Name + " " + sa.FE.Last_Name, sa.FE.NDA_Acceptance_Date.Value.ToString("dd-MM-yyyy HH:mm:ss"), filename);


                }
                else
                {
                    if (change != changeem)
                    {
                        change = change + "</table>";
                        string body = utlity.Changelog(sa.FE.First_Name + " " + sa.FE.Last_Name, change);

                        utlity.sendmailAcc(sa.FE.Email, "Data Updated", body, "Data Updated<support@inwinteck.com>");
                    }

                    TempData["message"] = "Details Updated !!";
                }

            }
            catch (Exception ex)
            {
                TempData["message"] = "Details Not Updated !! ";
            }
            return RedirectToAction("FEProfile", "Master", new { id = UrlEncryptionHelper.Encrypt(sa.FE.Id) });
        }

        public ActionResult DeleteFEProfile(int FeId)
        {
            try
            {

                if (FeId > 0)
                {
                    int nid = FeId;

                    FE_Master_Personal FI = new FE_Master_Personal();
                    FI = (from c in db.FE_Master_Personal where c.Id == FeId select c).FirstOrDefault();

                    ApplicationUser au = new ApplicationUser();

                    au = (from s in db.Users where s.UserName == FI.Email select s).FirstOrDefault();

                    if (FI != null)
                    {
                        db.Entry(FI).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();

                        db.Entry(au).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();

                        TempData["message"] = "Data Deleted !! ";
                    }
                }
                else
                {
                    TempData["message"] = "Empty Data !! ";
                }
            }
            catch
            {
                TempData["message"] = "Error while deleting !! ";
            }


            return RedirectToAction("FEMaster", "Master");
        }
        public ActionResult InsertIdentificationFEProfile(List<IdentificationInsertView> IV)
        {

            if (IV != null)
            {
                int nid = 0;

                FE_Master_Identification FeID = new FE_Master_Identification();

                //Loop and insert records.
                foreach (IdentificationInsertView iv in IV)
                {
                    FeID.FE_ID = iv.FE_ID;
                    nid = iv.FE_ID;
                    FeID.ID_Number = iv.ID_Number;
                    FeID.ID_Type = iv.ID_Type;
                    string pic = "IDType_";
                    if (iv.ID_Upload != null)
                    {

                        pic = System.IO.Path.GetFileName(iv.ID_Upload.FileName);
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                        string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/ID"), fileName);
                        iv.ID_Upload.SaveAs(pathtest);

                        FeID.ID_Upload = url + "ID/" + fileName;
                    }
                    //FeID.ID_Upload = utlity.ConvertToImage(iv.ID_Upload);
                    FeID.CreatedBy = User.Identity.GetUserId();
                    FeID.CreatedOn = DateTime.Now;
                    db.FE_Master_Identification.Add(FeID);
                    db.SaveChanges();
                }
                TempData["message"] = "Data Updated !! ";
                return RedirectToAction("FEProfile", "Master", new { id = nid });
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEProfile", "Master");
            }
        }
        public ActionResult DeleteIdentificationFEProfile(int FeId, int IdType)
        {
            string change = " <table style='width: 100 %; height: 40px;padding-left:20px; border:1px;'><tr style='background: rgb(131,194, 51);'><td>Fields</td><td>Old Data<td>New Data</td></tr>";


            if (FeId > 0 && IdType > 0)
            {
                int nid = FeId;

                FE_Master_Identification FI = new FE_Master_Identification();
                FE_Master_Personal sa = new FE_Master_Personal();
                sa = (from s in db.FE_Master_Personal where s.Id == FeId select s).FirstOrDefault();
                FI = (from c in db.FE_Master_Identification where c.FE_ID == FeId && c.ID_Type == IdType select c).FirstOrDefault();
                string IDtpe = (from s in db.HeaderDescription where s.id == FI.ID_Type select s.header_description).FirstOrDefault();
                if (FI != null)
                {
                    db.Entry(FI).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";

                    change = change + "<tr><td> ID Type </td><td>" + IDtpe + "</td><td> Data Deleted</td></tr></table>";
                    string body = utlity.Changelog(sa.First_Name + " " + sa.Last_Name, change);

                    utlity.sendmailAcc(sa.Email, "Data Deleted", body, "Data Updated<support@inwinteck.com>");
                    return RedirectToAction("FEProfile", "Master", new { id = FeId });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEProfile", "Master", new { id = FeId });
            }

            return View();
        }

        public ActionResult FEChargesProfile(int id)
        {
            // To check user access right
            string uri1 = "/Master/FEProfile";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            ManageFeCharges MC = new ManageFeCharges();

            // access right ends
            //FE_Master_Charges sa = new FE_Master_Charges();

            int cnt = (from s in db.FE_Master_Charges where s.FE_ID == id select s).Count();
            ViewBag.Currency = (from s in db.Currency_Master where s.Country == (from a in db.FE_Master_Personal where a.Id == id select a.Country).FirstOrDefault() select s.Currency).FirstOrDefault();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();
            ViewBag.FE = id;
            ViewBag.Contract = 1;
            if (cnt != 0)
            {
                MC.FE = (from s in db.FE_Master_Charges where s.FE_ID == id select s).FirstOrDefault();
                MC.IC = (from c in db.FE_Master_serviceArea where c.FE_ID == id select new InsertServiceArea { FE_ID = c.FE_ID, Country = c.Country, ZipCode_pincode = c.ZipCode_pincode }).ToList();
                ViewBag.Edit = "Yes";
                return View(MC);
            }
            else
            {
                MC.FE = (from s in db.FE_Master_Charges where s.FE_ID == id select s).FirstOrDefault();
                MC.IC = (from c in db.FE_Master_serviceArea where c.FE_ID == id select new InsertServiceArea { FE_ID = c.FE_ID, Country = c.Country, ZipCode_pincode = c.ZipCode_pincode }).ToList();
                return View(MC);
            }

        }
        public ActionResult InsertFeChargesProfile(List<InsertServiceArea> IV)
        {
            int nid = 0;

            if (IV != null)
            {


                FE_Master_serviceArea FeID = new FE_Master_serviceArea();

                //Loop and insert records.
                foreach (InsertServiceArea iv in IV)
                {
                    FeID.FE_ID = iv.FE_ID;
                    nid = iv.FE_ID;
                    FeID.Country = iv.Country;
                    FeID.ZipCode_pincode = iv.ZipCode_pincode;

                    //FeID.ID_Upload = utlity.ConvertToImage(iv.ID_Upload);
                    FeID.CreatedBy = User.Identity.GetUserId();
                    FeID.CreatedOn = DateTime.Now;
                    db.FE_Master_serviceArea.Add(FeID);
                    db.SaveChanges();
                }
                TempData["message"] = "Data Updated !! ";
                return RedirectToAction("FEProfile", "Master", new { id = nid });
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEProfile", "Master", new { id = nid });
            }
        }
        public ActionResult DeleteFeChargesProfile(int FeId, string Country)
        {

            if (FeId > 0 && Country != "")
            {
                int nid = FeId;

                FE_Master_serviceArea FI = new FE_Master_serviceArea();
                FI = (from c in db.FE_Master_serviceArea where c.FE_ID == FeId && c.Country == Country select c).FirstOrDefault();
                if (FI != null)
                {
                    db.Entry(FI).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    return RedirectToAction("FEProfile", "Master", new { id = FeId });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEProfile", "Master", new { id = FeId });
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addFEChargesProfile(FE_Master_Charges sa)
        {
            try
            {

                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.FE_Master_Charges.Add(sa);
                db.SaveChanges();

                FE_Master_Personal FMP = new FE_Master_Personal();

                FMP = (from s in db.FE_Master_Personal where s.Id == sa.FE_ID select s).FirstOrDefault();

                FMP.Status = 0;
                FMP.ModifiedBy = User.Identity.GetUserId();
                FMP.ModifiedOn = DateTime.Now;
                db.Entry(FMP).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                TempData["link"] = "Yes";
                TempData["message"] = "FE Charges Added !!";

            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Charges Not Created !! ";
            }
            return RedirectToAction("FEChargesProfile", "Master", new { id = sa.FE_ID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditFEChargesProfile(FE_Master_Charges sa)
        {
            try
            {

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                FE_Master_Personal FMP = new FE_Master_Personal();

                FMP = (from s in db.FE_Master_Personal where s.Id == sa.FE_ID select s).FirstOrDefault();

                FMP.Status = 0;
                FMP.ModifiedBy = User.Identity.GetUserId();
                FMP.ModifiedOn = DateTime.Now;
                db.Entry(FMP).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                TempData["link"] = "Yes";
                TempData["message"] = "FE Charges Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Charges Not Updated !! ";
            }
            return RedirectToAction("FEChargesProfile", "Master", new { id = sa.FE_ID });
        }
        public ActionResult FEFinancialProfile(int id)
        {
            // To check user access right
            string uri1 = "/Master/FEProfile";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends
            FE_Master_Financial sa = new FE_Master_Financial();

            int cnt = (from s in db.FE_Master_Financial where s.FE_ID == id select s).Count();
            ViewBag.Contract = 1;
            if (cnt != 0)
            {
                sa = (from s in db.FE_Master_Financial where s.FE_ID == id select s).FirstOrDefault();

                ViewBag.Edit = "Yes";
                return View(sa);
            }
            else
            {
                ViewBag.FE = id;
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addFEFinancialProfile(FE_Master_Financial sa)
        {
            try
            {

                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.FE_Master_Financial.Add(sa);
                db.SaveChanges();

                FE_Master_Personal FMP = new FE_Master_Personal();

                FMP = (from s in db.FE_Master_Personal where s.Id == sa.FE_ID select s).FirstOrDefault();

                FMP.Status = 0;
                FMP.ModifiedBy = User.Identity.GetUserId();
                FMP.ModifiedOn = DateTime.Now;
                db.Entry(FMP).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "FE Financia Added !!";

            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Financia Not Created !! ";
            }
            return RedirectToAction("FEFinancialProfile", "Master", new { id = sa.FE_ID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditFEFinancialProfile(FE_Master_Financial sa)
        {
            try
            {

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                FE_Master_Personal FMP = new FE_Master_Personal();

                FMP = (from s in db.FE_Master_Personal where s.Id == sa.FE_ID select s).FirstOrDefault();

                FMP.Status = 0;
                FMP.ModifiedBy = User.Identity.GetUserId();
                FMP.ModifiedOn = DateTime.Now;
                db.Entry(FMP).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "FE Financia Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Financia Not Updated !! ";
            }
            return RedirectToAction("FEFinancialProfile", "Master", new { id = sa.FE_ID });
        }

        public ActionResult FESkillProfile(int? id)
        {
            // To check user access right
            string uri1 = "/Master/FEProfile";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();
            ManageSkill MS = new ManageSkill();
            MS.IV = new List<Models.InsertCertificate>();
            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends
            //FE_Master_Skill sa = new FE_Master_Skill();

            int cnt = (from s in db.FE_Master_Skill where s.FE_ID == id select s).Count();
            ViewBag.FE = id;
            ViewBag.Contract = 1;
            if (cnt != 0)
            {
                MS.IC = (from c in db.FE_Master_Certification join mc in db.Certification_Master on c.Certification_Name equals mc.Id where c.FE_ID == id select new InsertCertificateView { Id = c.Certification_Name, FE_ID = c.FE_ID, Certification_Upload = c.Certification_Upload, Certification_Name = mc.Certification_Name }).ToList();
                if (id.HasValue)
                {
                    MS.Id = id.Value;
                }

                MS.FE = (from s in db.FE_Master_Skill where s.FE_ID == id select s).FirstOrDefault();

                ViewBag.Edit = "Yes";
                ViewBag.Certification = (from c in db.Certification_Master where c.Status == 1 select new SelectListItem { Text = c.Certification_Name, Value = SqlFunctions.StringConvert((double)c.Id).Trim() }).ToList();
                ViewBag.Exp = (from c in db.HeaderDescription where c.header_id == 12 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
                return View(MS);
            }
            else
            {
                if (id.HasValue)
                {
                    MS.Id = id.Value;
                }
                MS.IC = (from c in db.FE_Master_Certification join mc in db.Certification_Master on c.Certification_Name equals mc.Id where c.FE_ID == id select new InsertCertificateView { Id = c.Certification_Name, FE_ID = c.FE_ID, Certification_Upload = c.Certification_Upload, Certification_Name = mc.Certification_Name }).ToList();

                ViewBag.Certification = (from c in db.Certification_Master where c.Status == 1 select new SelectListItem { Text = c.Certification_Name, Value = SqlFunctions.StringConvert((double)c.Id).Trim() }).ToList();
                ViewBag.Exp = (from c in db.HeaderDescription where c.header_id == 12 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
                return View(MS);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addFESkillProfile(FE_Master_Skill sa, HttpPostedFileBase pht)
        {
            try
            {
                string pic = "";


                if (pht != null)
                {

                    pic = System.IO.Path.GetFileName(pht.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Resume"), fileName);
                    pht.SaveAs(pathtest);

                    sa.Bio_Data = url + "Resume/" + fileName;
                }

                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.FE_Master_Skill.Add(sa);
                db.SaveChanges();

                FE_Master_Personal FMP = new FE_Master_Personal();

                FMP = (from s in db.FE_Master_Personal where s.Id == sa.FE_ID select s).FirstOrDefault();

                FMP.Status = 0;
                FMP.ModifiedBy = User.Identity.GetUserId();
                FMP.ModifiedOn = DateTime.Now;
                db.Entry(FMP).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "FE Skill Added !!";

            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Skill Not Created !! ";
            }
            return RedirectToAction("FESkillProfile", "Master", new { id = sa.FE_ID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditFESkillProfile(FE_Master_Skill sa, HttpPostedFileBase pht)
        {
            try
            {
                string pic = "";
                string ct = "";
                if (pht != null)
                {

                    pic = System.IO.Path.GetFileName(pht.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Resume"), fileName);
                    pht.SaveAs(pathtest);

                    sa.Bio_Data = url + "Resume/" + fileName;
                }


                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                FE_Master_Personal FMP = new FE_Master_Personal();

                FMP = (from s in db.FE_Master_Personal where s.Id == sa.FE_ID select s).FirstOrDefault();

                FMP.Status = 0;
                FMP.ModifiedBy = User.Identity.GetUserId();
                FMP.ModifiedOn = DateTime.Now;
                db.Entry(FMP).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "FE Skill Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "FE Skill Not Updated !! ";
            }
            return RedirectToAction("FESkillProfile", "Master", new { id = sa.FE_ID });
        }

        public ActionResult InsertCertificateProfile(List<InsertCertificate> IV)
        {


            int nid = 0;

            FE_Master_Certification FeID = new FE_Master_Certification();

            //Loop and insert records.
            foreach (InsertCertificate iv in IV)
            {
                FeID.FE_ID = iv.FE_ID;
                nid = iv.FE_ID;
                FeID.Certification_Name = iv.Certification_Name;

                string pic = "IDType_";
                if (iv.Certification_Upload != null)
                {

                    pic = System.IO.Path.GetFileName(iv.Certification_Upload.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/Certificate"), fileName);
                    iv.Certification_Upload.SaveAs(pathtest);

                    FeID.Certification_Upload = url + "Certificate/" + fileName;
                }
                //FeID.ID_Upload = utlity.ConvertToImage(iv.ID_Upload);
                FeID.CreatedBy = User.Identity.GetUserId();
                FeID.CreatedOn = DateTime.Now;
                db.FE_Master_Certification.Add(FeID);
                db.SaveChanges();
            }
            TempData["message"] = "Data Updated !! ";
            return RedirectToAction("FEProfile", "Master", new { id = nid });

        }
        public ActionResult DeleteCertificateProfile(int FeId, int IdType)
        {
            string change = " <table style='width: 100 %; height: 40px;padding-left:20px; border:1px;'><tr style='background: rgb(131,194, 51);'><td>Fields</td><td>Old Data<td>New Data</td></tr>";

            if (FeId > 0 && IdType > 0)
            {
                int nid = FeId;
                FE_Master_Personal sa = new FE_Master_Personal();
                sa = (from s in db.FE_Master_Personal where s.Id == FeId select s).FirstOrDefault();

                FE_Master_Certification FI = new FE_Master_Certification();
                FI = (from c in db.FE_Master_Certification where c.FE_ID == FeId && c.Certification_Name == IdType select c).FirstOrDefault();
                string certiname = (from s in db.Certification_Master where s.Id == FI.Certification_Name select s.Certification_Name).FirstOrDefault();
                if (FI != null)
                {
                    db.Entry(FI).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    change = change + "<tr><td> Certification Name </td><td>" + certiname + "</td><td> Data Deleted</td></tr></table>";
                    string body = utlity.Changelog(sa.First_Name + " " + sa.Last_Name, change);

                    utlity.sendmailAcc(sa.Email, "Data Deleted", body, "Data Updated<support@inwinteck.com>");
                    return RedirectToAction("FEProfile", "Master", new { id = FeId });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEProfile", "Master", new { id = FeId });
            }

            return View();
        }

        public ActionResult InsertSkillsProfile(List<InsertSkills> IV)
        {


            int nid = 0;

            FE_Master_Skill_Data FeID = new FE_Master_Skill_Data();

            //Loop and insert records.
            foreach (InsertSkills iv in IV)
            {
                FeID.FE_ID = iv.FE_ID;
                nid = iv.FE_ID;
                FeID.Skill = iv.Skill;
                FeID.Exp = iv.Exp;
                FeID.CreatedBy = User.Identity.GetUserId();
                FeID.CreatedOn = DateTime.Now;
                db.FE_Master_Skill_Data.Add(FeID);
                db.SaveChanges();
            }
            TempData["message"] = "Data Updated !! ";
            return RedirectToAction("FEProfile", "Master", new { id = nid });

        }
        public ActionResult DeleteSkillsProfile(int FeId, int IdType)
        {

            if (FeId > 0 && IdType > 0)
            {
                int nid = FeId;

                FE_Master_Skill_Data FI = new FE_Master_Skill_Data();
                FI = (from c in db.FE_Master_Skill_Data where c.Id == IdType select c).FirstOrDefault();
                if (FI != null)
                {
                    db.Entry(FI).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    return RedirectToAction("FEProfile", "Master", new { id = FeId });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("FEProfile", "Master", new { id = FeId });
            }

            return View();
        }

        public ActionResult EnquiryCustomer()
        {
            List<EnqDetailsEU> li = new List<EnqDetailsEU>();
            li = db.Database.SqlQuery<EnqDetailsEU>("getEnqEUDetails").ToList();
            return View(li);
        }

        public ActionResult CreateEnquiryCustomer()
        {
            ViewBag.EU = (from c in db.EU_Master_Sales where c.Status == 1 select new SelectListItem { Text = c.Customer_Name, Value = c.Id.ToString() }).ToList();
            ViewBag.Project_Type = (from c in db.HeaderDescription where c.header_id == 11 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateEnquiryCustomer(Enq_EU sa, string Remark, HttpPostedFileBase ath)
        {
            try
            {
                string pic = "";
                if (ath != null)
                {

                    pic = System.IO.Path.GetFileName(ath.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/CEnq"), fileName);
                    ath.SaveAs(pathtest);

                    sa.Attach = url + "CEnq/" + fileName;
                }

                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.Enq_EU.Add(sa);
                int res = db.SaveChanges();
                if (res > 0)
                {
                    Enq_EU_History th = new Enq_EU_History();
                    th.Enq_Id = sa.Id;
                    th.Comments = Remark;
                    th.Status = sa.Status;
                    th.CreatedBy = User.Identity.GetUserId();
                    th.CreatedOn = DateTime.Now;
                    db.Enq_EU_History.Add(th);
                    db.SaveChanges();

                }

                //string eu = (from s in db.EU_Master where s.Id == sa.EU_ID select s.Customer_Name).FirstOrDefault();
                //string body = utlity.TicketGenerated(eu, sa.Ticket_No, sa.Case_No, sa.Site_Name, sa.Street_Address, sa.Dispatch_Date.ToString());
                //utlity.sendmail(email, Email_Subject, body, mailer);
                TempData["message"] = "Enquiry Created : Enquiry No :" + sa.Id;

            }
            catch (Exception ex)
            {
                TempData["message"] = "Enquiry Not Created !! ";
            }
            return RedirectToAction("editEnquiryCustomer", "Master", new { id = sa.Id });
        }

        public ActionResult editEnquiryCustomer(int id)
        {
            Enq_EU ti = new Enq_EU();
            ti = (from s in db.Enq_EU where s.Id == id select s).FirstOrDefault();

            ViewBag.EU = (from c in db.EU_Master_Sales where c.Status == 1 && c.Id == ti.EU_ID select c.Customer_Name).FirstOrDefault();
            ViewBag.EU_Office = (from c in db.EU_Master_Branch_Sales where c.Id == ti.EU_Office select c.Office).FirstOrDefault();
            ViewBag.Project_Type = (from c in db.HeaderDescription where c.header_id == 11 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            return View(ti);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult editEnquiryCustomer(Enq_EU sa, string Remark, HttpPostedFileBase ath)
        {
            try
            {
                string pic = "";
                if (ath != null)
                {

                    pic = System.IO.Path.GetFileName(ath.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    string pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/CEnq"), fileName);
                    ath.SaveAs(pathtest);

                    sa.Attach = url + "CEnq/" + fileName;
                }

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                int res = db.SaveChanges();
                if (res > 0)
                {
                    Enq_EU_History th = new Enq_EU_History();
                    th.Enq_Id = sa.Id;
                    th.Comments = Remark;
                    th.Status = sa.Status;
                    th.CreatedBy = User.Identity.GetUserId();
                    th.CreatedOn = DateTime.Now;
                    db.Enq_EU_History.Add(th);
                    db.SaveChanges();

                }



                //string eu = (from s in db.EU_Master where s.Id == sa.EU_ID select s.Customer_Name).FirstOrDefault();
                //string body = utlity.TicketGenerated(eu, sa.Ticket_No, sa.Case_No, sa.Site_Name, sa.Street_Address, sa.Dispatch_Date.ToString());
                //utlity.sendmail(email, Email_Subject, body, mailer);
                TempData["message"] = "Enquiry Updated ";

            }
            catch (Exception ex)
            {
                TempData["message"] = "Enquiry Not Created !! ";
            }
            return RedirectToAction("editEnquiryCustomer", "Master", new { id = sa.Id });
        }

        public ActionResult EUMasterSales(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
            // To check user access right
            string uri1 = "/Master/EUMasterSales";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            ViewBag.Permission = cm;
            // access right ends 
            int pageSize = 10;
            int pgc = 0;
            int pageCount = 0;
            List<EU_Master_Sales> li = new List<EU_Master_Sales>();
            if (searchtype != "" && searchtext != "")
            {
                if (searchtype == "Name")
                {
                    pgc = db.EU_Master_Sales.Where(x => x.Customer_Name.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.EU_Master_Sales
                          orderby c.Customer_Name
                          select c).Where(x => x.Customer_Name.StartsWith(searchtext)).ToList();
                }
                else if (searchtype == "Email")
                {
                    pgc = db.EU_Master_Sales.Where(x => x.Customer_Email.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.EU_Master_Sales
                          orderby c.Customer_Email
                          select c).Where(x => x.Customer_Email.StartsWith(searchtext)).ToList();
                }
                ViewBag.Search = searchtext;

            }
            else
            {
                pgc = db.EU_Master_Sales.Count();
                pageCount = pgc / pageSize;

                li = (from s in db.EU_Master_Sales orderby s.Customer_Name select s).Skip(pageNo * pageSize).Take(pageSize).ToList();
                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }

            return View(li);
        }
        public ActionResult addEUMasterSales()
        {
            // To check user access right
            string uri1 = "/Master/EUMasterSales";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 

            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addEUMasterSales(EU_Master_Sales sa)
        {
            try
            {
                int cnt = (from s in db.EU_Master_Sales where s.Customer_Email == sa.Customer_Email select s).Count();
                if (cnt > 0)
                {
                    TempData["message"] = "EU already Register with us !!";
                    return RedirectToAction("addEUMasterSales", "Master");
                }

                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.EU_Master_Sales.Add(sa);
                db.SaveChanges();


                TempData["link"] = "Yes";
                TempData["message"] = "Client Sales Master Added !!";

            }
            catch (Exception ex)
            {
                TempData["message"] = "Client Sales Master Not Created !! ";
            }
            return RedirectToAction("EditEUMasterSales", "Master", new { id = sa.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult OfficeEUMasterSales(EU_Master_Branch_Sales sa, int OID)
        {
            if (OID != 0)
            {
                try
                {
                    sa.Id = OID;
                    sa.CreatedBy = (from s in db.EU_Master_Branch_Sales where s.Id == OID select s.CreatedBy).FirstOrDefault();
                    sa.CreatedOn = (from s in db.EU_Master_Branch_Sales where s.Id == OID select s.CreatedOn).FirstOrDefault();
                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    TempData["message"] = "Client Sale's Office Updated !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Client Sale's Office Not Updated !! ";
                }
            }
            else
            {
                try
                {
                    sa.CreatedBy = User.Identity.GetUserId();
                    sa.CreatedOn = DateTime.Now;
                    db.EU_Master_Branch_Sales.Add(sa);
                    db.SaveChanges();


                    TempData["message"] = "Client Sale's Office Added !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Client Sale's Office Not Created !! ";
                }
            }

            return RedirectToAction("EditEUMasterSales", "Master", new { id = sa.EU_ID });
        }

        public ActionResult deleteOfficeSales(int OD, int ED)
        {

            if (OD > 0)
            {

                EU_Master_Branch_Sales sa = new EU_Master_Branch_Sales();
                sa = (from c in db.EU_Master_Branch_Sales where c.Id == OD select c).FirstOrDefault();
                if (sa != null)
                {
                    db.Entry(sa).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    return RedirectToAction("EditEUMasterSales", "Master", new { id = ED });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("EditEUMasterSales", "Master", new { id = ED });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ContactEUMasterSales(EU_Master_Contacts_Sales sa, int Contact_ID, int Contact_EU_ID)
        {
            if (Contact_ID != 0)
            {
                try
                {
                    sa.Id = Contact_ID;
                    sa.CreatedBy = (from s in db.EU_Master_Contacts_Sales where s.Id == Contact_ID select s.CreatedBy).FirstOrDefault();
                    sa.CreatedOn = (from s in db.EU_Master_Contacts_Sales where s.Id == Contact_ID select s.CreatedOn).FirstOrDefault();
                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    TempData["message"] = "Client Sale's Contact Updated !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Client Sale's Contact Not Updated !! ";
                }
            }
            else
            {
                try
                {
                    sa.CreatedBy = User.Identity.GetUserId();
                    sa.CreatedOn = DateTime.Now;
                    db.EU_Master_Contacts_Sales.Add(sa);
                    db.SaveChanges();


                    TempData["message"] = "Client Sales's Contact Added !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Client Sales's Contact Not Created !! ";
                }
            }

            return RedirectToAction("EditEUMasterSales", "Master", new { id = Contact_EU_ID });
        }

        public ActionResult deleteContactSales(int OC)
        {
            int od = (from s in db.EU_Master_Contacts_Sales where s.Id == OC select s.Office_ID).FirstOrDefault();
            int ED = (from s in db.EU_Master_Branch_Sales where s.Id == od select s.EU_ID).FirstOrDefault();
            if (OC > 0)
            {

                EU_Master_Contacts_Sales sa = new EU_Master_Contacts_Sales();
                sa = (from c in db.EU_Master_Contacts_Sales where c.Id == OC select c).FirstOrDefault();
                if (sa != null)
                {
                    db.Entry(sa).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    return RedirectToAction("EditEUMasterSales", "Master", new { id = ED });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("EditEUMasterSales", "Master", new { id = ED });
            }

            return View();
        }
        public ActionResult EditEUMasterSales(int id)
        {
            // To check user access right
            string uri1 = "/Master/EUMasterSales";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends
            EU_Master_Sales sa = new EU_Master_Sales();
            sa = (from s in db.EU_Master_Sales where s.Id == id select s).FirstOrDefault();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();
            ViewBag.Office = (from s in db.EU_Master_Branch_Sales where s.EU_ID == sa.Id select s).ToList();
            ViewBag.Office_Name = (from c in db.EU_Master_Branch_Sales where c.EU_ID == sa.Id select new SelectListItem { Text = c.Office, Value = SqlFunctions.StringConvert((double)c.Id).TrimStart() }).ToList();

            return View(sa);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditEUMasterSales(EU_Master_Sales sa)
        {
            try
            {

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "Client Sales Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "Client Sales Not Updated !! ";
            }
            return RedirectToAction("EditEUMasterSales", "Master", new { id = sa.Id });
        }
        public ActionResult viewEUMasterSales(int id)
        {
            // To check user access right
            string uri1 = "/Master/EUMasterSales";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_view == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends
            EU_Master_Sales sa = new EU_Master_Sales();
            sa = (from s in db.EU_Master_Sales where s.Id == id select s).FirstOrDefault();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();

            return View(sa);
        }

        public ActionResult statusEUMasterSales(int id)
        {
            // To check user access right
            string uri1 = "/Master/EUMasterSales";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_update == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            EU_Master_Sales sa = new EU_Master_Sales();
            sa = (from s in db.EU_Master_Sales where s.Id == id select s).FirstOrDefault();
            if (sa.Status == 1)
            {
                sa.Status = 0;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            else
            {
                sa.Status = 1;
                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
            }
            db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EUMasterSales", "Master");
        }

       

        //public async Task<ActionResult> FEMasterExe(int pageNo = 0, string searchtype = "", string searchtext = "")
        //{
        //    // To check user access right
        //    string uri1 = "/Master/FEMasterExe";

        //    int bid = utlity.GetBussinessId(uri1);
        //    string ucheckid = User.Identity.GetUserId();
        //    ViewBag.ucheckid = ucheckid;

        //    checkpermission cm = new checkpermission();

        //    cm = await db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefaultAsync();
        //    if (cm == null)
        //    {
        //        return RedirectToAction("UnAuthorisedAccess", "Dashboard");
        //    }
        //    ViewBag.Permission = cm;
        //    // access right ends 

        //    int pageSize = 10;

        //    // Fetch data in parallel using separate DbContext instances
        //    var feMasterPersonalTask = GetFEMasterPersonalAsync();
        //    var activatedCountTask = GetActivatedCountAsync();
        //    var deactivatedCountTask = GetDeactivatedCountAsync();
        //    var blackListedCountTask = GetBlackListedCountAsync();
        //    var certiActCountTask = GetCertiActCountAsync();
        //    var notInterestedCountTask = GetNotInterestedCountAsync();
        //    var totalCountTask = GetTotalCountAsync();

        //    await Task.WhenAll(feMasterPersonalTask, activatedCountTask, deactivatedCountTask, blackListedCountTask, certiActCountTask, notInterestedCountTask, totalCountTask);

        //    // Retrieve the results from the tasks
        //    var feMasterPersonalList = await feMasterPersonalTask;
        //    var activatedCount = await activatedCountTask;
        //    var deactivatedCount = await deactivatedCountTask;
        //    var blackListedCount = await blackListedCountTask;
        //    var certiActCount = await certiActCountTask;
        //    var notInterestedCount = await notInterestedCountTask;
        //    var totalCount = await totalCountTask;

        //    // Calculate the total number of pages
        //    var pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

        //    ViewBag.pageNo = pageNo;
        //    ViewBag.pageCount = pageCount;
        //    ViewBag.Activated = activatedCount;
        //    ViewBag.DeActivated = deactivatedCount;
        //    ViewBag.BlackListed = blackListedCount;
        //    ViewBag.CertiAct = certiActCount;
        //    ViewBag.NotInterested = notInterestedCount;

        //    ViewBag.CertiDeAct = totalCount - certiActCount;
        //    return View(feMasterPersonalList);
        //}

        //private async Task<List<FE_Master_Personal_list>> GetFEMasterPersonalAsync()
        //{
        //    using (var context = new ApplicationDbContext())
        //    {
        //        return await (from s in context.FE_Master_Personal
        //                      orderby s.CreatedOn descending
        //                      select new FE_Master_Personal_list
        //                      {
        //                          Id = s.Id,
        //                          CreatedOn = s.CreatedOn,
        //                          NDA_Acceptance_Date = s.NDA_Acceptance_Date,
        //                          First_Name = s.First_Name,
        //                          Last_Name = s.Last_Name,
        //                          FE_Type = s.FE_Type,
        //                          Company_Name = s.Company_Name,
        //                          Email = s.Email,
        //                          Phone_Number_Code = s.Phone_Number_Code,
        //                          Phone_Number = s.Phone_Number,
        //                          Country = s.Country,
        //                          State = s.State,
        //                          City = s.City,
        //                          FeInterest = s.FeInterest,
        //                          ZipCode_Pincode = s.ZipCode_Pincode,
        //                          Status = s.Status,
        //                          Certification = (from a in context.FE_Master_Certification where a.FE_ID == s.Id select a).Count(),
        //                          Manager_Name = s.Manager_Name,
        //                          Manager_Phone_Number = s.Manager_Phone_Number,
        //                          CreatedBy = (from a in context.Users where a.Id == s.CreatedBy select a.Name).FirstOrDefault(),
        //                      }).ToListAsync();
        //    }
        //}

        //private async Task<int> GetTotalCountAsync()
        //{
        //    using (var context = new ApplicationDbContext())
        //    {
        //        return await context.FE_Master_Personal.CountAsync();
        //    }
        //}

        //private async Task<int> GetActivatedCountAsync()
        //{
        //    using (var context = new ApplicationDbContext())
        //    {
        //        return await (from s in context.FE_Master_Personal where s.Status == 1 select s).CountAsync();
        //    }
        //}

        //private async Task<int> GetDeactivatedCountAsync()
        //{
        //    using (var context = new ApplicationDbContext())
        //    {
        //        return await (from s in context.FE_Master_Personal where s.Status == 0 select s).CountAsync();
        //    }
        //}

        //private async Task<int> GetBlackListedCountAsync()
        //{
        //    using (var context = new ApplicationDbContext())
        //    {
        //        return await (from s in context.FE_Master_Personal where s.Status == 2 select s).CountAsync();
        //    }
        //}

        //private async Task<int> GetCertiActCountAsync()
        //{
        //    using (var context = new ApplicationDbContext())
        //    {
        //        return await (from s in context.FE_Master_Certification select s).GroupBy(x => x.FE_ID).CountAsync();
        //    }
        //}

        //private async Task<int> GetNotInterestedCountAsync()
        //{
        //    using (var context = new ApplicationDbContext())
        //    {
        //        return await (from s in context.FE_Master_Personal where s.FeInterest == 0 select s).CountAsync();
        //    }
        //}


        public ActionResult FEPersonalExe(int id)
        {
            int per = 25;
            ManageFePersonal MP = new ManageFePersonal();
            // To check user access right
            string uri1 = "/Master/FEMasterExe";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            if (cm.record_insert == false)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends
            ViewBag.Language = (from c in db.HeaderDescription where c.header_id == 1 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).OrderBy(x => x.Text).ToList();
            ViewBag.Identification = (from c in db.HeaderDescription where c.header_id == 2 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Chat_mode = (from c in db.HeaderDescription where c.header_id == 3 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Citizen = (from c in db.HeaderDescription where c.header_id == 8 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.FE_Type = (from c in db.HeaderDescription where c.header_id == 9 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.DialCode = (from c in db.Country_Dialing_Code where c.Status == 1 select new SelectListItem { Text = c.Code + " " + c.Country, Value = c.Code.Trim() }).ToList();
            ViewBag.Currency = (from s in db.Currency_Master
                                where
   s.Country == (from a in db.FE_Master_Personal where a.Id == id select a.Country).FirstOrDefault() && s.Currency != "USD"
                                select new SelectListItem { Text = s.Currency, Value = SqlFunctions.StringConvert((double)s.Id).Trim() }).ToList();
            ViewBag.Certification = (from c in db.Certification_Master where c.Status == 1 select new SelectListItem { Text = c.Certification_Name, Value = SqlFunctions.StringConvert((double)c.Id).Trim() }).ToList();
            ViewBag.Exp = (from c in db.HeaderDescription where c.header_id == 12 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Skill = (from c in db.HeaderDescription where c.header_id == 18 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();


            MP.IV = new List<IdentificationInsertView>();

            MP.FE = (from s in db.FE_Master_Personal where s.Id == id select s).FirstOrDefault();
            MP.IF = (from s in db.FE_Master_Identification
                     join hd in db.HeaderDescription on s.ID_Type equals hd.id
                     where s.FE_ID == id
                     select new IdentificationInsertFill
                     {
                         FE_ID = s.FE_ID,
                         ID_Number = s.ID_Number,
                         ID_Type = hd.header_description,
                         Upload = s.ID_Upload,
                         IdType = s.ID_Type
                     }).ToList();
            ViewBag.Edit = "Yes";

            if (MP.FE.NDA_Accept == 1)
            {
                per = 60;
            }

            if (MP.FE.latitude != null)
            {
                TempData["geo"] = "Yes";
            }
            else
            {
                TempData["geo"] = null;
            }
            //========================================== Charges & Services ===========================================================
            int cntc = (from s in db.FE_Master_Charges where s.FE_ID == id select s).Count();

            if (cntc != 0)
            {
                MP.FEC = (from s in db.FE_Master_Charges where s.FE_ID == id select s).FirstOrDefault();
                MP.ICS = (from c in db.FE_Master_serviceArea where c.FE_ID == id select new InsertServiceArea { FE_ID = c.FE_ID, Country = c.Country, ZipCode_pincode = c.ZipCode_pincode }).ToList();
                ViewBag.ChargEdit = "Yes";
                ViewBag.ChargesPer = 1;
                per = per + 15;
            }
            else
            {
                MP.FEC = (from s in db.FE_Master_Charges where s.FE_ID == id select s).FirstOrDefault();
                MP.ICS = (from c in db.FE_Master_serviceArea where c.FE_ID == id select new InsertServiceArea { FE_ID = c.FE_ID, Country = c.Country, ZipCode_pincode = c.ZipCode_pincode }).ToList();
                ViewBag.ChargesPer = 0;
                per = per + 0;
            }

            //============================================== Financial ===============================================================

            int cntf = (from s in db.FE_Master_Financial where s.FE_ID == id select s).Count();

            if (cntf != 0)
            {
                MP.FEF = (from s in db.FE_Master_Financial where s.FE_ID == id select s).FirstOrDefault();

                ViewBag.Editf = "Yes";
            }
            else
            {
            }

            //=============================================== Skill ==================================================================

            int cnts = (from s in db.FE_Master_Skill where s.FE_ID == id select s).Count();
            if (cnts != 0)
            {
                MP.ICCE = (from c in db.FE_Master_Certification join mc in db.Certification_Master on c.Certification_Name equals mc.Id where c.FE_ID == id select new InsertCertificateView { Id = c.Certification_Name, FE_ID = c.FE_ID, Certification_Upload = c.Certification_Upload, Certification_Name = mc.Certification_Name }).ToList();

                MP.FES = (from s in db.FE_Master_Skill where s.FE_ID == id select s).FirstOrDefault();
                MP.ISV = (from s in db.FE_Master_Skill_Data join a in db.HeaderDescription on s.Skill equals a.id.ToString() join b in db.HeaderDescription on s.Exp equals b.id.ToString() where s.FE_ID == id select new InsertSkillsView { Id = s.Id, FE_ID = s.FE_ID, Skill_Name = a.header_description, Exp_Upload = b.header_description }).ToList();
                ViewBag.EditS = "Yes";
                ViewBag.SkillPer = 1;
                per = per + 15;
            }
            else
            {
                per = per + 0;
                ViewBag.SkillPer = 0;
                MP.ICCE = (from c in db.FE_Master_Certification join mc in db.Certification_Master on c.Certification_Name equals mc.Id where c.FE_ID == id select new InsertCertificateView { Id = c.Certification_Name, FE_ID = c.FE_ID, Certification_Upload = c.Certification_Upload, Certification_Name = mc.Certification_Name }).ToList();
                MP.ISV = (from s in db.FE_Master_Skill_Data join a in db.HeaderDescription on s.Skill equals a.id.ToString() join b in db.HeaderDescription on s.Exp equals b.id.ToString() where s.FE_ID == id select new InsertSkillsView { Id = s.Id, FE_ID = s.FE_ID, Skill_Name = a.header_description, Exp_Upload = b.header_description }).ToList();
            }
            ViewBag.Percentage = per;
            ViewBag.ResumePer = (from s in db.FE_Master_Skill where s.FE_ID == id select s.Bio_Data).FirstOrDefault();
            //------------------------------------------------------------- Activity Log 

            ViewBag.TicketReceived = (from s in db.Ticket where s.FE_ID == id select s).Count();

            ViewBag.TicketExecuted = (from s in db.Ticket where s.FE_ID == id && s.Status == 20 select s).Count();

            ViewBag.TicketDenied = (from s in db.Ticket_FE_Selection where s.FE_ID == id && s.Status == "Reject" select s).Count();


            FECSAT fc = new FECSAT();

            fc = db.Database.SqlQuery<FECSAT>("getFECSAT @id", new SqlParameter("@id", id)).FirstOrDefault();

            ViewBag.CSAT = fc.CSAT + "/5" + " [" + fc.Count + "]";

            //------------------------------------------------------------- Activity Log Ends

            return View(MP);
        }


        [HttpGet]
        public ActionResult BulksFEGreeting()
        {


            var fe = (from s in db.FE_Master_Personal select s).ToList();
            foreach (FE_Master_Personal fp in fe)
            {
                try
                {
                    int cnts = (from s in db.EmailLogs where s.Emailid.Trim() == fp.Email.Trim() select s).Count();
                    if (cnts > 0)
                    {

                    }
                    else
                    {
                        utlity.sendmailGreeting(fp.Email, "Greeting :: From Inwinteck", "Greetings <support@inwinteck.com>");

                    }

                }
                catch (Exception ex)
                {
                    string msg = "/Greetings/log" + " " + "Email: " + fp.Email + " " + " Status: Error :: " + ex.InnerException.ToString();
                    utlity.createlog(msg);
                }
            }



            return View();
        }

        //----------------------- FE Profile Ends ---------------------------
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetLocation(string PinCode)
        {
            string res = "";

            PinCodeMaster Location = new PinCodeMaster();
            Location = (from c in db.PinCode where c.pincode == PinCode && c.status == 1 select c).FirstOrDefault();
            if (Location != null)
            {
                res = "Success";
            }
            return Json(new { res, Location }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetCountryCode(string Cnt)
        {
            string res = "";
            Country_Dialing_Code CC = new Country_Dialing_Code();
            CC = (from c in db.Country_Dialing_Code where c.Country == Cnt && c.Status == 1 select c).FirstOrDefault();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetOffice(int Id)
        {
            string res = "";
            EU_Master_Branch CC = new EU_Master_Branch();
            CC = (from c in db.EU_Master_Branch where c.Id == Id select c).FirstOrDefault();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetOfficeContact(int Id)
        {
            string res = "";
            List<EU_Master_Contacts> CC = new List<EU_Master_Contacts>();
            CC = (from c in db.EU_Master_Contacts where c.Office_ID == Id select c).ToList();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetContact(int Id)
        {
            string res = "";
            EU_Master_Contacts CC = new EU_Master_Contacts();
            CC = (from c in db.EU_Master_Contacts where c.Id == Id select c).FirstOrDefault();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetOfficeSales(int Id)
        {
            string res = "";
            EU_Master_Branch_Sales CC = new EU_Master_Branch_Sales();
            CC = (from c in db.EU_Master_Branch_Sales where c.Id == Id select c).FirstOrDefault();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetOfficeContactSales(int Id)
        {
            string res = "";
            List<EU_Master_Contacts_Sales> CC = new List<EU_Master_Contacts_Sales>();
            CC = (from c in db.EU_Master_Contacts_Sales where c.Office_ID == Id select c).ToList();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetContactSales(int Id)
        {
            string res = "";
            EU_Master_Contacts_Sales CC = new EU_Master_Contacts_Sales();
            CC = (from c in db.EU_Master_Contacts_Sales where c.Id == Id select c).FirstOrDefault();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ContactEUMasterSales(EU_Master_Contacts_Sales sa, int Contact_ID)
        {
            if (Contact_ID != 0)
            {
                try
                {
                    sa.Id = Contact_ID;
                    sa.CreatedBy = (from s in db.EU_Master_Contacts_Sales where s.Id == Contact_ID select s.CreatedBy).FirstOrDefault();
                    sa.CreatedOn = (from s in db.EU_Master_Contacts_Sales where s.Id == Contact_ID select s.CreatedOn).FirstOrDefault();
                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    TempData["message"] = "Client Master's Sales Contact Updated !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Client Master's Sales Contact Not Updated !! ";
                }
            }
            else
            {
                try
                {
                    sa.CreatedBy = User.Identity.GetUserId();
                    sa.CreatedOn = DateTime.Now;
                    db.EU_Master_Contacts_Sales.Add(sa);
                    db.SaveChanges();


                    TempData["message"] = "Client Master's Sales Contact Added !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Client Master's Sales Contact Not Created !! ";
                }
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult deleteContactSaless(int OC)
        {
            int od = (from s in db.EU_Master_Contacts_Sales where s.Id == OC select s.Office_ID).FirstOrDefault();
            int ED = (from s in db.EU_Master_Branch_Sales where s.Id == od select s.EU_ID).FirstOrDefault();
            if (OC > 0)
            {

                EU_Master_Contacts_Sales sa = new EU_Master_Contacts_Sales();
                sa = (from c in db.EU_Master_Contacts_Sales where c.Id == OC select c).FirstOrDefault();
                if (sa != null)
                {
                    db.Entry(sa).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }

            }
            else
            {
                TempData["message"] = "Empty Data !! ";
            }


            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetContactSaless(int Cnt)
        {
            string res = "";
            List<EU_Master_Contacts_Sales> CC = new List<EU_Master_Contacts_Sales>();
            CC = (from c in db.EU_Master_Contacts_Sales where c.Office_ID == Cnt select c).ToList();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetOfficeSaless(int Cnt)
        {
            string res = "";
            List<EU_Master_Branch_Sales> CC = new List<EU_Master_Branch_Sales>();
            CC = (from c in db.EU_Master_Branch_Sales where c.EU_ID == Cnt select c).ToList();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEnqClientHistory(int tn)
        {
            string tv = "";
            List<TicketHist> hist = new List<TicketHist>();
            hist = db.Database.SqlQuery<TicketHist>("getClientEnqhistory @id", new SqlParameter("@id ", tn)).ToList();

            if (hist != null)
            {
                tv = "success";

            }
            else
            {
                tv = "failure";
            }


            return Json(new { tv, hist }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetCurrency(string PinCode)
        {
            string res = "";

            string Location;
            Location = (from c in db.Currency_Master where c.Country == PinCode && c.Status == 1 select c.Currency).FirstOrDefault();
            if (Location != null)
            {
                res = "Success";
            }
            return Json(new { res, Location }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFEOtherDetail(int FEID)
        {
            string res = "";
            List<FE_Master_Other_Detail_History> PZ = new List<FE_Master_Other_Detail_History>();
            try
            {
                PZ = (from s in db.FE_Master_Other_Detail
                      where s.FE_ID == FEID
                      select new FE_Master_Other_Detail_History { Other_Details = s.Other_Details, CreatedOn = s.CreatedOn.ToString() }).ToList();
                res = "Success";
            }
            catch (Exception ex)
            {
                res = "Error";
            }
            return Json(new { res, PZ }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFEOtherExtraCertDetail(int FEID)
        {
            string res = "";
            List<FE_Master_Other_Extra_Detail_History> PZ = new List<FE_Master_Other_Extra_Detail_History>();
            try
            {
                PZ = (from s in db.FE_Master_Certification_Extra_Detail
                      where s.FE_ID == FEID
                      select new FE_Master_Other_Extra_Detail_History { Other_Details = s.Other_Details, CreatedOn = s.CreatedOn.ToString() }).ToList();
                res = "Success";
            }
            catch (Exception ex)
            {
                res = "Error";
            }
            return Json(new { res, PZ }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GETFERegCount()
        {
            string res = "";
            int cnt;
            try
            {
                cnt = (from s in db.FE_Master_Personal select s).Count();
                res = "Success";
            }
            catch (Exception ex)
            {
                res = "Error";
                cnt = 0;
            }
            return Json(new { res, cnt }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult DeleteBlackCert(int CertificateId, int FeID)
        //{
        //   // int FeID = sa.FE.Id;
        //    string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        // Open the connection
        //        connection.Open();

                
        //        using (SqlCommand command = new SqlCommand("DeleteBlacklistItem", connection))
        //        {
                    
        //            command.CommandType = CommandType.StoredProcedure;

                    
        //            command.Parameters.Add(new SqlParameter("@CertificateId", CertificateId));

                   
        //            command.Parameters.Add(new SqlParameter("@FeID", FeID));

        //            // Execute the stored procedure
        //            int rowsAffected = command.ExecuteNonQuery();

        //            if (rowsAffected > 0)
        //            {
        //                // Deletion was successful
        //                return Json(new { success = true });
        //            }
        //            else
        //            {
        //                // Deletion failed or no records were deleted
        //                return Json(new { success = false });
        //            }
        //        }
        //    }
        //}
    }

}