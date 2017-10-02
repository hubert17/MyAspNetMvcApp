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
    [Authorize(Roles = @"admin")]
    public class RolesController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        //
        // GET: /Roles/
        public ActionResult Index(string Message = "")
        {
            var users = context.Users.OrderBy(o=>o.UserName).ToList();

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
                if (RoleName != "admin")
                {
                    var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    context.Roles.Remove(thisRole);
                    context.SaveChanges();
                }
            }
            catch
            {
                TempData["Message"] = RoleName + " cannot be deleted. It has users.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            if(!string.IsNullOrEmpty(RoleName))
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                userManager.AddToRole(user.Id, RoleName);

                // prepopulat roles for the view dropdown
                var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
            }

            TempData["UserName"] = UserName;
            return RedirectToAction("Index");
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
                var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
            }

            TempData["UserName"] = UserName;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            var account = new AccountController();
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (account.UserManager.IsInRole(user.Id, RoleName))
            {
                account.UserManager.RemoveFromRole(user.Id, RoleName);
            }

            // prepopulat roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            TempData["UserName"] = UserName;
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> LockUser(string UserName)
        {
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //await userManager.SetLockoutEnabledAsync(user.Id, true);
            //await userManager.SetLockoutEndDateAsync(user.Id, DateTime.Today.AddYears(10));

            user.UserProfile.IsActive = false;
            await userManager.UpdateAsync(user);

            TempData["Message"] = UserName + " has been deactivated.";

            TempData["UserName"] = UserName;
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> UnlockUser(string UserName)
        {
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //await userManager.SetLockoutEnabledAsync(user.Id, false);
            //await userManager.SetLockoutEndDateAsync(user.Id, DateTime.Now);

            user.UserProfile.IsActive = true;
            await userManager.UpdateAsync(user);

            TempData["Message"] = UserName + " has been reactivated.";

            TempData["UserName"] = UserName;
            return RedirectToAction("Index");
        }
    }

}