using MyAspNetMvcApp.Areas.Account.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Models
{

    public class Student
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public UserProfile Profile { get; set; }
        public string Course { get; set; }
        public string SchoolLastGraduated { get; set; }

    }

    public class Faculty
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public UserProfile Profile { get; set; }
        public DateTime? HireDate { get; set; }
        public string SSSNumber { get; set; }

    }

}