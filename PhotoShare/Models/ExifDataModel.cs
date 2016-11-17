using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PhotoShare.DataAccess.Entities;

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