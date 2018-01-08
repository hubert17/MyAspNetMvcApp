using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;
using Gabs.Helpers;
using MyAspNetMvcApp.Areas.OrderFramework.Models;
using X.PagedList;

namespace MyAspNetMvcApp.Areas.OrderFramework.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int pageSize = 3)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var items = db.Products.AsEnumerable();

            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                                       || s.Category.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    items = items.OrderBy(s => s.UnitPrice);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(s => s.UnitPrice);
                    break;
                default:  // Name ascending 
                    items = items.OrderBy(s => s.Name);
                    break;
            }

            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));


            //var items = db.Items.Include(i => i.Catagorie);
            //return View(await items.ToListAsync());
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
            var categories = db.Categories
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
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
            return RedirectToAction("Index", "Products", new { area = "store" });
        }


        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            Product product = db.Products.Find(id);

            if (product == null)
            {
                TempData[BSMessage.ALERT] = "Product does not exist.";
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

            return RedirectToAction("Index", "Products", new { area = "store" });
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
            return RedirectToAction("Index", "Products", new { area = "store" });
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
