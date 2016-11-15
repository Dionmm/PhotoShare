using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoShare.DataAccess.Entities;

namespace PhotoShare.DataAccess.DataContext
{
    public class PhotoShareDbContext : IdentityDbContext<User>
    {
        public PhotoShareDbContext() : base("DefaultConnection")
        {
            //this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<BannedWord> BannedWords { get; set; }
        public virtual DbSet<ExifData> ExifData { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }

        public static PhotoShareDbContext Create()
        {
            return new PhotoShareDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");

        }
    }
}