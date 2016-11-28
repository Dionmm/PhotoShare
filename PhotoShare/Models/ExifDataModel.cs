using System.ComponentModel.DataAnnotations;

namespace PhotoShare.Models
{
    public class ExifDataModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Value")]
        public string Value { get; set; }

    }
}