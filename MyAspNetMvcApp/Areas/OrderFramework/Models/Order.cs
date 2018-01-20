using Newtonsoft.Json;
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

        public List<OrderDetail> OrderDetails { get; set; }

        [Column("ShippingDetail")]
        internal string _ShippingDetail { get; set; }
        [NotMapped]
        public ShippingDetail ShippingDetail
        {
            get { return _ShippingDetail == null ? null : JsonConvert.DeserializeObject<ShippingDetail>(_ShippingDetail); }
            set { _ShippingDetail = JsonConvert.SerializeObject(value); }
            // https://www.codeproject.com/Articles/1166099/Entity-Framework-Storing-complex-properties-as-JSO
        }

        public string MetaData { get; set; }

        [ScaffoldColumn(false)]
        public string CreditCardId { get; set; }

    }

    public class ShippingDetail
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public string Notes { get; set; }
        public string Status { get; set; }
    }
}