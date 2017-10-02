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
        public int CustomerId { get; set; }
        public string UserName { get; set; }
        public UserProfile Profile { get; set; }
        public int Gender { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int Province { get; set; }
    }
}