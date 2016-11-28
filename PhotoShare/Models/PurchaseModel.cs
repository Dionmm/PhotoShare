using System.ComponentModel.DataAnnotations;

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