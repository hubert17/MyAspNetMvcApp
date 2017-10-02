namespace MyAspNetMvcApp.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using MyAspNetMvcApp.Models;
    using MyAspNetMvcApp.Areas.Account.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using MyAspNetMvcApp.Areas.App.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            SeedIdentity(context);
            SeedLookup(context);

        }

        private void SeedLookup(ApplicationDbContext context)
        {
            context.Lookups.AddOrUpdate(
              p => p.Key,
                new Lookup { Type = "gender", Key = 1, Value = "Male" },
                new Lookup { Type = "gender", Key = 2, Value = "Female" },
                new Lookup { Type = "order_status", Key = -1, Value = "Shopping"},
                new Lookup { Type = "order_status", Key = 10, Value = "Pending" },
                new Lookup { Type = "order_status", Key = 11, Value = "Processing" },
                new Lookup { Type = "order_status", Key = 12, Value = "Shipping" },
                new Lookup { Type = "order_status", Key = 13, Value = "Delivered" },
                new Lookup { Type = "product_category", Key = 20, Value = "Men's Clothing" },
                new Lookup { Type = "product_category", Key = 21, Value = "Women’s Clothing" },
                new Lookup { Type = "product_category", Key = 22, Value = "Computer & Office" },
                new Lookup { Type = "product_category", Key = 23, Value = "Consumer Electronics" },
                new Lookup { Type = "product_category", Key = 24, Value = "Toys, Kids & Baby" }
            );

        }

        private void SeedIdentity(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "admin"))
            {
                roleManager.Create(new IdentityRole("admin"));
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var username = System.Configuration.ConfigurationManager.AppSettings["AdminUsername"];
            if ((userManager.FindByName(username) == null))
            {

                var user = new ApplicationUser
                {
                    UserName = username,
                    PhoneNumber = "1234567890",                    
                    UserProfile = new UserProfile { UserName = username, UserType = "admin", LastName = "Admin", FirstName = "Temp", RegistrationDate = DateTime.Now, IsActive = true }
                };
                userManager.Create(user, System.Configuration.ConfigurationManager.AppSettings["AdminPassword"]);
                userManager.AddToRole(user.Id, "admin");


            }
            username = System.Configuration.ConfigurationManager.AppSettings["TempUsername"];
            if ((userManager.FindByName(username) == null))
            {
                var user = new ApplicationUser
                {
                    UserName = username,
                    PhoneNumber = "9876543210",
                    UserProfile = new UserProfile { UserName = username, LastName = "User", FirstName = "Temp", RegistrationDate = DateTime.Now, IsActive = true }
                };
                userManager.Create(user, System.Configuration.ConfigurationManager.AppSettings["TempPassword"]);
            }

        }

    }
}
