using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.Areas.Account.Models;
using System.Threading.Tasks;

namespace MyAspNetMvcApp.Areas.Account.Controllers
{
    // http://divineops.net/custom-mvc-role-authorize-attribute-using-app-settings/
    [AdminAuthorize]
    public class RolesController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        //
        // GET: /Roles/
        public ActionResult Index(string Message = "")
        {
            List<ApplicationUser> users;
            if (TempData["roleFilter"] == null)
                users = context.Users.OrderBy(o => o.UserName).ToList();
            else
            {
                var roleFilter = (string)TempData["roleFilter"];
                var roleId = context.Roles.Where(r => r.Name == roleFilter).First().Id;
                users = context.Users.Where(x => x.Roles.Any(r => r.RoleId == roleId)).OrderBy(o => o.UserName).ToList();
            }

            var userRoles = new Dictionary<string, List<string>>();

            foreach (var i in users)
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(i.UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var account = new AccountController();
                var RolesForThisUser = account.UserManager.GetRoles(user.Id).ToList();
                userRoles.Add(i.Id, RolesForThisUser);
                i.UserProfile = context.UserProfiles.Where(u => u.UserName.Equals(i.UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            }
           
            ViewBag.userRoles = userRoles;
            ViewBag.roles = context.Roles.ToList();
            ViewBag.Message = Message.Trim();
            
            return View(users);
        }

        public ActionResult FilterUsers(string RoleName)
        {
            TempData["roleFilter"] = RoleName;
            TempData[BSMessage.PANEL] = "Displaying users with " + RoleName + " role. Refresh to show all.";
            return RedirectToAction("Index");
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                context.SaveChanges();
            }
            catch
            {
            }

            return RedirectToAction("Index");

        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                if(role.Name != "admin")
                {
                    context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Roles/Delete/5
        public ActionResult Delete(string RoleName)
        {
            try
            {
                if (RoleName != System.Configuration.ConfigurationManager.AppSettings["AdminRolename"])
                {
                    var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    var users = context.Users.Where(x => x.Roles.Any(r => r.RoleId == thisRole.Id)).OrderBy(o => o.UserName).ToList();
                    if (users.Count > 0)
                    {
                        TempData["roleFilter"] = thisRole.Name;
                        TempData[BSMessage.PANEL] = RoleName + " cannot be deleted. This role has members.";
                    }
                    else
                    {
                        context.Roles.Remove(thisRole);
                        context.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                TempData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                TempData[BSMessage.PANEL] = "Oops! Something went wrong. " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            if (!string.IsNullOrEmpty(RoleName))
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                userManager.AddToRole(user.Id, RoleName);

                // prepopulat roles for the view dropdown
                //var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                //ViewBag.Roles = list;
            }

            //TempData["UserName"] = UserName;
            //return RedirectToAction("Index");

            ViewBag.username = UserName;
            ViewBag.rolename = RoleName;
            return PartialView("~/Areas/Account/Views/Roles/_DeleteRole.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                ViewBag.RolesForThisUser = userManager.GetRoles(user.Id);

                // prepopulat roles for the view dropdown
                //var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                //ViewBag.Roles = list;
            }

            TempData["UserName"] = UserName;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            if(System.Configuration.ConfigurationManager.AppSettings["AdminUsername"] == UserName)
            {
                //TempData[BSMessage.DIALOGBOX] = "Invalid action.";
                //return RedirectToAction("Index");
                return Json(false);
            }

            var account = new AccountController();
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (account.UserManager.IsInRole(user.Id, RoleName))
            {
                account.UserManager.RemoveFromRole(user.Id, RoleName);
            }

            // prepopulat roles for the view dropdown
            //var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            //ViewBag.Roles = list;

            //TempData["UserName"] = UserName;
            //return RedirectToAction("Index");

            return Json(true);
        }

        public async Task<ActionResult> LockUser(string UserName)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["AdminUsername"] == UserName)
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                //await userManager.SetLockoutEnabledAsync(user.Id, true);
                //await userManager.SetLockoutEndDateAsync(user.Id, DateTime.Today.AddYears(10));

                user.UserProfile.IsActive = false;
                user.LockoutEnabled = true;
                await userManager.UpdateAsync(user);

                //TempData[BSMessage.DIALOGBOX] = UserName + " has been deactivated.";

                //TempData["UserName"] = UserName;
                //return RedirectToAction("Index");

                return Json(new { result = true, action = "unlockuser", username = UserName }, JsonRequestBehavior.AllowGet);
            }
            catch { }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UnlockUser(string UserName)
        {
            try
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                //await userManager.SetLockoutEnabledAsync(user.Id, false);
                //await userManager.SetLockoutEndDateAsync(user.Id, DateTime.Now);

                user.UserProfile.IsActive = true;
                user.LockoutEnabled = false;
                await userManager.UpdateAsync(user);

                //TempData[BSMessage.DIALOGBOX] = UserName + " has been reactivated.";

                //TempData["UserName"] = UserName;
                //return RedirectToAction("Index");
                return Json(new { result = true, action = "lockuser", username = UserName }, JsonRequestBehavior.AllowGet);
            }
            catch
            { }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
    }

}

public class AdminAuthorizeAttribute : AuthorizeAttribute
{

    public AdminAuthorizeAttribute()
    {
        this.Roles = System.Configuration.ConfigurationManager.AppSettings["AdminRolename"];
    }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        return base.AuthorizeCore(httpContext);
    }
}