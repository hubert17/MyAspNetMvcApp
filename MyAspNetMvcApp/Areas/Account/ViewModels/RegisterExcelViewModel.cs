using SimpleExcelImport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.Account.ViewModels
{
    public class RegisterExcelViewModel
    {
        [ExcelImport("UserName", order = 1)]
        public string UserName { get; set; }
        [ExcelImport("LastName", order = 2)]
        public string LastName { get; set; }
        [ExcelImport("FirstName", order = 3)]
        public string FirstName { get; set; }
        [ExcelImport("Email", order = 4)]
        public string Email { get; set; }
        [ExcelImport("CountryCode", order = 5)]
        public string CountryCode { get; set; }
        [ExcelImport("PhoneNumber", order = 6)]
        public string PhoneNumber { get; set; }
        [ExcelImport("BirthDate", order = 7)]
        public string BirthDate { get; set; }
        [ExcelImport("Gender", order = 8)]
        public string Gender { get; set; }
        [ExcelImport("RegistrationType", order = 9)]
        public string RegistrationType { get; set; }

    }
}