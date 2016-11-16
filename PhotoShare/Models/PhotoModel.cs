using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PhotoShare.DataAccess.Entities;

namespace PhotoShare.Models
{
    public class PhotoModel
    {
        public HttpPostedFile File { get; set; }
        [Display(Name = "Id")]
        public int Id { get; set; }
        //[Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Optimised Address")]
        public string OptimisedAddress { get; set; }

        [Display(Name = "UserId")]
        public string UserId { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }

    }
}