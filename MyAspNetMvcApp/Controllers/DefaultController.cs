using MyAspNetMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            var souls = new List<string>();
            souls.Add("Aljo");
            souls.Add("Alma");
            souls.Add("Porto");
            souls.Add("Tiborsio");

            var students = new List<Student>();
            var stud1 = new Student();
            stud1.LastName = "Malaque";
            stud1.FirstName = "Aljo";
            stud1.Course = "BSIT";
            students.Add(stud1);

            var stud2 = new Student() { LastName = "Locsin", FirstName = "Angel", Course = "BSCS" };
            students.Add(stud2);

            students.Add(new Student() { LastName = "Gabon", FirstName = "Bern", Course = "BSED" });
            ViewBag.studs = students;


            ViewBag.souls = souls;

            return View();
        }

        public ActionResult PhoneBook(string contactname, string username)
        {
            Session["user"] = username;

            var phonebook = new Dictionary<string, string>();
            phonebook["bernard"] = "099999999";
            phonebook["alma"] = "088888888";
            phonebook["aljo"] = "077777777";

            ViewBag.contactno = phonebook[contactname.ToLower()];
            ViewBag.contactname = contactname;
            return View("Index");
        }

        public ActionResult Kalag()
        {
            var mga_ngalan = new List<string>();
            mga_ngalan.Add("Angelica");
            mga_ngalan.Add("Verhenia");
            mga_ngalan.Add("Nicka");

            ViewBag.names = mga_ngalan;

            var movies = new List<Movie>();
            var movie1 = new Movie();
            movie1.Title = "Titanic";
            movie1.MainActor = "Leonardo";
            movie1.Genre = "History/Suspense";
            movie1.Director = "James Cameron";
            movie1.ReleaseYear = 1997;
            movies.Add(movie1);

            var transformer = new Movie() { Title = "Transformer", MainActor = "Ambot", Genre = "Scifi", ReleaseYear = 2007 };
            movies.Add(transformer);

            movies.Add(new Movie() { Title = "Finding Dory", MainActor = "Ellen", Director = "Andrew Stanton", Genre = "Family", ReleaseYear = 2016 });
            ViewBag.movies = movies;

            return View();
        }
    }
}