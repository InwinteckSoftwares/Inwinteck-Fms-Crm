using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Data.Entity.SqlServer;
using Inwinteck_CRM.Models;

namespace Inwinteck_CRM.Controllers
{
    public class PartialController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        public PartialController()
        {
        }

        public PartialController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;

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
        // GET: Partial
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Menu()
        {
            Menu m = new Menu();
            string uid = User.Identity.GetUserId();
         
                m.mm = db.Database.SqlQuery<menu_master>("GetMainMenu @id", new SqlParameter("@id", uid)).OrderBy(x => x.menu_display_order).ToList();
                //    m.mm = db.Database.SqlQuery<menu_master>("GetMenu").OrderBy(x => x.menu_display_order).ToList();
                m.fl = db.Database.SqlQuery<firstLevel>("GetFirstLevel @id", new SqlParameter("@id", uid)).OrderBy(x => x.menu_display_order).ToList();
                m.sl = db.Database.SqlQuery<secondlevel>("GetSecondLevel @id", new SqlParameter("@id", uid)).OrderBy(x => x.display_order_no).ToList();



            // string uid = User.Identity.GetUserId();

            if (UserManager.IsInRole(uid, "Admin"))
            {

                ViewBag.Link = "IndexAdmin";
            }
            else if (UserManager.IsInRole(uid, "Help Desk Manager"))
            {
                ViewBag.Link = "IndexHelpDesk";
            }
            else if (UserManager.IsInRole(uid, "Sr.Help Desk Manager"))
            {
                ViewBag.Link = "IndexSrHelpDesk";
            }
            else
            {
                ViewBag.Link = "Index";
            }

            ViewBag.Name = (from s in db.Users where s.Id == uid select s.Name).FirstOrDefault();

          
            return PartialView(m);
        }
    }
}