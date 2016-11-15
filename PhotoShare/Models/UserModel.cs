using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
    }
}