﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.Areas.Account.Models;
using MyAspNetMvcApp.Areas.Account.ViewModels;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace MyAspNetMvcApp.Areas.Account.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext())
                {
                    string myPhone = String.Join("", model.UserName.Split('(', '-', ')')).TrimStart('0');
                    ApplicationUser myUser = db.Users.FirstOrDefault(u => u.UserName == model.UserName || (u.PhoneNumber == myPhone ));
                    if (myUser != null)
                    {
                        model.UserName = myUser.UserName;
                    }
                }
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null && user.UserProfile.IsActive)
                {
                    await SignInAsync(user, model.RememberMe);
                    if(string.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("index", "home", new { area = "" });
                    else
                        return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
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
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) ? null : model.PhoneNumber.TrimStart('0'),
                    CountyCode =  string.IsNullOrEmpty(model.PhoneNumber) ? null : model.CountyCode,
                    UserProfile = new UserProfile
                    {
                        UserName = model.UserName,
                        RegistrationType = model.RegistrationType,
                        LastName = model.LastName,
                        FirstName = model.FirstName,
                        BirthDate = model.BirthDate,
                        Gender = model.Gender,
                        RegistrationDate = DateTime.Now,
                        IsActive = true
                    }
                };

                if(AppSettings.EmailVerificationEnabled)
                {
                    char[] padding = { '=' };
                    user.Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
                    user.TokenExpiration = DateTime.Now.AddHours(1);
                }

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    RegisterViewModel.SaveRegistrationCustomData(model);

                    string WelcomeMsg = "Hello " + model.FirstName + "! Welcome to " + AppSettings.AppTitle + ". "; ;
                    if(AppSettings.EmailVerificationEnabled)
                    {
                        var callbackUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = user.Token });
                        Gabs.Helpers.EmailUtil.SendEmail(user.Email,
                           "Confirm Your Account",
                           "Hello " + model.FirstName +"!<br><br> Please confirm your account by clicking this <a href=\"" + callbackUrl + "\">link</a>.");
                        WelcomeMsg += "Kindly check your email to verify your account.";
                    }

                    await SignInAsync(user, isPersistent: false);

                    if (!string.IsNullOrEmpty(model.PhoneNumber))
                    {
                        var smsMsg = "Hello " + model.FirstName + "! You can now login to " + AppSettings.AppTitle + " using your mobile number.";
                        Gabs.Helpers.SMSUtil.Send("+" + model.CountyCode + model.PhoneNumber, smsMsg);
                        WelcomeMsg += " We have also sent a welcome message to your mobile phone.";
                        //return RedirectToAction("VerifyPhoneNumber", "Account");                       
                    }

                    TempData["MessageBox"] = WelcomeMsg;
                    return RedirectToAction("Index", "Home", new { area = "", welcome = true });
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult FbSignIn()
        {
            string app_id = Convert.ToString(ConfigurationManager.AppSettings["Fb_App_ID"]);
            string app_secret = Convert.ToString(ConfigurationManager.AppSettings["Fb_App_Secret"]);
            string scope = Convert.ToString(ConfigurationManager.AppSettings["Fb_App_Scope"]);
            string RedirectUrl = Convert.ToString(ConfigurationManager.AppSettings["Fb_RedirectUrl"]);

            return Redirect(string.Format(
                    "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}",
                    app_id, RedirectUrl, scope));
        }

        [AllowAnonymous]
        public async Task<ActionResult> FbRedirectHandler()
        {
            try
            {
                string app_id = Convert.ToString(ConfigurationManager.AppSettings["Fb_App_ID"]);
                string app_secret = Convert.ToString(ConfigurationManager.AppSettings["Fb_App_Secret"]);
                string scope = Convert.ToString(ConfigurationManager.AppSettings["Fb_App_Scope"]);
                string AccessCode = Request["code"].ToString();
                string access_token = string.Empty;

                string url = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&scope={2}&code={3}&client_secret={4}",
                    app_id, Request.Url.AbsoluteUri, scope, AccessCode, app_secret);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string jsonResponse = reader.ReadToEnd();

                    JavaScriptSerializer sr = new JavaScriptSerializer();
                    string jsondata = jsonResponse;

                    dynamic DynamicData = JObject.Parse(jsondata);

                    access_token = DynamicData.access_token;
                }

                var user = GetDetails(access_token);
                if (user != null)
                    await SignInAsync(user, isPersistent: false);
                else
                {
                    TempData["MessageBox"] = "Sign in failed. Unable to retrieve Facebook email information.";
                    return RedirectToAction("Login");
                }
            }
            catch(Exception ex)
            {
                TempData["MessagePanel"] = ex.Message;
                return RedirectToAction("Login");
            }

            return RedirectToAction("Index", "Home", new { area = ""});
        }

        private ApplicationUser GetDetails(string AccessToken)
        {
            Uri eatTargetUri = new Uri("https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id=" + ConfigurationManager.AppSettings["Fb_App_ID"] + "&client_secret=" + ConfigurationManager.AppSettings["Fb_App_Secret"] + "&fb_exchange_token=" + AccessToken);
            HttpWebRequest eat = (HttpWebRequest)HttpWebRequest.Create(eatTargetUri);

            StreamReader eatStr = new StreamReader(eat.GetResponse().GetResponseStream());
            string eatToken = eatStr.ReadToEnd().ToString().Replace("access_token=", "");

            // Split the access token and expiration from the single string
            string[] eatWords = eatToken.Split('&');
            string extendedAccessToken = eatWords[0];

            // Request the Facebook user information
            Uri targetUserUri = new Uri("https://graph.facebook.com/me?fields=first_name,last_name,gender,email,birthday&access_token=" + AccessToken);
            HttpWebRequest user = (HttpWebRequest)HttpWebRequest.Create(targetUserUri);

            // Read the returned JSON object response
            StreamReader userInfo = new StreamReader(user.GetResponse().GetResponseStream());
            string jsonResponse = string.Empty;
            jsonResponse = userInfo.ReadToEnd();

            // Deserialize and convert the JSON object to the Facebook.User object type
            JavaScriptSerializer sr = new JavaScriptSerializer();
            string jsondata = jsonResponse;

            dynamic facebook = JObject.Parse(jsondata);

            //string firstName = facebook.Name;
            string FacebookId = facebook.Id;
            string FacebookEmail = facebook.email;
            /*You can get other dynamic variables*/

            if(string.IsNullOrEmpty(FacebookEmail))
                return null;
            else
            {
                using (var db = new ApplicationDbContext())
                {
                    ApplicationUser _user = UserManager.FindByName(FacebookEmail);
                    if (_user == null)
                    {
                        _user = new ApplicationUser()
                        {
                            UserName = FacebookEmail,
                            UserProfile = new UserProfile
                            {
                                UserName = FacebookEmail,
                                LastName = facebook.last_name,
                                FirstName = facebook.first_name,
                                BirthDate = facebook.birthday,
                                Gender = facebook.gender == "male" ? "M" : "F",
                                RegistrationDate = DateTime.Now,
                                IsActive = true
                            }
                        };
                        UserManager.CreateAsync(_user);
                    }

                    return _user;
                }

            }
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                TempData["MessageAlert"] = "An error has occured.";
                return RedirectToAction("index", "home", new { area = "" });
            }

            var user = UserManager.FindById(userId);            
            if (user != null && user.TokenExpiration > DateTime.Now && !string.IsNullOrEmpty(user.Token) && user.Token.Equals(code))
            {
                user.EmailConfirmed = true;
                user.Token = null;
                user.TokenExpiration = null;
                UserManager.Update(user);
                TempData["MessageBox"] = "We have successfully verified your email.";
                return RedirectToAction("Login", "Account");
            }

            TempData["MessagePanel"] = "Sorry. Your email confirmation token has expired.";
            return RedirectToAction("index", "home", new { area = "" });
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult VerifyPhoneNumber()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        //{

        //}

        // Check existing email/phone number
        [AllowAnonymous]
        public ActionResult CheckExistingEmail(string UserName)
        {
            bool ifExist = false;
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    ApplicationUser myUser = db.Users.FirstOrDefault(u => u.UserName == UserName);
                    if (myUser != null) ifExist = true;
                }
                return Json(!ifExist, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        public ActionResult CheckExistingPhoneNumber(string PhoneNumber)
        {
            bool ifExist = false;
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    PhoneNumber = PhoneNumber.TrimStart('0');
                    ApplicationUser myUser = db.Users.FirstOrDefault(u => u.PhoneNumber == PhoneNumber);
                    if (myUser != null) ifExist = true;
                }
                return Json(!ifExist, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ChangePasswordViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public string ForgotPassword(string id = "", string email = "")
        {
            var user = UserManager.FindByName(email);
            if (user != null && user.EmailConfirmed)
            {
                char[] padding = { '=' };
                user.Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
                user.TokenExpiration = DateTime.Now.AddHours(1);
                var result = UserManager.Update(user);
                if (result.Succeeded)
                {
                    var callbackUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ResetPassword", "Account", new { UserId = user.Id, code = user.Token });
                    string msg = "To reset your password, <a href=\"" + callbackUrl + "\">click here</a>.<br>";
                    msg += "If you have not requested a password change for your account, please ignore this email.";

                    Gabs.Helpers.EmailUtil.SendEmail(user.Email, "Password Reset", msg);

                    return "1";
                }
            }

            return "0";
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ResetPassword(string code, string UserId)
        {
            var user = await UserManager.FindByIdAsync(UserId);
            if (user != null && user.TokenExpiration > DateTime.Now && !string.IsNullOrEmpty(user.Token) && user.Token.Equals(code))
            {
                var rp = new ResetPasswordViewModel();
                rp.Email = user.Email;
                rp.Code = code;
                return View(rp);
            }
            else if (user != null)
            {
                user.Token = null;
                user.TokenExpiration = null;
                await UserManager.UpdateAsync(user);
            }

            TempData["MessagePanel"] = "Sorry. Your password reset token has expired.";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ApplicationUser user = await UserManager.FindByNameAsync(model.Email);
            if (user!=null && user.TokenExpiration > DateTime.Now)
            {
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.NewPassword);
                user.Token = null;
                user.TokenExpiration = null;
                var result = await UserManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    TempData["MessageAlert"] = "Password reset failed.";
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            TempData["MessageBox"] = "You may now login with your new password.";
            return RedirectToAction("Login", "Account");
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
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

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            user.UserProfile.LastLogin = DateTime.Now;
            await UserManager.UpdateAsync(user);

            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
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
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
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