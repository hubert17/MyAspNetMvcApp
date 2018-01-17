using MyAspNetMvcApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.BernardGabonDotCom.Controllers
{
    public class StudentController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: BernardGabonDotCom/Student
        public ActionResult Index(string songId)
        {
            var student = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            if(string.IsNullOrEmpty(songId))
            {
                dynamic d = JObject.Parse(student.MetaData);
                student.MetaData = d.section;
            }

            return View(student);
        }
    }
}