using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        public int PublishYear { get; set; }
        public byte[] Picture { get; set; }
        public string ImgFilename { get; set; }
    }
}