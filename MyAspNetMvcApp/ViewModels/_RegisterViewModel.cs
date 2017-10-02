using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.Models.OrderApp;

namespace MyAspNetMvcApp.Areas.Account.ViewModels
{
    public partial class RegisterViewModel
    {
        //Add your custom Registration fields here
        public string City { get; set; }





        // DO NOT REMOVE! This saves custom field data upon Registration
        public static void SaveRegistrationCustomData(RegisterViewModel register)
        {
            var db = new ApplicationDbContext();

            // Add your custom User Registration class here
            var customer = new Customer();
            customer.UserName = register.UserName;
            customer.City = register.City;
            db.Customers.Add(customer);
            db.SaveChanges();
        }

    }
}