using SimpleExcelImport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.Examples.ViewModels
{
    public class StudentExcelViewModel
    {
        [ExcelImport("Student No.", order = 2)]
        public string IdNumber { get; set; }
        [ExcelImport("Student Name", order = 3)]
        public string LastName { get; set; }
        [ExcelImport("", ignore = true)]
        public string FirstName { get; set; }
        [ExcelImport("Section", order = 4)]
        public string YearSection { get; set; }

    }
}