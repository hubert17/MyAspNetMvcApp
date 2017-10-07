using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyAspNetMvcApp.Models;

namespace MyAspNetMvcApp.ViewModels
{
    public class AlbumViewModel
    {
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string RecordingLabel { get; set; }
        public int ReleaseYear { get; set; }
        public string ArtworkFilename { get; set; }

        public virtual List<Track> Tracks { get; set; }
    }
}