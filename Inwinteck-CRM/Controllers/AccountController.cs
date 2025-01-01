using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Text.RegularExpressions;
using Inwinteck_CRM.Models;
using CaptchaMvc.HtmlHelpers;

namespace Inwinteck_CRM.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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



        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string logintype)
        {
            try
            {
                bool rm = model.RememberMe == "on";

                if (!ModelState.IsValid)
                {
                    // Log model state errors
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        utlity.createlog($"Model Error: {error.ErrorMessage}");
                    }
                    return View(model);
                }

                if (logintype == "FMS")
                {
                    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, rm, shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            var user = UserManager.FindByEmail(model.Email);

                            if (user.Status == 1)
                            {
                                string msg = "Login" + " " + model.Email;
                                utlity.createlog(msg);

                                if (user.ChangePassword == 1)
                                {
                                    return RedirectToAction("ChangePasswordP", "Account");
                                }

                                if (UserManager.IsInRole(user.Id, "Admin"))
                                {
                                    return RedirectToLocal(returnUrl ?? Url.Action("IndexAdmin", "Dashboard"));
                                }
                                else if (UserManager.IsInRole(user.Id, "Help Desk Manager"))
                                {
                                    return RedirectToLocal(returnUrl ?? Url.Action("IndexAdmin", "Dashboard"));
                                }
                                else if (UserManager.IsInRole(user.Id, "Sr.Help Desk Manager"))
                                {
                                    return RedirectToLocal(returnUrl ?? Url.Action("IndexAdmin", "Dashboard"));
                                }
                                else if (UserManager.IsInRole(user.Id, "Quality"))
                                {
                                    return RedirectToLocal(returnUrl ?? Url.Action("IndexHelpDesk", "Dashboard"));
                                }
                                else if (UserManager.IsInRole(user.Id, "Source Support"))
                                {
                                    return RedirectToLocal(returnUrl ?? Url.Action("SourceSupportWelcome", "Chat"));
                                }
                                else if (UserManager.IsInRole(user.Id, "HR"))
                                {
                                    return RedirectToLocal(returnUrl ?? Url.Action("bossmessaging", "Chat"));
                                }
                                else if (UserManager.IsInRole(user.Id, "Field Engineer"))
                                {
                                    int fe = (from s in db.FE_Master_Personal where s.Email == user.Email select s.Id).FirstOrDefault();
                                    return RedirectToLocal(returnUrl ?? Url.Action("FEProfile", "Master", new { id = UrlEncryptionHelper.Encrypt(fe) }));
                                }

                                else
                                {
                                    return RedirectToLocal(returnUrl ?? Url.Action("Index", "Dashboard"));
                                }
                            }
                            else
                            {
                                string msg = "Login" + " " + model.Email + " " + "Your status is deactive, Contact admin.";
                                utlity.createlog(msg);
                                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                                ModelState.AddModelError("", "Your status is deactive, Contact admin.");
                                return View(model);
                            }

                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }
                }
                else if (logintype == "HRMS")
                {
                    int cnt = (from s in db.HRMS_Users where s.Username == model.Email && s.Password == model.Password select s).Count();
                    if (cnt > 0)
                    {
                        var empuser = (from s in db.HRMS_Users where s.Username == model.Email && s.Password == model.Password select s).FirstOrDefault();
                        return RedirectToLocal(returnUrl ?? Url.Action("Index", "HRMS", new { emp = empuser.Emp_Id }));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                utlity.createlog(msg);
            }

            return View(model);
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig 
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //   await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                string body = utlity.ResetPassword(user.Name, callbackUrl);
                utlity.sendmail(user.Email, "Reset Password", body, "Reset Password<support@inwinteck.com>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string Password)
        {
            try
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                UserManager.RemovePassword(User.Identity.GetUserId());

                IdentityResult result = UserManager.AddPassword(User.Identity.GetUserId(), Password);
                if (result.Succeeded)
                {

                    TempData["message"] = "Your Password has been Successfully Changed.";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        TempData["message"] = error;
                    }
                }

            }
            catch (Exception Ex)
            {
                TempData["message"] = "Error Occured while Saving!!" + Ex.Message;

            }
            return RedirectToAction("ChangePassword", "Account");
        }

        public ActionResult ChangePasswordP()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.FE = (from s in db.FE_Master_Personal where s.Email == user.Email select s.Id).FirstOrDefault();
            if (user.ChangePassword == 1)
            {
                ViewBag.CP = "yes";
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePasswordP(string Password)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.FE = (from s in db.FE_Master_Personal where s.Email == user.Email select s.Id).FirstOrDefault();
            ViewBag.Contract = 0;
            try
            {

                UserManager.RemovePassword(User.Identity.GetUserId());
                IdentityResult result = UserManager.AddPassword(User.Identity.GetUserId(), Password);
                if (result.Succeeded)
                {
                    ApplicationUser au = new ApplicationUser();
                    au = (from s in db.Users where s.Id == user.Id select s).FirstOrDefault();
                    au.ChangePassword = 0;
                    au.ModifiedBy = User.Identity.GetUserId();
                    au.ModifiedOn = DateTime.Now;
                    db.Entry(au).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = "Password has been changed successfully. Kindly proceed to activate your account.";
                    var fe = (from s in db.FE_Master_Personal where s.Email == user.Email select s).FirstOrDefault();
                    string body = utlity.ChangePassword(fe.First_Name + " " + fe.Last_Name);

                    utlity.sendmailAcc(user.Email, "Password Changed", body, "Password Changed<support@inwinteck.com>");

                    //   string bodyp = utlity.ChangePasswordS(fe.First_Name + " " + fe.Last_Name, fe.Email);
                    //   utlity.sendmail("partner@inwinteck.com", "User Password Changed", bodyp, "Password Changed<support@inwinteck.com>");

                    return RedirectToAction("ChangePasswordP", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        TempData["message"] = error;
                    }
                }




            }
            catch (Exception Ex)
            {
                TempData["message"] = "Error Occured while Saving!!" + Ex.InnerException.ToString();

            }
            return View();
        }

        //[AllowAnonymous]
        //public ActionResult Inwin_Engg(string id)
        //{
         
        //    ViewBag.CreatedBy = id;

        //    ViewBag.Chat_mode = (from c in db.HeaderDescription where c.header_id == 3 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
        //    ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).OrderBy(x => x.Text).ToList();
        //    ViewBag.DialCode = (from c in db.Country_Dialing_Code where c.Status == 1 select new SelectListItem { Text = c.Code + " " + c.Country, Value = c.Code.Trim() }).ToList();
        //    ViewBag.Citizen = (from c in db.HeaderDescription where c.header_id == 8 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
        //    ViewBag.FE_Type = (from c in db.HeaderDescription where c.header_id == 9 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
        //    ViewBag.Message = "Yes";
        //    return View();
        //}

        [AllowAnonymous]
        public ActionResult Inwin_Engg2(string id)
        {
         
            ViewBag.CreatedBy = id;

            ViewBag.Chat_mode = (from c in db.HeaderDescription where c.header_id == 3 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Country = (from c in db.Country where c.Status == 1 select new SelectListItem { Text = c.CountryName, Value = c.CountryName.Trim() }).OrderBy(x => x.Text).ToList();
            ViewBag.DialCode = (from c in db.Country_Dialing_Code where c.Status == 1 select new SelectListItem { Text = c.Code + " " + c.Country, Value = c.Code.Trim() }).ToList();
            ViewBag.Citizen = (from c in db.HeaderDescription where c.header_id == 8 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.FE_Type = (from c in db.HeaderDescription where c.header_id == 9 && c.Status == 1 select new SelectListItem { Text = c.header_description, Value = SqlFunctions.StringConvert((double)c.id).Trim() }).ToList();
            ViewBag.Message = "Yes";
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult Inwin_Engg2(FE_Master_Personal sa, string Others, string Storage, string Networking, string Server, string Laptop, string Desktop)
        {
            try
            {
                int cnt = db.FE_Master_Personal
                              .Count(s => s.Email == sa.Email || s.Phone_Number == sa.Phone_Number);

                if (cnt > 0)
                {
                    TempData["message"] = "Your Email or Phone number is already registered. Please contact partner1@inwinteck.com or login at fms.inwinteck.com";
                    return RedirectToAction("FEAlreadyRegisterd", "Account", new { email = sa.Email , number = sa.Phone_Number});
                }

                // Generate FE ID and set other fields
                sa.InwinFEID = utlity.FEInwinteckId(sa.Country);
                sa.CreatedBy = sa.CreatedBy ?? User.Identity.GetUserId();
                sa.CreatedOn = DateTime.Now;
                sa.Status = 0;

                // Save FE_Master_Personal to the database
                db.FE_Master_Personal.Add(sa);
                db.SaveChanges();

                // Create User for FE
                string password = Membership.GeneratePassword(8, 1); 
                var user = new ApplicationUser
                {
                    UserName = sa.Email,
                    Email = sa.Email,
                    Name = sa.First_Name + " " + sa.Last_Name,
                    CreatedBy = sa.CreatedBy,
                    CreatedOn = DateTime.Now,
                    ChangePassword = 1,
                    Status = 1
                };

                IdentityResult result = UserManager.Create(user, password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Field Engineer");
                    // Prepare email body and send
                    string fe_type = sa.FE_Type == 345
                        ? $"<b class='font-montserrat color-dark'>Company</b>.<br/> Company Name : <b class='font-montserrat color-dark'>{sa.Company_Name}</b>"
                        : "<b class='font-montserrat color-dark'>Freelancer</b>";

                    string body = utlity.WelcomeFE(sa.First_Name + " " + sa.Last_Name, password, sa.Email, fe_type, sa.City, sa.Country);
                    utlity.sendmailAcc(sa.Email, "Welcome Partner", body, "Register <support@inwinteck.com>");
                    return RedirectToAction("FEWelcomePage", "Account", new { email = sa.Email });
                }
                else
                {
                    TempData["message"] = string.Join(" ", result.Errors);
                    TempData["registrationSuccess"] = false;
                    return View(sa);
                }

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult FEWelcomePage(string email)
        {
            ViewBag.Email = email;
            return View();
        } 
        public ActionResult FEAlreadyRegisterd(string email , string number)
        {
            ViewBag.Email = email;
            ViewBag.PhoneNumber = number;
            return View();
        }


        //[HttpPost]
        //[ValidateInput(false)]
        //[AllowAnonymous]
        //public ActionResult Inwin_Engg(FE_Master_Personal sa, string Others, string Storage, string Networking, string Server, string Laptop, string Desktop)
        //{
        //    try
        //    {
        //        if (this.IsCaptchaValid("Captcha is not valid"))
        //        {

        //            int cnt = (from s in db.FE_Master_Personal where s.Email == sa.Email select s).Count();
        //            int cnt1 = (from s in db.FE_Master_Personal where s.Phone_Number == sa.Phone_Number select s).Count();
        //            if (cnt > 0)
        //            {
        //                TempData["message"] = "Your Email Id is already Register with us.For any query please reach us at partner1@inwinteck.com !!";
        //                TempData["error"] = "Yes";
        //                TempData["Popup"] = "No";
        //                return RedirectToAction("Inwin_Engg", "Account");

        //            }
        //            else if (cnt1 > 0)
        //            {
        //                TempData["message"] = "Your Phone number is already Register with us.For any query please reach us at partner1@inwinteck.com !!";
        //                TempData["error"] = "Yes";
        //                TempData["Popup"] = "No";
        //                return RedirectToAction("Inwin_Engg", "Account");

        //            }

        //            sa.InwinFEID = utlity.FEInwinteckId(sa.Country);

        //            if (sa.CreatedBy == null)
        //            {
        //                sa.CreatedBy = User.Identity.GetUserId();

        //            }
        //            sa.CreatedOn = DateTime.Now;
        //            db.FE_Master_Personal.Add(sa);
        //            db.SaveChanges();

                   
        //            string password = Membership.GeneratePassword(8, 1);
        //            try
        //            {
        //                UserRegistrationView model = new UserRegistrationView();
        //                model.Password = password;
        //                var user = new ApplicationUser { UserName = sa.Email, Email = sa.Email };

        //                user.Name = sa.First_Name + " " + sa.Last_Name;
        //                user.CreatedBy = User.Identity.GetUserId();
        //                user.CreatedOn = DateTime.Now;
        //                user.ChangePassword = 1;
        //                user.Status = 1;
                       
        //                IdentityResult result = UserManager.Create(user, model.Password);
        //                if (result.Succeeded)
        //                {
        //                    UserManager.AddToRole(user.Id, "Field Engineer");
        //                    ModelState.Clear();
        //                    TempData["message"] = "User Created !!";
        //                }
        //                else
        //                {
        //                    foreach (var error in result.Errors)
        //                    {
        //                        TempData["message"] = error;
        //                    }
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                TempData["message"] = "Error Occured While Saving !!" + ex.ToString();
        //            }

        //            string fe_type;
        //            if (sa.FE_Type == 345)
        //            {
        //                fe_type = "<b class='font-montserrat color-dark'>Company</b>.<br/> Company Name : <b class='font-montserrat color-dark'>" + sa.Company_Name + "</b>";
        //            }
        //            else
        //            {
        //                fe_type = "<b class='font-montserrat color-dark'>Freelancer</b>";
        //            }
        //            string body = utlity.WelcomeFE(sa.First_Name + " " + sa.Last_Name, password, sa.Email, fe_type, sa.City, sa.Country);
        //            utlity.sendmailAcc(sa.Email, "Welcome Partner", body, "Register <support@inwinteck.com>");

        //            TempData["error"] = "No";
        //            TempData["login"] = sa.Email;
        //            TempData["message"] = "Thank you for initiating the registration.  Password has been sent to your email id.  Kindly login for completing the registration ";
        //            TempData["Popup"] = "Yes";
        //        }
        //        ViewBag.ErrMessage = "Error: captcha is not valid.";
        //        ViewBag.Message = "Yes";
        //        return RedirectToAction("Inwin_Engg", "Account");

        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["error"] = "Yes";
        //        TempData["message"] = "FE Not Created !! ";
        //    }
        //    return View();
        //}


        ////[HttpPost]
        //public async Task<ActionResult> Created_By(LoginViewModel model, FE_Master_Personal sa   )
        //{
        //    var Email = model.Email;
        //    try {
        //        string loggedInUserEmail = model.Email; // Get the logged-in user's email

        //        // Retrieve data records created by the logged-in user
        //        var userRecords = db.FE_Master_Personal
        //            .Where(record => record.Email == loggedInUserEmail)
        //            .ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["error"] = "Yes";
        //        TempData["message"] = "You are not created any Login !! ";
        //    }


        //}



        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}