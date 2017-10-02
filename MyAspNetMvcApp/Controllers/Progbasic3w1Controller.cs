using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class Progbasic3w1Controller : Controller
    {
        // GET: Progbasic3w1
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grade()
        {
            var name = Request.Form["Fullname"];
            var firstGrade = Double.Parse(Request.Form["FirstGrade"]);
            var secondGrade = Double.Parse(Request.Form["SecondGrade"]);
            var thirdGrade = Double.Parse(Request.Form["thirdGrade"]);

            double finalGrade = (firstGrade * 0.33) 
                + (secondGrade * 0.33) 
                + (thirdGrade * 0.34);

            ViewBag.name = name;
            ViewBag.finalGrade = finalGrade;

            return View();
        }

    }
}