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
using EntityFramework.Extensions;
using Inwinteck_CRM.Models;

namespace Inwinteck_CRM.Controllers
{
    [Authorize]
    public class SecurityController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext db = new ApplicationDbContext();
        public SecurityController()
        {
        }

        public SecurityController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: Security
        public ActionResult Index(string RoleId, string UserId = "")
        {
            // To check user access right
            string uri1 = "/Security/Index";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            security_management sm = new security_management();
            List<SelectListItem> Usr = new List<SelectListItem>();
            Usr = (from c in db.Users select new SelectListItem { Text = c.UserName, Value = c.Id.TrimStart() }).ToList();
            ViewBag.User = Usr;
            List<SelectListItem> Role = new List<SelectListItem>();
            Role = (from c in db.Roles select new SelectListItem { Text = c.Name, Value = c.Id.TrimStart() }).ToList();
            ViewBag.Role = Role;
            sm.Role = (from c in db.Roles select new RoleList { id = c.Id, name = c.Name }).ToList();
            sm.UserManage = (from c in db.Users select new user_manage { UserId = c.Id.TrimStart(), UserName = c.UserName }).ToList();

            List<menu_master> mn = new List<menu_master>();
            sm.MenuMaster = db.Database.SqlQuery<menu_master>("GetMenu").ToList();

            sm.Submenu = (from c in db.menu_master join b in db.component_master on c.menu_id equals b.menu_id where c.parent_menu_id != 0 select c).Distinct().ToList();
            sm.Previlege = (from c in db.privilege_master select c).ToList();
            if (UserId != "")
            {
                sm.UserPermission = (from c in db.user_permission_map where c.user_id == UserId select c).ToList();
            }
            else if (Role != null)
            {
                sm.UserPermission = (from c in db.user_permission_map where c.user_role_id == RoleId select c).ToList();
            }
            else
            {
                sm.UserPermission = (from c in db.user_permission_map where c.user_role_id == RoleId select c).ToList();
            }

            sm.ComponentMaster = (from c in db.component_master select c).ToList();
            sm.UP = null;
            return View(sm);
        }
        [HttpPost]
        public ActionResult Index(security_management sm)
        {
            user_permission_map um = new user_permission_map();

            List<user_permission_map> upm = new List<user_permission_map>();
            List<user_permission_map> upminsert = new List<user_permission_map>();
            if (sm.UserId != "")
            {
                int j = db.user_permission_map.Where(x => x.user_id == sm.UserId).Delete();
                //upm = (from c in db.user_permission_map where c.user_id == sm.UserId select c).ToList();
            }
            if (sm.RoleId != "")
            {
                sm.UserId = "";
                int k = db.user_permission_map.Where(x => x.user_role_id == sm.RoleId && sm.UserId == "").Delete();
                //upm = (from c in db.user_permission_map where c.user_id == sm.RoleId select c).ToList();
            }
            try
            {
                if (sm != null)
                {

                    for (int i = 0; i < sm.UP.Count; i++)
                    {
                        if (sm.UP[i].record_insert != false || sm.UP[i].record_delete != false || sm.UP[i].record_update != false || sm.UP[i].record_view != false || sm.UP[i].record_import != false || sm.UP[i].record_export != false)
                        {

                            um.user_id = sm.UserId;
                            um.business_object_id = sm.UP[i].business_object_id;
                            um.user_role_id = sm.RoleId;
                            um.record_insert = sm.UP[i].record_insert;
                            um.record_delete = sm.UP[i].record_delete;
                            um.record_update = sm.UP[i].record_update;
                            um.record_view = sm.UP[i].record_view;
                            um.record_import = sm.UP[i].record_import;
                            um.record_export = sm.UP[i].record_export;
                            db.user_permission_map.Add(um);
                            db.SaveChanges();
                            TempData["message"] = "Saved !!!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error Occurred , While Saving !!!";


            }
            return RedirectToAction("Index");
        }

        

        public ActionResult Roles()
        {
            // To check user access right
            string uri1 = "/Security/Roles";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends 
            List<RoleList> sa = new List<RoleList>();
            sa = (from s in db.Roles select new RoleList { id = s.Id, name = s.Name }).ToList();
            return View(sa);
        }
        public ActionResult CreateRoles()
        {
            // To check user access right
            string uri1 = "/Security/Roles";

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
            return View();
        }

        [HttpPost]
        public ActionResult CreateRoles(Role sa)
        {
            try
            {
                if (sa != null)
                {


                    db.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                    {
                        Name = sa.Name
                    });
                    db.SaveChanges();

                    TempData["message"] = "Data Saved !!";
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                TempData["message"] = "Error occurred while Saving !!";
            }
            return RedirectToAction("CreateRoles", "Security");
        }



        public ActionResult Users()
        {
            // To check user access right
            string uri1 = "/Security/Users";

            int bid = utlity.GetBussinessId(uri1);
            string ucheckid = User.Identity.GetUserId();

            checkpermission cm = new checkpermission();

            cm = db.Database.SqlQuery<checkpermission>("Access @id,@bid", new SqlParameter("@id", ucheckid), new SqlParameter("@bid", bid)).FirstOrDefault();
            if (cm == null)
            {
                return RedirectToAction("UnAuthorisedAccess", "Dashboard");
            }
            // access right ends
            List<userlist> ul = new List<userlist>();
            ul = (from u in db.Users
                  from ur in u.Roles
                  join r in db.Roles on ur.RoleId equals r.Id
                  orderby u.CreatedOn descending
                  select new userlist
                  {
                      id = u.Id,
                      Name = u.Name,
                      email = u.Email,
                      role = r.Name,
                      status = u.Status
                  }).ToList();
            return View(ul);
        }

        public ActionResult CreateUsers()
        {
            // To check user access right
            string uri1 = "/Security/Users";

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
            List<SelectListItem> Roles = new List<SelectListItem>();
            Roles = (from s in db.Roles select new SelectListItem { Text = s.Name, Value = s.Name }).ToList();
            ViewBag.Role = Roles;
            return View();
        }

        [HttpPost]
        public ActionResult CreateUsers(string Name, string Email, string Role,string Password)
        {
            try
            {
                UserRegistrationView model = new UserRegistrationView();
                model.Password = Password;
                var user = new ApplicationUser { UserName = Email, Email = Email };

                user.Name = Name;
                user.CreatedBy = User.Identity.GetUserId();
                user.CreatedOn = DateTime.Now;
                user.Status = 1;

                IdentityResult result = UserManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, Role);
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


            return RedirectToAction("CreateUsers", "Security");
        }
        public ActionResult editUsers(string id)
        {

            // To check user access right
            string uri1 = "/Security/Users";

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
            userlist sa = new userlist();
            sa = (from u in db.Users
                  from ur in u.Roles
                  join r in db.Roles on ur.RoleId equals r.Id
                  where u.Id == id
                  select new userlist
                  {
                      id = u.Id,
                      Name = u.Name,
                      email = u.Email,
                      role = r.Id,
                      status = u.Status
                  }).FirstOrDefault();

            List<SelectListItem> Roles = new List<SelectListItem>();
            Roles = (from s in db.Roles select new SelectListItem { Text = s.Name, Value = s.Id }).ToList();
            ViewBag.Role = Roles;
            return View(sa);
        }
        [HttpPost]
        public ActionResult editUsers(string Name, string id)
        {
            try
            {
                ApplicationUser au = new ApplicationUser();
                au = (from s in db.Users where s.Id == id select s).FirstOrDefault();
                au.Name = Name;
                au.ModifiedBy = User.Identity.GetUserId();
                au.ModifiedOn = DateTime.Now;
                db.Entry(au).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["message"] = "User Updated !!";
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error Occured While Saving !!" + ex.ToString();
            }
            return RedirectToAction("editUsers", "Security", new { id = id });
        }

        public ActionResult viewUsers(string id)
        {
            // To check user access right
            string uri1 = "/Security/Users";

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

            userlist sa = new userlist();
            sa = (from u in db.Users
                  from ur in u.Roles
                  join r in db.Roles on ur.RoleId equals r.Id
                  where u.Id == id
                  select new userlist
                  {
                      id = u.Id,
                      Name = u.Name,
                      email = u.Email,
                      role = r.Name,
                      status = u.Status
                  }).FirstOrDefault();
            return View(sa);
        }

        public ActionResult statusUsers(string id)
        {
            // To check user access right
            string uri1 = "/Security/Users";

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
            ApplicationUser au = new ApplicationUser();
            au = (from s in db.Users where s.Id == id select s).FirstOrDefault();
            if (au.Status == 1)
            {
                au.Status = 0;
                au.ModifiedBy = User.Identity.GetUserId();
                au.ModifiedOn = DateTime.Now;
                TempData["message"] = "User Deactivated !!";
            }
            else
            {
                au.Status = 1;
                au.ModifiedBy = User.Identity.GetUserId();
                au.ModifiedOn = DateTime.Now;
                TempData["message"] = "User Activated !!";
            }

            db.Entry(au).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Users", "Security");
        }

    }
}