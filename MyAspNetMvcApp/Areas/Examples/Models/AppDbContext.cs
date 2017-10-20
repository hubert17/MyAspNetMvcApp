using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace MyAspNetMvcApp.Areas.Examples.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("ExamplesDb") // Connection string name ex: "DbConnJetAccess"
        {
        }

        // Put your database tables here...
        // public DbSet<Class> TableName { get; set; }

        public DbSet<Student> Students { get; set; }
    }


}