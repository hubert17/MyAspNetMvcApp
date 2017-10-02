using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class MyController : Controller
    {
        // GET: My
        public string Index()
        {
            return "I'm the index";
        }

        public string theotherwoman()
        {
            return "Im the other woman";
        }

        public string Imyournumber1()
        {
            return "Im ur number 1";
        }
    }
}