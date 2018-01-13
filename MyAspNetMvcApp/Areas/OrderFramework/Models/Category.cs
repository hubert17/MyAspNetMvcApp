using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.OrderFramework.Models
{
    [Table("OF_Categories")]
    public class Category
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [DisplayName("Catagory Name")]
        public string Name { get; set; }

        public virtual ICollection<Product> Items { get; set; }
    }
}