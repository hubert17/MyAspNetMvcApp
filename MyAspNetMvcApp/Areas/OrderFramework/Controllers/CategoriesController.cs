﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.Areas.OrderFramework.Models;

namespace MyAspNetMvcApp.Areas.OrderFramework.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        public async Task<ActionResult> Index()
        {
            return View(await db.OF_Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category catagorie = await db.OF_Categories.FindAsync(id);
            if (catagorie == null)
            {
                return HttpNotFound();
            }
            return View(catagorie);
        }

        // GET: Categories/Create
         [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name")] Category catagorie)
        {
            if (ModelState.IsValid)
            {
                db.OF_Categories.Add(catagorie);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(catagorie);
        }

        // GET: Categories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category catagorie = await db.OF_Categories.FindAsync(id);
            if (catagorie == null)
            {
                return HttpNotFound();
            }
            return View(catagorie);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name")] Category catagorie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catagorie).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(catagorie);
        }

        // GET: Categories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category catagorie = await db.OF_Categories.FindAsync(id);
            if (catagorie == null)
            {
                return HttpNotFound();
            }
            return View(catagorie);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Category catagorie = await db.OF_Categories.FindAsync(id);
            db.OF_Categories.Remove(catagorie);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
