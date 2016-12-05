using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhotoShare.Models.AdminModels
{
    public class ChangeUserRoleModel
    {
        [Required]
        public string UserName { get; set; }
        public string UserId { get; set; }
        [Required]
        public string Role { get; set; }
    }
}