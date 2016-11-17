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

        private void AddUser(PhotoShareDbContext context, User user, string password, string roleName)
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

        private void AddRole(PhotoShareDbContext context, string roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            
            if (!roleManager.RoleExists(roleName))
            {
                IdentityResult roleResult = roleManager.Create(new IdentityRole(roleName));
            }
        }

        private void AddMessage(PhotoShareDbContext context, User user, string content)
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

        private void AddPhoto(PhotoShareDbContext context, User user, string name, string address, decimal price, string optimisedAddress)
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
                        OptimisedVersionAddress = optimisedAddress,
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
                PhoneNumber = "07881913156",
                FirstName = "Dion",
                LastName = "MacIntyre"
            };
            var jack = new User
            {
                UserName = "JackBlack",
                Email = "jack@macinty.re",
                PhoneNumber = "07881913134"
            };
            const string administrator = "administrator";
            const string photographer = "photographer";

            /*AddRole(context, administrator);
            AddRole(context, photographer);
            AddUser(context, dion, "password", administrator);
            AddUser(context, jack, "password", photographer);
            AddMessage(context, dion, "Testing messages");
            AddMessage(context, jack, "Hey man how's it going");
            AddMessage(context, dion, "nb mate, you");
            AddMessage(context, dion, "ye gg bby");
            AddPhoto(context, dion, "OnTheBeach", "http://placekitten.com/250/300", 4.50m, "http://placekitten.com/250/300");
            AddPhoto(context, dion, "DownThePark", "http://placekitten.com/150/300", 8.59m, "http://placekitten.com/150/300");
            AddPhoto(context, jack, "SomeGirl", "http://placekitten.com/200/350", 0, "http://placekitten.com/200/350");
            AddPhoto(context, dion, "AGuyAtThePub", "http://placekitten.com/400/300", 4.50m, "http://placekitten.com/400/300");
            AddPhoto(context, dion, "Testing", "http://placekitten.com/200/450", 8.59m, "http://placekitten.com/200/450");
            AddPhoto(context, jack, "HaveAnotherOne", "http://placekitten.com/100/300", 0, "http://placekitten.com/100/300");
            AddPhoto(context, dion, "HeilHydra", "http://placekitten.com/500/300", 4.50m, "http://placekitten.com/500/300");
            AddPhoto(context, dion, "AmSexyNAKenIt", "http://placekitten.com/200/300", 8.59m, "http://placekitten.com/200/300");
            AddPhoto(context, jack, "WhatsYourName", "http://placekitten.com/200/330", 0, "http://placekitten.com/200/330");
            AddPhoto(context, dion, "RandomString", "http://placekitten.com/200/600", 4.50m, "http://placekitten.com/200/600");
            AddPhoto(context, dion, "ThisIsFun", "http://placekitten.com/200/200", 8.59m, "http://placekitten.com/200/200");
            AddPhoto(context, jack, "HaveACake", "http://placekitten.com/200/150", 0, "http://placekitten.com/200/150");*/

        }
    }
}
