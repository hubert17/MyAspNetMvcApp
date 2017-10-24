using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAspNetMvcApp.Areas.Account.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, 
    // please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //[Index(IsUnique = true)]
        [MaxLength(15)]
        public override string PhoneNumber { get; set; }
        [MaxLength(3)]
        public string CountryCode { get; set; }
        public string Token { get; set; }
        public DateTime? TokenExpiration { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }

}