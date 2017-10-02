using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.App.Models
{
    public class LookupViewModel
    {
        public List<Lookup> Lookups { get; set; }
        public Lookup NewLookup { get; set; }
    }
}