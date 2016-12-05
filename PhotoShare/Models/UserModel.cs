using System.ComponentModel.DataAnnotations;

namespace PhotoShare.Models
{
    public class UserModel
    {
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Display(Name = "ProfileDescription")]
        public string ProfileDescription { get; set; }

        [Display(Name = "ProfilePhoto")]
        public string ProfilePhoto { get; set; }

        public string BackgroundPhoto { get; set; }

        public string Email { get; set; }
    }
}