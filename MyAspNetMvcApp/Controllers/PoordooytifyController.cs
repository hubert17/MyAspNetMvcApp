using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class PoordooytifyController : Controller
    {
        // GET: Poordooytify
        public ActionResult Index()
        {
            ViewBag.Redirect = "index";
            return View();
        }

        public ActionResult Upload()
        {
            ViewBag.Redirect = "upload";
            return View("Index");
        }

        public ActionResult Mood()
        {
            ViewBag.Redirect = "mood";
            return View("Index");
        }

        public ActionResult DropboxInvite()
        {
            ViewBag.Redirect = "dropbox";
            return View("Index");
        }
    }
}