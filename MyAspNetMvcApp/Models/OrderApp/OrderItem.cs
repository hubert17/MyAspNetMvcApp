using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Models.OrderApp
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Id of parent Order

        [ForeignKey("Product")]     
        public int ProductId { get; set; } // The Id of Product
        public Product Product { get; set; }  //Navigation property

        public int Quantity { get; set; }
    }
}