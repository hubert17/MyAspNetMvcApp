using MyAspNetMvcApp.Models.Lessons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class BootstrapController : Controller
    {
        // GET: Bootstrap
        public ActionResult Index()
        {
            var s = new Song();
            return View(s);
        }

        [HttpPost]
        public ActionResult Index(Song s)
        {
            s.Artist = s.Artist.ToUpper();
            return View("index",s);
        }
        
    }
}