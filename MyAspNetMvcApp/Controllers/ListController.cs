using MyAspNetMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Controllers
{
    public class ListController : Controller
    {
        // GET: List
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Songs(Song s)
        {
            var songs = new List<Song>();
            songs.Add(s);
            return View(songs);

            //

            //var song1 = new Song();
            //song1.Title = "Ikaw";
            //song1.Artist = "Yang";
            //song1.Artwork = "20170410_175509.jpg";
            //songs.Add(song1);

            //var song2 = new Song() { Title = "Nothing else matta", Artist = "Metallicca", Artwork = "20170410_175629.jpg" };
            //songs.Add(song2);

            //songs.Add(new Song() { Title = "Born For You", Artist = "David P", Artwork = "20170412_101825.jpg" });


            //return View(songs);
        }
    }
}