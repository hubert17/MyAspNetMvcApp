using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;

namespace MyAspNetMvcApp.Controllers
{
    public class VinChatController : Controller
    {
        // GET: VinChat
        public ActionResult Index()
        {
            if (Session["username"] == null)
                ViewBag.username = string.Empty;
            else
                ViewBag.username = Session["username"].ToString();
            return View();
        }

        public ActionResult Login(string username)
        {

            Session["username"] = username;
            var cm = new ChatMessage();
            cm.Sender = Session["username"].ToString();
            cm.Message = " has joined";
            cm.SendTime = DateTime.Now;
            List<ChatMessage> messages;
            if(HttpRuntime.Cache["messages"] == null)
            {
                messages = new List<ChatMessage>();              
            }
            else
            {
                messages = HttpRuntime.Cache["messages"] as List<ChatMessage>;
            }

            messages.Add(cm);
            HttpRuntime.Cache["messages"] = messages;

            return RedirectToAction("Index");
        }

        public ActionResult Send(string txtmsg)
        {
            var cm = new ChatMessage();
            cm.Sender = Session["username"].ToString();
            cm.Message = txtmsg;
            cm.SendTime = DateTime.Now;

            var messages = HttpRuntime.Cache["messages"] as List<ChatMessage>;
            messages.Add(cm);
            HttpRuntime.Cache["messages"] = messages;

            return RedirectToAction("Index");

        }

        public ActionResult Messages()
        {
            ViewBag.Messages = HttpRuntime.Cache["messages"];

            return View();

        }

    }
}