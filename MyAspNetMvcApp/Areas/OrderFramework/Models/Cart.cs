using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.OrderFramework.Models
{
    [Table("OF_Carts")]
    public class Cart
    {
        public int Id { get; set; }
        public string CartId { get; set; }
        public int ItemId { get; set; }
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }
        public virtual Product Item { get; set; }
    }
}