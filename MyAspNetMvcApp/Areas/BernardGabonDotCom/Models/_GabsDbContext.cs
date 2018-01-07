using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyAspNetMvcApp.Areas.Account.Models;
using MyAspNetMvcApp.Areas.App.Models;

namespace MyAspNetMvcApp.Areas.BernardGabonDotCom.Models
{
    public class _GabsDbContext : IdentityDbContext<ApplicationUser>
    {
        public _GabsDbContext() : base("DbConnSqlAzure")
        {
        }

        // DO NOT REMOVE THIS!
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Lookup> Lookups { get; set; }

        // Put your database tables here...
        public DbSet<Project> Projects { get; set; }
        public DbSet<Submission> Submissions { get; set; }

    }
}