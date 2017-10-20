using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.Examples.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Index(IsUnique = true)]
        public string IdNumber { get; set; }
        //[Index("IX_FirstAndSecond", 1, IsUnique = true)]
        public string LastName { get; set; }
        //[Index("IX_FirstAndSecond", 2, IsUnique = true)]
        public string FirstName { get; set; }
        public string YearSection { get; set; }
    }
}