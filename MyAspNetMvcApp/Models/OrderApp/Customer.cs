using MyAspNetMvcApp.Areas.Account.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Models.OrderApp
{
    public class Customer
    {
        // Note: If Primary Key is not of type Integer
        // then a [Key] attribute is required.

        // Primary Key at the same time...
        [Key, ForeignKey("Profile")]  // a Foreign key
        public string UserName { get; set; }
        public UserProfile Profile { get; set; }

        public int Gender { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int Province { get; set; }
        public string PostalCode { get; set; }
    }
}