﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.Areas.App.Models;

namespace MyAspNetMvcApp.Areas.App.Controllers
{
 
    [AdminAuthorize]
    public class LookupController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: App/Lookup
        public ActionResult Index(bool showInactive = false, string filterType = "")
        {
            if(string.IsNullOrEmpty(filterType))
                ViewBag.lookups = showInactive ? db.Lookups.ToList() : db.Lookups.Where(x => x.IsActive == true).ToList();
            else
                ViewBag.lookups = showInactive ? db.Lookups.Where(x=>x.Type == filterType).ToList() : db.Lookups.Where(x => x.IsActive == true && x.Type == filterType).ToList();

            ViewBag.lookupTypes = db.Lookups.ToList().GroupBy(x => x.Type).Select(s => s.FirstOrDefault()).Select(l=>l.Type).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Lookup lookup)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    lookup.IsActive = true;
                    db.Lookups.Add(lookup);
                    db.SaveChanges();
                    return PartialView("~/Areas/App/Views/Lookup/_LookupItem.cshtml", lookup);
                }
                catch
                { }
            }
            return new HttpStatusCodeResult(400, "Oops! Something went wrong. Please properly fill up the form.");
        }

        public ActionResult Edit(int Id)
        {
            try
            {
                var lookup = db.Lookups.Find(Id);
                return PartialView("~/Areas/App/Views/Lookup/_LookupEdit.cshtml", lookup);
            }
            catch
            {
                return new HttpStatusCodeResult(400, "Oops! Something went wrong.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lookup lookup)
        {
            try
            {
                db.Entry(lookup).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return PartialView("~/Areas/App/Views/Lookup/_LookupItem.cshtml", lookup);
            }
            catch
            {
                return new HttpStatusCodeResult(400, "Oops! Something went wrong.");
            }
        }
    }
}