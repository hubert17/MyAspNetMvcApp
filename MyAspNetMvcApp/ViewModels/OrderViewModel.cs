using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.ViewModels
{
    public class OrderViewModel
    {
        // Parent
        public int OrderId { get; set; } // Same as the Id of Order
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public double OrderTotal { get; set; }

        // Child
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}