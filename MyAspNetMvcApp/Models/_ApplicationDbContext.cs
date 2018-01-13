using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyAspNetMvcApp.Areas.Account.Models;
using MyAspNetMvcApp.Areas.App.Models;
using MyAspNetMvcApp.Areas.OrderFramework.Models;

namespace MyAspNetMvcApp.Models
{
    // Uncomment the line below when using MySql Database
    //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base(System.Configuration.ConfigurationManager.AppSettings["AppDbContextDatabase"]) // Connection string name ex: "DbConnJetAccess"
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().Property(u => u.UserName).HasMaxLength(255);
            modelBuilder.Entity<ApplicationUser>().Property(u => u.Email).HasMaxLength(255);
            modelBuilder.Entity<IdentityRole>().Property(r => r.Name).HasMaxLength(255);
        }

        // DO NOT REMOVE THIS!
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Lookup> Lookups { get; set; }

        // Put your database tables here...
        // public DbSet<Class> TableName { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

    }


}