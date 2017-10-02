using MyAspNetMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class SignalrSampleController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            //List<Book> books = (from book in db.Books select book).ToList();
            var books = db.Books.ToList();

            return View(books);

        }

        public ActionResult GetBook(int Id)
        {
            var book = db.Books.Find(Id);
            return PartialView("_GetBook", book);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Book book)
        {
            db.Books.Add(book);
            db.SaveChanges();
            TempData["MessagePanel"] = book.Title + " has been successfully added.";

            //Once the record is inserted , then notify all the subscribers (Clients)
            SignalRHub.NotifyNewBookAdded(book.Id);

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

            //Once the record is inserted , then notify all the subscribers (Clients)
            SignalRHub.NotifyNewBookUpdated(book.Id);

            return RedirectToAction("Index", "SignalrSample");

        }

        [HttpGet]
        public ActionResult Delete(int Id)
        {
            var book = db.Books.Find(Id);
            db.Books.Remove(book);
            db.SaveChanges();
            TempData["MessageAlert"] = book.Title + " has been deleted.";

            //Once the record is inserted , then notify all the subscribers (Clients)
            SignalRHub.NotifyNewBookDeleted(Id);

            return RedirectToAction("Index", "SignalrSample");
        }

    }
}