using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Models
{
    public class Track
    {
        public int Id { get; set; }
        public int AlbumId { get; set; }
        public int No { get; set; }
        public string Title { get; set; }
        public string Time { get; set; }
        public string AudioFilename { get; set; }
    }
}