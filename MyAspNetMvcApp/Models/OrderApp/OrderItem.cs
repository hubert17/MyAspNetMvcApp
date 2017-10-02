using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Models.OrderApp
{
    public class OrderItem
    {
        public int Id { get; set; }
        //Foreign Key
        public int ProductId { get; set; }
        //Navigation property
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }

    }
}