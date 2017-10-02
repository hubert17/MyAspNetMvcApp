using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;

namespace MyAspNetMvcApp.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Send(Email mail, HttpPostedFileBase Attachment)
        {
            var attachment = Gabs.Helpers.EmailUtil.FileToAttachment(Attachment);
            TempData["MessagePanel"] = Gabs.Helpers.EmailUtil.SendEmail(mail.MailTo, mail.Subject, mail.Message, attachment);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SendSMS(Email mail)
        {            
            TempData["MessagePanel"] = Gabs.Helpers.SMSUtil.Send(mail.MailTo, mail.Message);

            return RedirectToAction("Index");
        }

    }
}