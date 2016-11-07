using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhotoShare.DataAccess.Entities
{
    public class Photo : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string OptimisedVersionAddress { get; set; }
        public decimal Price { get; set; }
        [Required]
        public virtual User User { get; set; }
        //public virtual ICollection<ExifData> ExifData { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}