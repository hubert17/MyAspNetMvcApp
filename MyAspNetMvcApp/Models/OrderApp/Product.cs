using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Models.OrderApp
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; } // Nullable
        public double Price { get; set; }
        public byte[] Picture { get; set; }
        public string PictureFilename { get; set; }
    }
}