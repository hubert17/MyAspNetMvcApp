using MyAspNetMvcApp.Areas.BernardGabonDotCom.Models;
using MyAspNetMvcApp.Areas.BernardGabonDotCom.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.BernardGabonDotCom.Controllers
{
    public class GabsProjectController : Controller
    {
        private _GabsDbContext db = new _GabsDbContext();

        public ActionResult Index()
        {
            return RedirectToAction("Submit");
        }

        public ActionResult Submit()
        {
            ViewBag.ProjectDropDown = ProjectViewModel.getSelectList();
            ViewBag.submissions = db.Submissions.OrderBy(x => x.ProjectId).ThenBy(y => y.SubmitDate).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Submit(Submission s)
        {
            if (!string.IsNullOrEmpty(s.LastName) && !string.IsNullOrEmpty(s.Link))
            {
                s.SubmitDate = DateTime.Now;
                db.Submissions.Add(s);
                db.SaveChanges();
            }
            return RedirectToAction("Submit");
        }

        [Authorize(Roles = @"admin")]
        public ActionResult CreateProject()
        {
            ViewBag.ProjectList = db.Projects.ToList();
            return View();
        }

        [Authorize(Roles = @"admin")]
        [HttpPost]
        public ActionResult CreateProject(Project p)
        {
            db.Projects.Add(p);
            db.SaveChanges();
            return RedirectToAction("CreateProject");
        }

    }
}


