﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.Account.Models
{
    public class UserProfile
    {
        [Key]
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public byte[] Picture { get; set; }
        public string RegistrationType { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public string MetaData { get; set; }
        public bool IsActive { get; set; }
    }


}