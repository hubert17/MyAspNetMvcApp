using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;

namespace MyAspNetMvcApp.Controllers
{
    public class WorkingWithDataController : Controller
    {
        // GET: WorkingWithData
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            var projectlist = db.Projects.Select(s => new SelectListItem { Value=s.Id.ToString(), Text = s.Name }).ToList();


            ViewBag.projectDropdown = projectlist;

            return View();
        }

        public ActionResult Person()
        {
            return View();
        }
    }
}