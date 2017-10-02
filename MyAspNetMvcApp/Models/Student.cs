using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Models
{
    public class Student
    {
        [Key]
        public string UserName { get; set; }
        public DateTime BirthDate { get; set; }
        public string SchoolLastAttended { get; set; }
    }
}