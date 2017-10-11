using MyAspNetMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.MessageBox = "Hello! Welcome to " + AppSettings.AppTitle + "!";
            return View();

            // This is for www.bernardgabon.com. Kindly remove this and uncomment the code above.
            //return RedirectToAction("Index", "BernardGabonDotCom", new { area = "BernardGabonDotCom" });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}