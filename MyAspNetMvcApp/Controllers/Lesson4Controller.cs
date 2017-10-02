using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class Lesson4Controller : Controller
    {
        // GET: Lesson4
        public ActionResult Index()
        {
            var phonebook = new Dictionary<string, string>();
            phonebook["bernard"] = "0999999999";
            phonebook["angel"] = "08888888888";
            phonebook["kulas"] = "0777777777";

            phonebook.Add("Bitok", "055555555555");

            ViewBag.phonebook = phonebook;

            return View();
        }

    }
}