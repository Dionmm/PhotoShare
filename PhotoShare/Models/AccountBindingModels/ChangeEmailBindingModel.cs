using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhotoShare.Models.AccountBindingModels
{
    public class ChangeEmailBindingModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; set; }
        
    }
}