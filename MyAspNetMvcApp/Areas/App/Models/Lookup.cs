using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.App.Models
{
    public class Lookup
    {
        public int Id { get; set; }
        public string Type { get; set; }
        [Index(IsUnique = true)] //Makes the property unique
        public int Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool InActive { get; set; }
    }
}