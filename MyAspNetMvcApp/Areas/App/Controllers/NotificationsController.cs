using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.App.Controllers
{
    public class NotificationsController : Controller
    {
        // GET: App/Notifications
        public ActionResult Index()
        {
            return View();
        }
    }
}