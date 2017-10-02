using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.Areas.App.Models;

namespace MyAspNetMvcApp.Areas.App.Controllers
{
    
    public class LookupController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: App/Lookup
        public ActionResult Index(bool showInactive = false)
        {
            ViewBag.lookups = showInactive ? db.Lookups.ToList() : db.Lookups.Where(x => x.InActive == false).ToList();
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}