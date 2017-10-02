using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyAspNetMvcApp.Areas.Account.Models;

namespace MyAspNetMvcApp.Models.OrderApp
{
    public class Order
    {
        public int Id { get; set; }        
        public string UserName { get; set; } //Foreign Key        
        public UserProfile Customer { get; set; } //Navigation property
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        //Navigation property
        public virtual List<OrderItem> OrderItems { get; set; }
    }
}