using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhotoShare.Models
{
    public class PurchaseModel
    {

        public string PhotoId { get; set; }
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Token", Description = "Token generated in the Stripe charge request")]
        public string Token { get; set; }
    }
}