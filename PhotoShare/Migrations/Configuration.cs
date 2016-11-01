using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;

namespace PhotoShare.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PhotoShare.DataAccess.DataContext.PhotoShareDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        private void AddUser(PhotoShareDbContext context)
        {
            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));
            var user = new User()
            {
                UserName = "Dionmm",
                Email = "dion@macinty.re",
                PhoneNumber = "07881913156"
            };
            if (userManager.FindByName(user.UserName) == null)
            {
                var identityResult = userManager.Create(user, "password");
                if (identityResult.Succeeded)
                {
                    var currentUser = userManager.FindByName(user.UserName);
                    var roleResult = userManager.AddToRole(currentUser.Id, "administrator");
                }
            }
        }

        private void AddRole(PhotoShareDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string roleName = "administrator";
            if (!roleManager.RoleExists(roleName))
            {
                IdentityResult roleResult = roleManager.Create(new IdentityRole(roleName));
            }
        }
        protected override void Seed(PhotoShare.DataAccess.DataContext.PhotoShareDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            AddRole(context);
            AddUser(context);

        }
    }
}
