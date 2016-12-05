using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PhotoShare.DataAccess.Entities
{
    public class User : IdentityUser
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string ProfileDescription { get; set; }
        public virtual string ProfilePhoto { get; set; }
        public virtual string BackgroundPhoto { get; set; }
        public virtual bool AwaitingAdminConfirmation { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}