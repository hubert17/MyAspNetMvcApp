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
        public ActionResult Index(string strRedirect)
        {
            //ViewData[BSMessage.DIALOGBOX] = "Hello! Welcome to " + AppSettings.AppTitle + "!";
            ViewBag.ConAct = MyAspNetMvcApp.Areas.App.Models.AppControllerAction.GetExamples();

            if (String.IsNullOrEmpty(strRedirect))
                return View();
            else
                return RedirectToAction("Index", strRedirect, new { area = "BernardGabonDotCom" });

            // This is for www.bernardgabon.com. Kindly remove this and uncomment the code above.
            //return RedirectToAction("Index", "BernardGabonDotCom", new { area = "BernardGabonDotCom" });
        }

        public ActionResult About()
        {
            var ViewName = "AboutMe"; // or "About"
            return View(ViewName);
        }

        public ActionResult Contact()
        {
            return View();
        }

    }
}