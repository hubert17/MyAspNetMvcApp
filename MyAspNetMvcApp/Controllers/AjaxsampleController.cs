using MyAspNetMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class AjaxsampleController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ajaxsample
        public ActionResult Index()
        {
            var books = db.Books.ToList();
            return View(books);
        }

        [HttpPost]
        public ActionResult Create(Book book, HttpPostedFileBase fileUpload)
        {
            book.ImgFilename = Gabs.Helpers.ImageUploadUtil.SaveToJpegFile(fileUpload, book.Title);
            db.Books.Add(book);
            db.SaveChanges();

            return PartialView("_book", book);
        }

    }
}