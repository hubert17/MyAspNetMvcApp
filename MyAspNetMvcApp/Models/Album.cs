using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
        public string RecordingLabel { get; set; }
        public int ReleaseYear { get; set; }
        public string ArtworkFilename { get; set; }
    }
}