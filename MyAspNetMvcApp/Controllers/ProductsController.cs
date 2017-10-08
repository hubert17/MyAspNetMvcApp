using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.Models.OrderApp;
using Gabs.Helpers;

namespace MyAspNetMvcApp.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        [AllowAnonymous]
        public ActionResult Index(string keyword)
        {
            List<Product> result;
            if(string.IsNullOrEmpty(keyword))
            {
                result = db.Products.ToList();
            }
            else
            {
                result = db.Products.Where(p => p.Name.Contains(keyword)).ToList();
            }
            
            return View(result);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            Product product = db.Products.Find(id);
            return View(product);
        }

        // GET: Products/Create
        //[Authorize(Roles = "staff")]
        public ActionResult Create()
        {
            var categories = db.Lookups.Where(x => x.Type == "product_category")
                .Select(s => new SelectListItem { Value = s.Key.ToString(), Text = s.Value })
                .ToList();

            ViewBag.categories = categories;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase fileUpload)
        {
            product.Picture = fileUpload.ToImageByteArray();
            db.Products.Add(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            Product product = db.Products.Find(id);

            if (product == null)
            {
                TempData["MessageAlert"] = "Product does not exist.";
                return RedirectToAction("Index");
            }

            ViewBag.categories = db.Lookups.Where(x => x.Type == "product_category")
                .Select(s => new SelectListItem
                {
                    Value = s.Key.ToString(),
                    Text = s.Value,
                    Selected = s.Id == product.CategoryId ? true : false
                }).ToList();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase fileUpload)
        {
            db.Entry(product).State = EntityState.Modified;

            if (fileUpload != null) // Update picture
                product.Picture = fileUpload.ToImageByteArray();
            else // Retain the current picture
                db.Entry(product).Property(x => x.Picture).IsModified = false;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
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
