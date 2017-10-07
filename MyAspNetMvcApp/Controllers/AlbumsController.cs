using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.ViewModels;

namespace MyAspNetMvcApp.Controllers
{
    public class AlbumsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Albums
        public ActionResult Index()
        {
            var albums = db.Albums.Include(a => a.Artist);
            return View(albums.ToList());
        }

        public ActionResult AddTrack(int Id)
        {
            var albumVM = db.Albums.Where(x => x.Id == Id)
                .Include(a => a.Artist)
                .Select(s => new AlbumViewModel
                {
                    AlbumId = s.Id,
                    Title = s.Title,
                    ArtistName = s.Artist.Name,
                    RecordingLabel = s.RecordingLabel,
                    ReleaseYear = s.ReleaseYear,
                    ArtworkFilename = s.ArtworkFilename,
                }).FirstOrDefault();

            albumVM.Tracks = db.Tracks.Where(x => x.AlbumId == Id).ToList();

            return View(albumVM);
        }

        [HttpPost]
        public ActionResult AddTrack(Track track, int AlbumId)
        {
            track.AlbumId = AlbumId;
            db.Tracks.Add(track);
            db.SaveChanges();

            return RedirectToAction("AddTrack", new { Id = AlbumId });
        }

        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // GET: Albums/Create
        public ActionResult Create()
        {
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name");
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Album album, HttpPostedFileBase FileUpload)
        {
            album.ArtworkFilename = Gabs.Helpers.ImageUploadUtil.SaveToJpegFile(FileUpload);
            db.Albums.Add(album);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", album.ArtistId);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,ArtistId,RecordingLabel,ReleaseYear")] Album album)
        {
            if (ModelState.IsValid)
            {
                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", album.ArtistId);
            return View(album);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Find(id);
            db.Albums.Remove(album);
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
