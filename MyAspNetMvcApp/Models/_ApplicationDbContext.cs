using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyAspNetMvcApp.Areas.Account.Models;
using MyAspNetMvcApp.Areas.App.Models;

namespace MyAspNetMvcApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DbConnJetAccess") // Connection string name
        {
        }

        // DO NOT REMOVE THIS!
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Lookup> Lookups { get; set; }

        // Put your database tables here...
        // public DbSet<Class> TableName { get; set; }

    }
}