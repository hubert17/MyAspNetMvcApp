using MyAspNetMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.ViewModels
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsActive { get; set; }

        public static string getProjectNameById(int Id)
        {
            var db = new ApplicationDbContext();
            return db.Projects.Find(Id).Name;
        }

        public static SelectList getSelectList()
        {
            var db = new ApplicationDbContext();
            var items = db.Projects.Where(x=>x.IsActive != false).Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            var selectList = new SelectList(items, "Value", "Text");
            return selectList;
        }
    }
}