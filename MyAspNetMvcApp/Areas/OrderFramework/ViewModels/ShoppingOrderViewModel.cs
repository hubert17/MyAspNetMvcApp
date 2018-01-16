using MyAspNetMvcApp.Areas.OrderFramework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.OrderFramework.ViewModels
{
    public class ShoppingOrderViewModel
    {
        // https://github.com/ridercz/Altairis.ValidationToolkit
        // https://forums.asp.net/t/2052218.aspx?Conditional+Required+Field+MVC5
        public int Id { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string CCNumber { get; set; }
        public string CCHolderName { get; set; }
        public int CCExpiryMonth { get; set; }
        public int CCExpiryYear { get; set; }
        public int CCV { get; set; }

        public ShoppingCartViewModel ShoppingCart { get; set; }
    }
}