﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyAspNetMvcApp.Areas.Account.Models;
using MyAspNetMvcApp.Areas.App.Models;
using MyAspNetMvcApp.Models.OrderApp;

namespace MyAspNetMvcApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base(System.Configuration.ConfigurationManager.AppSettings["AppDbContextDatabase"]) // Connection string name ex: "DbConnJetAccess"
        {
        }

        // DO NOT REMOVE THIS!
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Lookup> Lookups { get; set; }

        // Put your database tables here...
        // public DbSet<Class> TableName { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

    }


}