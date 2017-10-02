using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class Activity2Controller : Controller
    {
        // GET: Activity2
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Average()
        {
            var fil = double.Parse(Request.Form["fil"]);
            var eng = double.Parse(Request.Form["eng"]);
            var mat = double.Parse(Request.Form["mat"]);
            var sci = double.Parse(Request.Form["sci"]);

            double average = (fil + eng + mat + sci) / 4;
            ViewBag.fil = fil;
            ViewBag.eng = eng;
            ViewBag.mat = mat;
            ViewBag.sci = sci;
            ViewBag.average = average;

            return View();
        }
    }
}