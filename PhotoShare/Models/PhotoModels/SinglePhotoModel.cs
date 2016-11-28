using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhotoShare.Models.PhotoModels
{
    public class SinglePhotoModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

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

        [Display(Name = "ExifCollection")]
        public IEnumerable<ExifDataModel> Exif { get; set; }

    }
}