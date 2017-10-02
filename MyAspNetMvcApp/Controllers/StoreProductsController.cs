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

namespace MyAspNetMvcApp.Controllers
{
    [Authorize]
    public class StoreProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StoreProducts
        [AllowAnonymous]
        public ActionResult Index(string keyword, int sort = 1)
        {
            List<Product> result;
            if(string.IsNullOrEmpty(keyword))
            {
                result = db.Products.ToList();
            }
            else
            {
                result = db.Products.Where(x => x.Name.Contains(keyword)).ToList();
            }

            if (sort == 1)
                result = result.OrderBy(x => x.Price).ToList();
            else if (sort == 2)
                result = result.OrderByDescending(x => x.Price).ToList();


                return View(result);
        }

        // GET: StoreProducts/Details/5
        public ActionResult Details(int? id)
        {
            Product product = db.Products.Find(id);
            return View(product);
        }

        // GET: StoreProducts/Create
        [Authorize(Roles = "staff")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: StoreProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase FileUpload)
        {
            product.Picture = Gabs.Helpers.ImageUploadUtil.FileToByteArray(FileUpload);
            db.Products.Add(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: StoreProducts/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: StoreProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,CategoryId,Price,Picture")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: StoreProducts/Delete/5
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

        // POST: StoreProducts/Delete/5
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
