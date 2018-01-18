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
        public ActionResult Index(string songId, string delSongId, string encUsername)
        {
            Account.Models.UserProfile student;

            if (Request.IsAuthenticated)
                student = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            else
            {
                encUsername = Base64Decode(encUsername);
                student = db.UserProfiles.Where(x => x.UserName == encUsername).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(songId))
            {
                var songIds = new List<string>();
                var metadata =  JsonConvert.DeserializeObject<StudentMetadata>(student.MetaData);
                if(metadata.songIds != null)
                    songIds = metadata.songIds.ToList();

                songIds.Add(songId);

                student.MetaData = JsonConvert.SerializeObject(new { section = metadata.section, songIds = songIds });
                db.Entry(student).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else if(!string.IsNullOrEmpty(delSongId))
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

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            // https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }

}