using MyAspNetMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class DataShareController : Controller
    {
        // GET: DataShare
        public ActionResult Index()
        {
            ViewBag.usercount = TempData["userCount"];
            return View();
        }

        public ActionResult CreateUser(string username)
        { 
            Session["userId"] = username;
            //HttpRuntime.Cache["users"] = new List<UserProfile>();
            //var users = HttpRuntime.Cache["Users"];
            //HttpRuntime.Cache["usercount"] = 1;

            int userCount;
            if (HttpRuntime.Cache["UsersCount"] == null)
            {
                HttpRuntime.Cache["UsersCount"] = 1;
                userCount = 1;
            }
            else
            {
                userCount = (int)HttpRuntime.Cache["UsersCount"];
                userCount++;
                HttpRuntime.Cache["UsersCount"] = userCount;
            }
            TempData["userCount"] = userCount;


            //Session.Add("userId", username);

            return RedirectToAction("Index");
        }
    }
}