using System;
using System.ComponentModel.DataAnnotations;

namespace PhotoShare.Models.AdminModels
{
    public class AdminUserModel
    {
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Profile Photo")]
        public string ProfilePhoto { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Phone Number Confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "Two Factor Enabled")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "Lockout Enabled")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Lockout End Date")]
        public DateTime LockoutEndDate { get; set; }

        [Display(Name = "Access Failed Count")]
        public string AccessFailedCount { get; set; }

        [Display(Name = "Awaiting Admin Confirmation")]
        public bool AwaitingAdminConfirmation { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}