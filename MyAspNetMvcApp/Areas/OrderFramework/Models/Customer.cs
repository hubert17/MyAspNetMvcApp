﻿using MyAspNetMvcApp.Areas.Account.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.OrderFramework.Models
{
    public class Customer
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
        public DateTime CCExpiry { get; set; }
        public string CCHolderName { get; set; }
    }
}