using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyAspNetMvcApp.Areas.Account.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyAspNetMvcApp.Models.OrderApp
{
    public class Order
    {
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public string UserName { get; set; }
        //Navigation property
        public Customer Customer { get; set; } 

        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
    }
}