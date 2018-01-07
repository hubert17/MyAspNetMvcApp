using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.OrderFramework.Models
{
    //[Bind(Exclude = "ID")]
    [Table("OF_Items")]
    public class Item
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [DisplayName("Catagories")]
        public int CatagoryId { get; set; }

        [Required(ErrorMessage = "An Item Name is required")]
        [StringLength(160)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        //[Range(0.01, 999.99, ErrorMessage = "Price must be between 0.01 and 999.99")]
        public double UnitPrice { get; set; }

        public byte[] InternalImage { get; set; }

        [DisplayName("Item Picture URL")]
        [StringLength(1024)]
        public string ItemPictureUrl { get; set; }

        public virtual Category Catagory { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }

}