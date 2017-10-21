using MyAspNetMvcApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp
{
    public class Lookup
    {
        public int Id { get; set; }
        public string Type { get; set; }
        [Index(IsUnique = true)] //Makes the property unique
        public int Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }

        #region Methods
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static string GetValue(int? lookupKey)
        {
            string value = string.Empty;
            try
            {
                if (lookupKey != null)
                    value = db.Lookups.Where(x => x.Key.Equals((int)lookupKey)).Select(x => x.Value).First();
                return value.Length > 0 ? value : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static int GetKey(string lookupValue, string lookupType)
        {
            int key = 0;

            if (lookupValue != null)
                key = db.Lookups.Where(x => x.Value.ToLower() == lookupValue.ToLower() && x.Type.ToLower() == lookupType.ToLower()).Select(x => x.Key).First();

            return key;
        }

        //@Html.DropDownListFor(m => m.Property, MyAspNetMvcApp.Lookup.DropDownList("order_status"), new { @class = "form-control" })
        public static SelectList DropDownList(string lookupType)
        {
            return new SelectList(db.Lookups.Where(l => l.Type.ToLower().Trim() == lookupType.ToLower().Trim() && l.IsActive == true).OrderBy(l => l.SortOrder), "Key", "Value");
        }

        //@Html.DropDownListFor(m => m.Property, MyAspNetMvcApp.Lookup.DropDownList("product_category", Model.Property), new { @class = "form-control" })
        public static SelectList DropDownList(string lookupType, object selectedValue)
        {
            return new SelectList(db.Lookups.Where(l => l.Type.ToLower().Trim() == lookupType.ToLower().Trim() && l.IsActive == true).OrderBy(l => l.SortOrder), "Key", "Value", selectedValue);
        }

        #endregion

    }
}