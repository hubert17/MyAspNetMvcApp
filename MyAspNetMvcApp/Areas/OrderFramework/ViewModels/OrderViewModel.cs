using MyAspNetMvcApp.Areas.OrderFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.OrderFramework.ViewModels
{
    public class ShoppingOrderViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CountryCode { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }

        public string CCNumber { get; set; }
        public string CCHolderName { get; set; }
        public int CCExpiryMonth { get; set; }
        public int CCExpiryYear { get; set; }
        public int CCV { get; set; }

        public ShoppingCartViewModel ShoppingCart { get; set; }
    }
}