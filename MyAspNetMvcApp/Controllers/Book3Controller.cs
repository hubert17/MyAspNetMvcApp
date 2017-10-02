using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;
using System.Data.Entity;

namespace MyAspNetMvcApp.Controllers
{
    public class Book3Controller : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Book3
        public ActionResult Index()
        {
            //List<Book> books = (from book in db.Books select book).ToList();
            var books = db.Books.ToList();

            if (TempData.Keys.Count <= 0)
            {
                ViewBag.MessageBox = "Welcome to Book app by Bootstrap Modal!";
                ViewBag.MessagePanel = "Welcome to Book app by BS Alert Panel!";
                ViewBag.MessageAlert = "Welcome to Book app by Kavascript Alert !";
            }

            return View(books);

        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Book book, HttpPostedFileBase fileUpload)
        {
            book.Picture = Gabs.Helpers.ImageUploadUtil.FileToByteArray(fileUpload);
            //book.ImgFilename = Gabs.Helpers.ImageUploadUtil.SaveToJpegFile(fileUpload, book.Title);
            db.Books.Add(book);
            db.SaveChanges();

            TempData["MessagePanel"] = book.Title + " has been successfully added.";
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var book = db.Books.Find(id);
            return View(book);
        }
        
        [HttpPost]
        public ActionResult Edit(Book book)
        {
            db.Entry(book).State = EntityState.Modified;
            db.SaveChanges();
            TempData["MessageBox"] = book.Title + " has been updated.";
            return RedirectToAction("Index", "Book3");

        }

        [HttpGet]
        public ActionResult Delete(int Id)
        {
            var book = db.Books.Find(Id);
            TempData["MessageAlert"] = book.Title + " has been deleted.";
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index", "Book3");
        }

    }
}