using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class ChatmvcController : Controller
    {
        // GET: Chatmvc
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Messages()
        {
            return View();
        }

        public ActionResult Join(string username)
        {
            Session["username"] = username;
            return RedirectToAction("Index");
        }
    }
}