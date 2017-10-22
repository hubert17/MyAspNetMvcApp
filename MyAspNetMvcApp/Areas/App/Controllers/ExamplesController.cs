using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Areas.App.Models;

namespace MyAspNetMvcApp.Areas.App.Controllers
{
    public class ExamplesController : Controller
    {
        // GET: App/Examples
        public ActionResult Index()
        {
            var examples = AppControllerAction.GetExamples();
            return PartialView(examples);
        }

    }
}