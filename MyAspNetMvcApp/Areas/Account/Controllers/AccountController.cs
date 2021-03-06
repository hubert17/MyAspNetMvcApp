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
using System.Security.Principal;
using System.Collections.Generic;
using SimpleExcelImport;

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

            UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, bool allowGuest = false)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.AllowGuest = allowGuest;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        string myPhone = String.Join("", model.UserName.Split('(', '-', ')')).TrimStart('0');
                        ApplicationUser myUser = db.Users.FirstOrDefault(u => (u.UserName == model.UserName || u.Email == model.UserName) || u.PhoneNumber == myPhone);
                        if (myUser != null)
                        {
                            model.UserName = myUser.UserName;
                        }
                    }
                    var user = await UserManager.FindAsync(model.UserName, model.Password);
                    if (user != null && (user.UserProfile.IsActive || user.LockoutEnabled == false))
                    {
                        await SignInAsync(user, model.RememberMe);
                        if (string.IsNullOrEmpty(returnUrl))
                            return RedirectToAction("index", "home", new { area = "", redirectindexto = true });
                        else
                            return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
            }
            catch(Exception ex)
            {
                ViewData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                ViewData[BSMessage.DIALOGBOX] = "Database connection string is not properly configured or invalid. " + ex.Message;
            }
            // If we got this far, something failed, redisplay form
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string RegType)
        {
            ViewBag.RegType = RegType;
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
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    Email = model.UserName,
                    PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) ? null : model.PhoneNumber.TrimStart('0'),
                    CountryCode = string.IsNullOrEmpty(model.PhoneNumber) ? null : model.CountryCode,
                    UserProfile = new UserProfile
                    {
                        UserName = model.UserName,
                        RegistrationType = model.RegistrationType,
                        LastName = textInfo.ToTitleCase(model.LastName.ToLower()),
                        FirstName = textInfo.ToTitleCase(model.FirstName.ToLower()),
                        BirthDate = model.BirthDate,
                        Gender = model.Gender,
                        RegistrationDate = DateTime.Now,
                        IsActive = true
                    }
                };

                if (AppSettings.EmailVerificationEnabled)
                {
                    char[] padding = { '=' };
                    user.Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
                    user.TokenExpiration = DateTime.Now.AddHours(1);
                }

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string WelcomeMsg = "Hello " + model.FirstName + "! Welcome to " + AppSettings.AppTitle + ". ";

                    RegisterViewModel.SaveRegistrationCustomData(model);

                    if (!string.IsNullOrEmpty(model.RegistrationType))
                    {
                        string InitRole = RegisterViewModel.AddRole(model.UserName, model.RegistrationType);
                        if (!string.IsNullOrEmpty(InitRole))
                            WelcomeMsg += "The webapp initially assigns your role as a/n " + InitRole + ". ";
                    }

                    if (AppSettings.EmailVerificationEnabled)
                    {
                        var callbackUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = user.Token });
                        Gabs.Helpers.EmailUtil.SendEmail(user.Email,
                           "Confirm Your Account",
                           "Hello " + model.FirstName + "!<br><br> Please confirm your account by clicking this <a href=\"" + callbackUrl + "\">link</a>.");
                        WelcomeMsg += "Kindly check your email to verify your account. ";
                    }

                    await SignInAsync(user, isPersistent: false);

                    if (!string.IsNullOrEmpty(model.PhoneNumber))
                    {
                        var smsMsg = "Hello " + model.FirstName + "! You can now login to " + AppSettings.AppTitle + " using your mobile number.";
                        Gabs.Helpers.SMSUtil.Send("+" + model.CountryCode + model.PhoneNumber, smsMsg);
                        WelcomeMsg += " We have sent a welcome message to your mobile phone. ";
                        //return RedirectToAction("VerifyPhoneNumber", "Account");                       
                    }

                    TempData[BSMessage.DIALOGBOX] = WelcomeMsg;
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

        public async Task<ActionResult> ImportUsers(HttpPostedFileBase ExcelFile, string temppass)
        {
            var message = string.Empty;
            try
            {
                var output = new List<RegisterExcelViewModel>();
                if (ExcelFile != null)
                {
                    var data = ExcelFile.ToFileByteArray();
                    ImportFromExcel import = new ImportFromExcel();
                    if (Path.GetExtension(ExcelFile.FileName.ToLower()) == ".xlsx")
                        import.LoadXlsx(data);
                    else if (Path.GetExtension(ExcelFile.FileName.ToLower()) == ".xls")
                        import.LoadXls(data);
                    else
                    {
                        TempData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                        TempData[BSMessage.DIALOGBOX] = "Invalid Excel worksheet: " + Path.GetExtension(ExcelFile.FileName);
                        return RedirectToAction("Index", "Roles", new { area = "" });
                    }

                    output = import.ExcelToList<RegisterExcelViewModel>(0, 1);
                    if(output.Count == 0)
                    {
                        TempData[BSMessage.TYPE] = BSMessage.MessageType.WARNING;
                        TempData[BSMessage.DIALOGBOX] = "Excel worksheet has no user records.";
                        return RedirectToAction("Index", "Roles", new { area = "" });
                    }

                    var duplicates = output.GroupBy(x => x.UserName)
                                .Select(g => new { Value = g.Key, Count = g.Count() })
                                .Where(h => h.Count > 1)
                                .Select(s => s.Value);

                    if (duplicates.Count() > 0)
                    {
                        TempData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                        message = "<p>We have found the following duplicate records: " + string.Join(", ", duplicates) + " </p>";
                    }

                    //var users = output.GroupBy(x => x.UserName).Select(x => x.First()).ToList();

                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                    foreach (var s in output)
                    {
                        s.PhoneNumber = string.IsNullOrEmpty(s.PhoneNumber) ? null : s.PhoneNumber.TrimStart('0');
                        using (var db = new ApplicationDbContext())
                        {
                            var phoneExist = db.Users.Where(x => (x.CountryCode + x.PhoneNumber) == (s.CountryCode + s.PhoneNumber)).FirstOrDefault() != null;
                            var emailExist = db.Users.Where(u => u.UserName == s.Email || u.Email == s.Email).FirstOrDefault() != null;
                            if (emailExist || phoneExist)
                            {
                                s.CountryCode = null;
                                s.PhoneNumber = null;
                                s.Email = null;
                            }
                        }

                        DateTime? bdate;
                        try
                        {
                            bdate = DateTime.ParseExact(s.BirthDate, "M/d/yyyy", new CultureInfo("en-US"));
                        }
                        catch
                        {
                            bdate = null;
                        }
                        var user = new ApplicationUser()
                        {
                            UserName = s.UserName,
                            Email = s.Email,
                            PhoneNumber = s.PhoneNumber,
                            CountryCode = string.IsNullOrEmpty(s.PhoneNumber) ? null : s.CountryCode,
                            UserProfile = new UserProfile
                            {
                                UserName = s.UserName,
                                RegistrationType = s.RegistrationType,
                                LastName = textInfo.ToTitleCase(s.LastName.ToLower()),
                                FirstName = textInfo.ToTitleCase(s.FirstName.ToLower()),
                                BirthDate = bdate,
                                Gender = string.IsNullOrEmpty(s.Gender) ? null : s.Gender[0].ToString().ToUpper(),
                                RegistrationDate = DateTime.Now,
                                IsActive = true
                            }
                        };
                        var result = await UserManager.CreateAsync(user, string.IsNullOrEmpty(temppass) ? user.UserName : temppass);
                        if (result.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(user.UserProfile.RegistrationType))
                            {
                                RegisterViewModel.AddRole(user.UserName, user.UserProfile.RegistrationType);
                            }
                        }
                        else
                            message += user.UserName + " / " + user.UserProfile.LastName + ", " + user.UserProfile.FirstName + " was not added. <br>";
                    }


                }
            }
            catch(Exception ex)
            {
                TempData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                TempData[BSMessage.PANEL] = "Oops! Something went wrong. " + ex.GetBaseException();
            }

            if (!string.IsNullOrEmpty(message))
                TempData[BSMessage.PANEL] = message;
            return RedirectToAction("Index", "Roles", new { area = "" });
        }

        public ActionResult DownloadTemplate()
        {
            return File("~/Areas/Account/Views/Account/BulkImportExcelTemplate.xls", "application/vnd.ms-excel", "ImportUsersFromExcel_" + AppSettings.AppTitle + ".xls");
        }


        [AllowAnonymous]
        public ActionResult FbSignIn(string RegType)
        {
            string app_id = ConfigurationManager.AppSettings["Fb_App_ID"].ToString();
            string app_secret = ConfigurationManager.AppSettings["Fb_App_Secret"].ToString();
            string scope = ConfigurationManager.AppSettings["Fb_App_Scope"].ToString();
            string RedirectUrl = ConfigurationManager.AppSettings["Fb_RedirectUrl"].ToString();

            Session["RegType"] = RegType;

            return Redirect(string.Format(
                    "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}",
                    app_id, RedirectUrl, scope));
        }

        [AllowAnonymous]
        public async Task<ActionResult> FbRedirectHandler()
        {
            try
            {
                string app_id = ConfigurationManager.AppSettings["Fb_App_ID"].ToString();
                string app_secret = ConfigurationManager.AppSettings["Fb_App_Secret"].ToString();
                string scope = ConfigurationManager.AppSettings["Fb_App_Scope"].ToString();
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
                {
                    if(user.UserProfile.IsActive == false || user.LockoutEnabled == true)
                    {
                        TempData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                        TempData[BSMessage.DIALOGBOX] = "Sign in failed. Contact the administrator.";
                    }
                    else
                        await SignInAsync(user, isPersistent: false);
                }
                else
                {
                    TempData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                    TempData[BSMessage.DIALOGBOX] = "Sign in failed. Unable to retrieve Facebook email information.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                TempData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                TempData[BSMessage.PANEL] = ex.Message;
                return RedirectToAction("Login");
            }

            return RedirectToAction("Index", "Home", new { area = "" });
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
            Uri targetUserUri = new Uri("https://graph.facebook.com/me?fields=email,first_name,last_name,gender,picture.width(300),birthday&access_token=" + AccessToken);
            HttpWebRequest user = (HttpWebRequest)HttpWebRequest.Create(targetUserUri);

            // Read the returned JSON object response
            StreamReader userInfo = new StreamReader(user.GetResponse().GetResponseStream());
            string jsonResponse = string.Empty;
            jsonResponse = userInfo.ReadToEnd();

            // Deserialize and convert the JSON object to the Facebook.User object type
            JavaScriptSerializer sr = new JavaScriptSerializer();
            string jsondata = jsonResponse;

            dynamic facebook = JObject.Parse(jsondata);

            //string FacebookId = facebook.id;
            string FacebookEmail = facebook.email;
            /*You can get other dynamic variables*/

            if (string.IsNullOrEmpty(FacebookEmail))
                return null;
            else
            {
                using (var db = new ApplicationDbContext())
                {
                    ApplicationUser _user = UserManager.FindByEmail(FacebookEmail);
                    if (_user == null)
                    {
                        byte[] fbPhotoData;
                        try
                        {
                            WebClient webClient = new WebClient();
                            string fbPhotoUrl = facebook.picture.data.url;
                            fbPhotoData = webClient.DownloadData(fbPhotoUrl);
                        }
                        catch
                        {
                            fbPhotoData = null;
                        }
                        var newUser = new ApplicationUser()
                        {
                            UserName = FacebookEmail,
                            Email = FacebookEmail,
                            EmailConfirmed = true,
                            //PhoneNumber = ,
                            UserProfile = new UserProfile
                            {
                                UserName = FacebookEmail,
                                LastName = facebook.last_name,
                                FirstName = facebook.first_name,
                                BirthDate = facebook.birthday,
                                Gender = facebook.gender == "male" ? "M" : facebook.gender == "female" ? "F" : null,
                                RegistrationType = Session["RegType"] != null ?  Session["RegType"].ToString() : null,
                                RegistrationDate = DateTime.Now,
                                IsActive = true,
                                Picture = fbPhotoData
                            }
                        };
                        UserManager.Create(newUser);

                        string WelcomeMsg = "Hello " + newUser.UserProfile.FirstName + "! Welcome to " + AppSettings.AppTitle + ". ";
                        if (!string.IsNullOrEmpty(newUser.UserProfile.RegistrationType))
                        {
                            string InitRole = RegisterViewModel.AddRole(newUser.UserName, newUser.UserProfile.RegistrationType);
                            if (!string.IsNullOrEmpty(InitRole))
                                WelcomeMsg += "The webapp initially assigns your role as a/n " + InitRole + ". ";
                        }
                        Session.Remove("RegType");

                        TempData[BSMessage.DIALOGBOX] = WelcomeMsg;
                        return newUser;
                    }

                    return _user;
                }

            }
        }

        [AllowAnonymous]
        public string VerifyEmail()
        {
            if (AppSettings.EmailVerificationEnabled)
            {
                var user = UserManager.FindByName(User.Identity.Name);
                if(!user.EmailConfirmed)
                {
                    char[] padding = { '=' };
                    user.Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
                    user.TokenExpiration = DateTime.Now.AddHours(1);
                    UserManager.Update(user);

                    var callbackUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = user.Token });
                    Gabs.Helpers.EmailUtil.SendEmail(user.Email,
                       "Confirm Your Account",
                       "Hello " + user.UserProfile.FirstName + "!<br><br> Please confirm your account by clicking this <a href=\"" + callbackUrl + "\">link</a>.");

                    return "1";
                }
            }

            return "0";
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                TempData[BSMessage.ALERT] = "An error has occured.";
                return RedirectToAction("index", "home", new { area = "" });
            }

            var user = UserManager.FindById(userId);
            if (user != null && user.TokenExpiration > DateTime.Now && !string.IsNullOrEmpty(user.Token) && user.Token.Equals(code))
            {
                user.EmailConfirmed = true;
                user.Token = null;
                user.TokenExpiration = null;
                UserManager.Update(user);
                TempData[BSMessage.DIALOGBOX] = "We have successfully verified your email.";
                return RedirectToAction("Login", "Account");
            }
            TempData[BSMessage.TYPE] = BSMessage.MessageType.WARNING;
            TempData[BSMessage.PANEL] = "Sorry. Your email confirmation token has expired.";
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
                    ApplicationUser myUser = db.Users.FirstOrDefault(u => u.UserName == UserName || u.Email == UserName);
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
            ViewData[BSMessage.PANEL] =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : null;
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");

            using (var db = new ApplicationDbContext())
            {
                ApplicationUser user = UserManager.FindByName(User.Identity.GetUserName());
                ViewData["profile"] = new RegisterViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    CountryCode = user.CountryCode,
                    RegistrationType = string.Join(", ", UserManager.GetRoles(user.Id)),
                    LastName = user.UserProfile.LastName,
                    FirstName = user.UserProfile.FirstName,
                    BirthDate = user.UserProfile.BirthDate,
                    Gender = user.UserProfile.Gender,
                    Picture = user.UserProfile.Picture
                };
                ViewBag.Verified = user.EmailConfirmed;
                return View();
            }

        }

        public ActionResult EditProfile()
        {
            using (var db = new ApplicationDbContext())
            {
                var user = UserManager.FindByName(User.Identity.Name);
                var profile = new RegisterViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    CountryCode = user.CountryCode,
                    LastName = user.UserProfile.LastName,
                    FirstName = user.UserProfile.FirstName,
                    Gender = user.UserProfile.Gender,
                    BirthDate = user.UserProfile.BirthDate,
                    Picture = user.UserProfile.Picture,
                };

                return View(profile);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(RegisterViewModel profile, HttpPostedFileBase FileUpload, bool deletePic = false)
        {
            var db = new ApplicationDbContext();
            var phoneExist = db.Users.Where(x => x.PhoneNumber == profile.PhoneNumber).FirstOrDefault() != null;
            var emailExist = db.Users.Where(u => u.UserName == profile.Email || u.Email == profile.Email).FirstOrDefault() != null;

            var user = UserManager.FindByName(User.Identity.Name);

            user.UserProfile.LastName = profile.LastName;
            user.UserProfile.FirstName = profile.FirstName;
            user.UserProfile.Gender = profile.Gender;
            user.UserProfile.BirthDate = profile.BirthDate;
            if (FileUpload != null)
                user.UserProfile.Picture = FileUpload.ToImageByteArray(300);
            else if (deletePic)
                user.UserProfile.Picture = null;

            if(!emailExist && user.Email != user.UserName && !User.IsInRole(System.Configuration.ConfigurationManager.AppSettings["AdminRolename"]))
                user.Email = profile.Email;

            if (!phoneExist)
            {
                user.PhoneNumber = string.IsNullOrEmpty(profile.PhoneNumber) ? null : profile.PhoneNumber.TrimStart('0');
                user.CountryCode = profile.CountryCode;
            }

            UserManager.Update(user);

            TempData[BSMessage.PANEL] = "Your account has been successfully updated. ";
            return RedirectToAction("Manage");

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
            //return View(model);
            TempData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
            TempData[BSMessage.PANEL] = "ops! Something went wrong.";
            return RedirectToAction("Manage");
        }

        [AllowAnonymous]
        public string ForgotPassword(string id = "", string email = "")
        {
            var user = UserManager.FindByEmail(email);
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

            TempData[BSMessage.TYPE] = BSMessage.MessageType.WARNING;
            TempData[BSMessage.PANEL] = "Sorry. Your password reset token has expired.";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);
            if (user != null && user.TokenExpiration > DateTime.Now)
            {
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.NewPassword);
                user.Token = null;
                user.TokenExpiration = null;
                var result = await UserManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    TempData[BSMessage.ALERT] = "Password reset failed.";
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            TempData[BSMessage.DIALOGBOX] = "You may now login with your new password.";
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

            identity.AddClaim(new Claim("FirstName", user.UserProfile.FirstName));

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

public static class GenericPrincipalExtensions
{
    public static string FirstName(this IPrincipal user)
    {
        if (user.Identity.IsAuthenticated)
        {
            ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
            foreach (var claim in claimsIdentity.Claims)
            {
                if (claim.Type == "FirstName")
                    return claim.Value;
            }
            return "";
        }
        else
            return "";
    }
}