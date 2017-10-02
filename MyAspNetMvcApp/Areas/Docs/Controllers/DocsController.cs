using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.Docs.Controllers
{
    public class DocsController : Controller
    {
        // GET: Docs/Docs
        public ActionResult Index()
        {
            return View();
        }
    }
}