using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;

namespace MyAspNetMvcApp.Areas.Examples.Models
{
    public class ExamplesDbContext : DbContext
    {
        public ExamplesDbContext() 
        {
            Database.SetInitializer<ExamplesDbContext>(new DropCreateDatabaseIfModelChanges<ExamplesDbContext>());
        }

        // Put your database tables here...
        // public DbSet<Class> TableName { get; set; }

        public DbSet<Student> Students { get; set; }
    }


}