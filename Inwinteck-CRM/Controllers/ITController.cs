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
using Inwinteck_CRM.Models;


namespace Inwinteck_CRM.Controllers
{
    [Authorize]
    public class ITController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Transaction
     string url = "https://fms.inwinteck.com/Upload/";
        //    string url = "http://localhost:1957/Upload/";
        string mailer = "Support<support@inwinteck.com>";

        //string url = "http://inwinteckcrm.3sptechmind.com/Upload/";
        public ActionResult Vendor_Master(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
            // To check user access right
            string uri1 = "/IT/Vendor_Master";

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
            List<Vendor_Master> li = new List<Vendor_Master>();
            if (searchtype != "" && searchtext != "")
            {
                if (searchtype == "Name")
                {
                    pgc = db.Vendor_Master.Where(x => x.Customer_Name.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.Vendor_Master
                          orderby c.Customer_Name
                          select c).Where(x => x.Customer_Name.StartsWith(searchtext)).ToList();
                }
                else if (searchtype == "Email")
                {
                    pgc = db.Vendor_Master.Where(x => x.Customer_Email.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.Vendor_Master
                          orderby c.Customer_Email
                          select c).Where(x => x.Customer_Email.StartsWith(searchtext)).ToList();
                }
                ViewBag.Search = searchtext;

            }
            else
            {
                pgc = db.Vendor_Master.Count();
                pageCount = pgc / pageSize;

                li = (from s in db.Vendor_Master orderby s.Customer_Name select s).Skip(pageNo * pageSize).Take(pageSize).ToList();
                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }

            return View(li);
        }
        public ActionResult addVendorMaster()
        {
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addVendorMaster(Vendor_Master sa)
        {
            try
            {
                int cnt = (from s in db.Vendor_Master where s.Customer_Email == sa.Customer_Email select s).Count();
                if (cnt > 0)
                {
                    TempData["message"] = "Vendor already Register with us !!";
                    return RedirectToAction("addVendorMaster", "IT");
                }
                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.Vendor_Master.Add(sa);
                db.SaveChanges();


                TempData["link"] = "Yes";
                TempData["message"] = "Vendor Master Added !!";

            }
            catch (Exception ex)
            {
                TempData["message"] = "Vendor Master Not Created !! ";
            }
            return RedirectToAction("EditVendorMaster", "IT", new { id = sa.Id });
        }

        public ActionResult EditVendorMaster(int id)
        {
            // To check user access right
            string uri1 = "/IT/Vendor_Master";

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
            Vendor_Master sa = new Vendor_Master();
            sa = (from s in db.Vendor_Master where s.Id == id select s).FirstOrDefault();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();

            return View(sa);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditVendorMaster(Vendor_Master sa)
        {
            try
            {

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "Vendor Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "Vendor Not Updated !! ";
            }
            return RedirectToAction("EditVendorMaster", "IT", new { id = sa.Id });
        }

        public ActionResult viewVendorMaster()
        {
            return View();
        }

        public ActionResult statusVendorMaster(int id)
        {
            // To check user access right
            string uri1 = "/IT/Vendor_Master";

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
            Vendor_Master sa = new Vendor_Master();
            sa = (from s in db.Vendor_Master where s.Id == id select s).FirstOrDefault();
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
            return RedirectToAction("Vendor_Master", "IT");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ContactVendorMaster(Vendor_Master_Contacts sa, int Contact_ID)
        {
            if (Contact_ID != 0)
            {
                try
                {
                    sa.Id = Contact_ID;
                    sa.CreatedBy = (from s in db.Vendor_Master_Contacts where s.Id == Contact_ID select s.CreatedBy).FirstOrDefault();
                    sa.CreatedOn = (from s in db.Vendor_Master_Contacts where s.Id == Contact_ID select s.CreatedOn).FirstOrDefault();
                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    TempData["message"] = "Vendor Master's Contact Updated !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Vendor Master's Contact Not Updated !! ";
                }
            }
            else
            {
                try
                {
                    sa.CreatedBy = User.Identity.GetUserId();
                    sa.CreatedOn = DateTime.Now;
                    db.Vendor_Master_Contacts.Add(sa);
                    db.SaveChanges();


                    TempData["message"] = "Vendor Master's Contact Added !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "EU Master's Contact Not Created !! ";
                }
            }

            return RedirectToAction("EditVendorMaster", "IT", new { id = sa.Vendor_ID });
        }

        public ActionResult deleteContact(int OC)
        {
            int ED = (from s in db.Vendor_Master_Contacts where s.Id == OC select s.Vendor_ID).FirstOrDefault();
            if (OC > 0)
            {

                Vendor_Master_Contacts sa = new Vendor_Master_Contacts();
                sa = (from c in db.Vendor_Master_Contacts where c.Id == OC select c).FirstOrDefault();
                if (sa != null)
                {
                    db.Entry(sa).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    return RedirectToAction("EditVendorMaster", "IT", new { id = ED });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("EditVendorMaster", "IT", new { id = ED });
            }

            return View();
        }


        //===========================================================================================================================================

        public ActionResult Customer_Master(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
            // To check user access right
            string uri1 = "/IT/Customer_Master";

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
            List<IT_Customer_Master> li = new List<IT_Customer_Master>();
            if (searchtype != "" && searchtext != "")
            {
                if (searchtype == "Name")
                {
                    pgc = db.IT_Customer_Master.Where(x => x.Customer_Name.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.IT_Customer_Master
                          orderby c.Customer_Name
                          select c).Where(x => x.Customer_Name.StartsWith(searchtext)).ToList();
                }
                else if (searchtype == "Email")
                {
                    pgc = db.IT_Customer_Master.Where(x => x.Customer_Email.Contains(searchtext)).Count();
                    pageCount = pgc / pageSize;
                    li = (from c in db.IT_Customer_Master
                          orderby c.Customer_Email
                          select c).Where(x => x.Customer_Email.StartsWith(searchtext)).ToList();
                }
                ViewBag.Search = searchtext;

            }
            else
            {
                pgc = db.IT_Customer_Master.Count();
                pageCount = pgc / pageSize;

                li = (from s in db.IT_Customer_Master orderby s.Customer_Name select s).Skip(pageNo * pageSize).Take(pageSize).ToList();
                ViewBag.pageNo = pageNo;
                ViewBag.pageCount = pageCount;
            }

            return View(li);
        }
        public ActionResult addCustomerMaster()
        {
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addCustomerMaster(IT_Customer_Master sa)
        {
            try
            {
                int cnt = (from s in db.IT_Customer_Master where s.Customer_Email == sa.Customer_Email select s).Count();
                int cnt1 = (from s in db.IT_Customer_Master where s.Enq_Abbreviation == sa.Enq_Abbreviation select s).Count();
                if (cnt > 0)
                {
                    TempData["message"] = "Customer already Register with us !!";
                    return RedirectToAction("addCustomerMaster", "Master");
                }
                else if (cnt1 > 0)
                {
                    TempData["message"] = "Customer Abbreviation already Taken !!";
                    return RedirectToAction("addCustomerMaster", "Master");
                }

                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.IT_Customer_Master.Add(sa);
                db.SaveChanges();


                TempData["link"] = "Yes";
                TempData["message"] = "Customer Master Added !!";

            }
            catch (Exception ex)
            {
                TempData["message"] = "Customer Master Not Created !! ";
            }
            return RedirectToAction("EditCustomerMaster", "IT", new { id = sa.Id });
        }

        public ActionResult EditCustomerMaster(int id)
        {
            // To check user access right
            string uri1 = "/IT/Customer_Master";

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
            IT_Customer_Master sa = new IT_Customer_Master();
            sa = (from s in db.IT_Customer_Master where s.Id == id select s).FirstOrDefault();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).ToList();

            return View(sa);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditCustomerMaster(IT_Customer_Master sa)
        {
            try
            {

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["link"] = "Yes";
                TempData["message"] = "Customer Updated !!";


            }
            catch (Exception ex)
            {
                TempData["message"] = "Customer Not Updated !! ";
            }
            return RedirectToAction("EditCustomerMaster", "IT", new { id = sa.Id });
        }

        public ActionResult viewCustomerMaster()
        {
            return View();
        }

        public ActionResult statusCustomerMaster(int id)
        {
            // To check user access right
            string uri1 = "/IT/Customer_Master";

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
            IT_Customer_Master sa = new IT_Customer_Master();
            sa = (from s in db.IT_Customer_Master where s.Id == id select s).FirstOrDefault();
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
            return RedirectToAction("Customer_Master", "IT");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ContactCustomerMaster(IT_Customer_Contacts sa, int Contact_ID)
        {
            if (Contact_ID != 0)
            {
                try
                {
                    sa.Id = Contact_ID;
                    sa.CreatedBy = (from s in db.Vendor_Master_Contacts where s.Id == Contact_ID select s.CreatedBy).FirstOrDefault();
                    sa.CreatedOn = (from s in db.Vendor_Master_Contacts where s.Id == Contact_ID select s.CreatedOn).FirstOrDefault();
                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    TempData["message"] = "Customer Master's Contact Updated !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Customer Master's Contact Not Updated !! ";
                }
            }
            else
            {
                try
                {
                    sa.CreatedBy = User.Identity.GetUserId();
                    sa.CreatedOn = DateTime.Now;
                    db.IT_Customer_Contacts.Add(sa);
                    db.SaveChanges();


                    TempData["message"] = "Customer Master's Contact Added !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Customer Master's Contact Not Created !! ";
                }
            }

            return RedirectToAction("EditCustomerMaster", "IT", new { id = sa.IT_Customer_ID });
        }

        public ActionResult deleteContactCustomer(int OC)
        {
            int ED = (from s in db.IT_Customer_Contacts where s.Id == OC select s.IT_Customer_ID).FirstOrDefault();
            if (OC > 0)
            {

                IT_Customer_Contacts sa = new IT_Customer_Contacts();
                sa = (from c in db.IT_Customer_Contacts where c.Id == OC select c).FirstOrDefault();
                if (sa != null)
                {
                    db.Entry(sa).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    TempData["message"] = "Data Deleted !! ";
                    return RedirectToAction("EditCustomerMaster", "IT", new { id = ED });
                }
            }
            else
            {
                TempData["message"] = "Empty Data !! ";
                return RedirectToAction("EditCustomerMaster", "IT", new { id = ED });
            }

            return View();
        }


        public ActionResult Enquiry(int pageNo = 0, string searchtype = "", string searchtext = "")
        {
            // To check user access right
            string uri1 = "/IT/Enquiry";

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

            int pageSize = 10;
            int pgc = 0;
            int pageCount = 0;
            List<EnqDetails> li = new List<EnqDetails>();
            //if (searchtype != "" && searchtext != "")
            //{
            //    if (searchtype == "Name")
            //    {
            //        pgc = db.Enq_IT.Where(x => x.Source_Name.Contains(searchtext)).Count();
            //        pageCount = pgc / pageSize;
            //        li = (from c in db.Enq_IT
            //              orderby c.Source_Name
            //              select c).Where(x => x.Source_Name.StartsWith(searchtext)).ToList();
            //    }
            //    else if (searchtype == "Email")
            //    {
            //        pgc = db.Enq_IT.Where(x => x.Email.Contains(searchtext)).Count();
            //        pageCount = pgc / pageSize;
            //        li = (from c in db.Enq_IT
            //              orderby c.Email
            //              select c).Where(x => x.Email.StartsWith(searchtext)).ToList();
            //    }
            //    ViewBag.Search = searchtext;

            //}
            //else
            //{
            //    pgc = db.Enq_IT.Count();
            //    pageCount = pgc / pageSize;

            li = db.Database.SqlQuery<EnqDetails>("getRecruitDetails").Skip(pageNo * pageSize).Take(pageSize).ToList();
            ViewBag.pageNo = pageNo;
            ViewBag.pageCount = pageCount;
            //}

            return View(li);
        }

        public ActionResult addEnquiry()
        {
            ViewBag.Customer = (from c in db.IT_Customer_Master where c.Status == 1 select new SelectListItem { Text = c.Customer_Name, Value = c.Id.ToString() }).ToList();
            ViewBag.Chat_mode = (from c in db.HeaderDescription where c.header_id == 3 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult addEnquiry(Enq_IT sa,string Comments)
        {
            try
            {
                
                sa.Enq_No = utlity.CheckEnqNo(sa.Customer_Id);
                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.Enq_IT.Add(sa);
                int res = db.SaveChanges();
                if (res > 0)
                {
                    Enq_History th = new Enq_History();
                    th.Enq_Id = sa.Id;
                    th.Comments = Comments;
                    th.Status = sa.Status;
                    th.CreatedBy = User.Identity.GetUserId();
                    th.CreatedOn = DateTime.Now;
                    db.Enq_History.Add(th);
                    db.SaveChanges();
                  
                }

                TempData["message"] = "Enquiry Created : Enquiry No !!" + sa.Enq_No;

            }
            catch (Exception ex)
            {
                TempData["message"] = "Enquiry Not Created !! ";
            }
            return RedirectToAction("EditEquiry", "IT", new { id = sa.Id });
        }
        public ActionResult EditEquiry(int id)
        {
            // To check user access right
            string uri1 = "/IT/Enquiry";

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
            Enq_IT ti = new Enq_IT();
            ti = (from s in db.Enq_IT where s.Id == id select s).FirstOrDefault();

            ViewBag.DialCode = (from c in db.Country_Dialing_Code where c.Status == 1 select new SelectListItem { Text = c.Code + " " + c.Country, Value = c.Code.Trim() }).ToList();
            ViewBag.Chat_mode = (from c in db.HeaderDescription where c.header_id == 3 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Customer = (from c in db.IT_Customer_Master where c.Id == ti.Customer_Id select c.Customer_Name).FirstOrDefault();
            ViewBag.Vendor = (from c in db.Vendor_Master where c.Status == 1 select new SelectListItem { Text = c.Customer_Name, Value = c.Id.ToString() }).ToList();
            ViewBag.VendorEmail = (from s in db.Enq_Vendor_Email where s.Enq_Id == ti.Id select s).Count();
            ViewBag.CustomerEmail = (from s in db.Enq_Customer_Email where s.Enq_Id == ti.Id select s).Count();
            ViewBag.Candidate = (from c in db.Enq_Customer_Email where c.Enq_Id == ti.Id select new SelectListItem { Text = c.Candidate_Name, Value = c.Id.ToString() }).ToList();
            return View(ti);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditEquiry(Enq_IT sa, string Comments)
        {
            try
            {

                sa.ModifiedBy = User.Identity.GetUserId();
                sa.ModifiedOn = DateTime.Now;
                db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                int res = db.SaveChanges();
                if (res > 0)
                {
                    Enq_History th = new Enq_History();
                    th.Enq_Id = sa.Id;
                    th.Comments = Comments;
                    th.Status = sa.Status;
                    th.CreatedBy = User.Identity.GetUserId();
                    th.CreatedOn = DateTime.Now;
                    db.Enq_History.Add(th);
                    db.SaveChanges();

                }

                if (sa.Status == 3)
                {
                    TempData["message"] = "Enquiry Closed : Enquiry No !!" + sa.Enq_No;
                    return RedirectToAction("Enquiry", "IT");
                }

                TempData["message"] = "Enquiry Update : Enquiry No !!" + sa.Enq_No;

            }
            catch (Exception ex)
            {
                TempData["message"] = "Enquiry Not Created !! ";
            }
            return RedirectToAction("EditEquiry", "IT", new { id = sa.Id });
        }

        public ActionResult ViewEquiry(int id)
        {
            // To check user access right
            string uri1 = "/IT/Enquiry";

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
            Enq_IT ti = new Enq_IT();
            ti = (from s in db.Enq_IT where s.Id == id select s).FirstOrDefault();

            ViewBag.DialCode = (from c in db.Country_Dialing_Code where c.Status == 1 select new SelectListItem { Text = c.Code + " " + c.Country, Value = c.Code.Trim() }).ToList();
            ViewBag.Chat_mode = (from c in db.HeaderDescription where c.header_id == 3 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Customer = (from c in db.IT_Customer_Master where c.Id == ti.Customer_Id select c.Customer_Name).FirstOrDefault();
            ViewBag.Vendor = (from c in db.Vendor_Master where c.Status == 1 select new SelectListItem { Text = c.Customer_Name, Value = c.Id.ToString() }).ToList();
            ViewBag.VendorEmail = (from s in db.Enq_Vendor_Email where s.Enq_Id == ti.Id select s).Count();
            ViewBag.CustomerEmail = (from s in db.Enq_Customer_Email where s.Enq_Id == ti.Id select s).Count();
            ViewBag.Candidate = (from c in db.Enq_Customer_Email where c.Enq_Id == ti.Id select new SelectListItem { Text = c.Candidate_Name, Value = c.Id.ToString() }).ToList();
            return View(ti);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EquiryVendor(Enq_Vendor_Email sa)
        {
            try
            {

                
                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.Enq_Vendor_Email.Add(sa);
                db.SaveChanges();
                

                TempData["message"] = "Email Send  !!" ;

                Enq_IT EI = (from s in db.Enq_IT where s.Id == sa.Enq_Id select s).FirstOrDefault();
                
                string body = utlity.EnqVendor(EI.Job_Title,EI.Job_Location,EI.Job_Exp,EI.Job_Duration,EI.Job_Description,EI.Candidate_Rate,EI.Closing_Date.Value.ToString("dd-MM-yyyy"));
                utlity.sendmail(sa.Email_Vendor, sa.Email_Subject, body, mailer);

            }
            catch (Exception ex)
            {
                TempData["message"] = "Email Not Send  !! ";
            }
            return RedirectToAction("EditEquiry", "IT", new { id = sa.Enq_Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EquiryCustomer(Enq_Customer_Email sa, HttpPostedFileBase Enq_Resume)
        {
            try
            {
                string pic = "";
                string pathtest = "";
                string fileName;
                if (Enq_Resume != null)
                {

                    pic = System.IO.Path.GetFileName(Enq_Resume.FileName);
                    fileName = sa.Id + DateTime.Now.ToString("yyyyMMddHHmmssFFF") + "_" + pic;
                    pathtest = System.IO.Path.Combine(Server.MapPath("~/Upload/ITResume"), fileName);
                    Enq_Resume.SaveAs(pathtest);

                    sa.Resume = url + "ITResume/" + fileName;
                }

                sa.CreatedBy = User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                db.Enq_Customer_Email.Add(sa);
                db.SaveChanges();


                TempData["message"] = "Email Send  !!";

                int ce = (from s in db.Enq_IT where s.Id == sa.Enq_Id select s.Customer_Id).FirstOrDefault();
                string name = (from s in db.IT_Customer_Master where s.Id == ce select s.Customer_Name).FirstOrDefault();

                string body = utlity.EnqCustomer(name,sa.Candidate_Name,sa.Proposed_Rate);
                utlity.sendmailEnq(sa.Customer_Email, sa.Subject, body, mailer, pathtest);

            }
            catch (Exception ex)
            {
                TempData["message"] = "Email Not Send  !! ";
            }
            return RedirectToAction("EditEquiry", "IT", new { id = sa.Enq_Id });
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetVendorContact(int Id)
        {
            string res = "";
            List<Vendor_Master_Contacts> CC = new List<Vendor_Master_Contacts>();
            CC = (from c in db.Vendor_Master_Contacts where c.Vendor_ID == Id select c).ToList();
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
            Vendor_Master_Contacts CC = new Vendor_Master_Contacts();
            CC = (from c in db.Vendor_Master_Contacts where c.Id == Id select c).FirstOrDefault();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetCustomerContact(int Id)
        {
            string res = "";
            List<IT_Customer_Contacts> CC = new List<IT_Customer_Contacts>();
            CC = (from c in db.IT_Customer_Contacts where c.IT_Customer_ID == Id select c).ToList();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetContactCst(int Id)
        {
            string res = "";
            IT_Customer_Contacts CC = new IT_Customer_Contacts();
            CC = (from c in db.IT_Customer_Contacts where c.Id == Id select c).FirstOrDefault();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ContactCustMaster(IT_Customer_Contacts sa, int Contact_ID)
        {
            if (Contact_ID != 0)
            {
                try
                {
                    sa.Id = Contact_ID;
                    sa.CreatedBy = (from s in db.IT_Customer_Contacts where s.Id == Contact_ID select s.CreatedBy).FirstOrDefault();
                    sa.CreatedOn = (from s in db.IT_Customer_Contacts where s.Id == Contact_ID select s.CreatedOn).FirstOrDefault();
                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    TempData["message"] = "Customer Master's Contact Updated !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Customer Master's Contact Not Updated !! ";
                }
            }
            else
            {
                try
                {
                    sa.CreatedBy = User.Identity.GetUserId();
                    sa.CreatedOn = DateTime.Now;
                    db.IT_Customer_Contacts.Add(sa);
                    db.SaveChanges();


                    TempData["message"] = "Customer Master's Contact Added !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Customer Master's Contact Not Created !! ";
                }
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult deleteCustomer(int OC)
        {
            int od = (from s in db.IT_Customer_Contacts where s.Id == OC select s.IT_Customer_ID).FirstOrDefault();
            if (OC > 0)
            {

                IT_Customer_Contacts sa = new IT_Customer_Contacts();
                sa = (from c in db.IT_Customer_Contacts where c.Id == OC select c).FirstOrDefault();
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
        public JsonResult GetEnqHistory(int tn)
        {
            string tv = "";
            List<EnqHist> hist = new List<EnqHist>();
            hist = db.Database.SqlQuery<EnqHist>("getEnqhistory @id", new SqlParameter("@id ", tn)).ToList();

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
        public JsonResult GetContactVend(int Id)
        {
            string res = "";
            Vendor_Master_Contacts CC = new Vendor_Master_Contacts();
            CC = (from c in db.Vendor_Master_Contacts where c.Id == Id select c).FirstOrDefault();
            if (CC != null)
            {
                res = "Success";
            }
            return Json(new { res, CC }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ContactVendMaster(Vendor_Master_Contacts sa, int VContact_ID)
        {
            if (VContact_ID != 0)
            {
                try
                {
                    sa.Id = VContact_ID;
                    sa.CreatedBy = (from s in db.Vendor_Master_Contacts where s.Id == VContact_ID select s.CreatedBy).FirstOrDefault();
                    sa.CreatedOn = (from s in db.Vendor_Master_Contacts where s.Id == VContact_ID select s.CreatedOn).FirstOrDefault();
                    sa.ModifiedBy = User.Identity.GetUserId();
                    sa.ModifiedOn = DateTime.Now;
                    db.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    TempData["message"] = "Vendor Master's Contact Updated !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Vendor Master's Contact Not Updated !! ";
                }
            }
            else
            {
                try
                {
                    sa.CreatedBy = User.Identity.GetUserId();
                    sa.CreatedOn = DateTime.Now;
                    db.Vendor_Master_Contacts.Add(sa);
                    db.SaveChanges();


                    TempData["message"] = "Vendor Master's Contact Added !!";

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Vendor Master's Contact Not Created !! ";
                }
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult deleteVendor(int OC)
        {
            int od = (from s in db.Vendor_Master_Contacts where s.Id == OC select s.Vendor_ID).FirstOrDefault();
            if (OC > 0)
            {

                Vendor_Master_Contacts sa = new Vendor_Master_Contacts();
                sa = (from c in db.Vendor_Master_Contacts where c.Id == OC select c).FirstOrDefault();
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
        public JsonResult GetVendorEnqEmail(int Id)
        {
            string tv = "";
            List<EnqVenEmail> hist = new List<EnqVenEmail>();
            hist = db.Database.SqlQuery<EnqVenEmail>("getEnqVendorEmail @id", new SqlParameter("@id ", Id)).ToList();

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
        public JsonResult GetCustomerEnqEmail(int Id)
        {
            string tv = "";
            List<EnqCustEmail> hist = new List<EnqCustEmail>();
            hist = db.Database.SqlQuery<EnqCustEmail>("getEnqCustEmail @id", new SqlParameter("@id ", Id)).ToList();

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
    }
}