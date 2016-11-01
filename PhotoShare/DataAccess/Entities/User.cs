using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PhotoShare.DataAccess.Entities
{
    public class User : IdentityUser
    {
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}