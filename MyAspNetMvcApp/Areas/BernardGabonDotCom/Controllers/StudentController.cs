using MyAspNetMvcApp.Areas.BernardGabonDotCom.Models;
using MyAspNetMvcApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.BernardGabonDotCom.Controllers
{
    // https://www.codeproject.com/Articles/1166099/Entity-Framework-Storing-complex-properties-as-JSO
    public class StudentController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: BernardGabonDotCom/Student
        public ActionResult Index(string songId = "", string delSongId = "", string encUsername = "")
        {
            string UserName = Request.IsAuthenticated ? User.Identity.Name : Gabs.Helpers.EncryptionForUrl.Decrypt(encUsername);

            if(!string.IsNullOrEmpty(UserName))
            {
                var student = db.UserProfiles.Where(x => x.UserName == UserName).FirstOrDefault();

                if (!string.IsNullOrEmpty(songId))
                {
                    var songIds = new List<string>();
                    var metadata = JsonConvert.DeserializeObject<StudentMetadata>(student.MetaData);
                    if (metadata.songIds != null)
                        songIds = metadata.songIds.ToList();

                    if (!songIds.Contains(songId))
                        songIds.Add(songId);

                    student.MetaData = JsonConvert.SerializeObject(new { section = metadata.section, songIds = songIds });
                    db.Entry(student).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else if (!string.IsNullOrEmpty(delSongId))
                {
                    var songIds = new List<string>();
                    var metadata = JsonConvert.DeserializeObject<StudentMetadata>(student.MetaData);
                    if (metadata.songIds != null)
                        songIds = metadata.songIds.ToList();

                    songIds.Remove(delSongId);

                    student.MetaData = JsonConvert.SerializeObject(new { section = metadata.section, songIds = songIds });
                    db.Entry(student).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return View(student);
            }
            else
                return RedirectToAction("Index", "Home", new { area = "" });

        }

    }

}