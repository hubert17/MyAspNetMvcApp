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
        private string _userNameEmailBackingField;

        public override string UserName
        {
            get { return _userNameEmailBackingField; }
            set { _userNameEmailBackingField = value; }
        }

        public override string Email
        {
            get { return _userNameEmailBackingField; }
            set { _userNameEmailBackingField = value; }
        }

        [MaxLength(20)]
        [Index(IsUnique = true)]
        public override string PhoneNumber { get; set; }
        public string CountyCode { get; set; }
        public string Token { get; set; }
        public DateTime? TokenExpiration { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}