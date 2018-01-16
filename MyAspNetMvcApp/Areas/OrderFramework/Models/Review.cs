using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAspNetMvcApp.Areas.OrderFramework.Models
{
    [Table("OF_Reviews")]
    public class Review
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int ProductId { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsApproved { get; set; }
        public bool IsSpam { get; set; }
    }
}