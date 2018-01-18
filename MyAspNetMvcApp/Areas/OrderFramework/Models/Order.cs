using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.OrderFramework.Models
{
    [Table("OF_Orders")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ScaffoldColumn(false)]
        public string UserName { get; set; }

        public Customer Customer { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderStatus { get; set; }

        [ScaffoldColumn(false)]
        public decimal OrderTotal { get; set; }

        public string MetaData { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }

        public ShippingDetail ShippingDetail { get; set; }

        [ScaffoldColumn(false)]
        public string CreditCardId { get; set; }

    }
}