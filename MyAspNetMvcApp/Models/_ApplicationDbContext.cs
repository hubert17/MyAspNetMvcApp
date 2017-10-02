using System.Data.Entity;
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
        public ApplicationDbContext() : base("DbConnJetAccess")
        {
        }

        // DO NOT REMOVE THIS!
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Lookup> Lookups { get; set; }

        // Put your database tables here...
        public DbSet<Project> Projects { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Book> Books { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}