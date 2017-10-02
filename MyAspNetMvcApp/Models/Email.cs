using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Models
{
    public class Email
    {
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}