using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.OrderFramework.ViewModels
{
    public class ShoppingCartResponseViewModel
    {
        public string ItemName { get; set; }
        public int ItemQty { get; set; }
        public decimal CartTotal { get; set; }
        public string CartTotalFormatted { get; set; }
        public int CartCount { get; set; }
        public int DeleteId { get; set; }
    }
}