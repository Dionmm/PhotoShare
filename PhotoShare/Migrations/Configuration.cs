using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoShare.App_Start;
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

        private void AddUser(PhotoShareDbContext context, User user, String password, String roleName)
        {
            var userManager = new ApplicationUserManager(new UserStore<User>(context));
            
            if (userManager.FindByName(user.UserName) == null)
            {
                var identityResult = userManager.Create(user, password);
                if (identityResult.Succeeded)
                {
                    var currentUser = userManager.FindByName(user.UserName);
                    var roleResult = userManager.AddToRole(currentUser.Id, roleName);
                }
            }
        }

        private void AddRole(PhotoShareDbContext context, String roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            
            if (!roleManager.RoleExists(roleName))
            {
                IdentityResult roleResult = roleManager.Create(new IdentityRole(roleName));
            }
        }

        private void AddMessage(PhotoShareDbContext context, User user, String content)
        {
            var userManager = new ApplicationUserManager(new UserStore<User>(context));
            var currentUser = userManager.FindByName(user.UserName);
            if (currentUser != null)
            {
                context.Messages.AddOrUpdate(
                    new Message
                    {
                        Content = content,
                        User = currentUser,
                        CreatedDateTime = DateTime.Now,
                        UpdatedDateTime = DateTime.Now
                    });
            }
        }

        private void AddPhoto(PhotoShareDbContext context, User user, String name, String address, decimal price)
        {
            var userManager = new ApplicationUserManager(new UserStore<User>(context));
            var currentUser = userManager.FindByName(user.UserName);
            if (currentUser != null)
            {
                context.Photos.Add(
                    new Photo
                    {
                        Name = name,
                        Address = address,
                        Price = price,
                        CreatedDateTime = DateTime.Now,
                        UpdatedDateTime = DateTime.Now,
                        User = currentUser
                    });
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

            var dion = new User
            {
                UserName = "Dionmm",
                Email = "dion@macinty.re",
                PhoneNumber = "07881913156"
            };
            var jack = new User
            {
                UserName = "JackBlack",
                Email = "jack@macinty.re",
                PhoneNumber = "07881913134"
            };
            const string administrator = "administrator";
            const string photographer = "photographer";

            AddRole(context, administrator);
            AddRole(context, photographer);
            AddUser(context, dion, "password", administrator);
            AddUser(context, jack, "password", photographer);
            AddMessage(context, dion, "Testing messages");
            AddMessage(context, jack, "Hey man how's it going");
            AddMessage(context, dion, "nb mate, you");
            AddMessage(context, dion, "ye gg bby");
            AddPhoto(context, dion, "MyPhoto1", "/blah/folder/file.jpg", 4.50m);
            AddPhoto(context, dion, "MyPhoto2", "/blah/folder/file.jpg", 8.59m);
            AddPhoto(context, jack, "MyFirstPhoto", "/blah/folder/file.jpg", 0);

        }
    }
}
