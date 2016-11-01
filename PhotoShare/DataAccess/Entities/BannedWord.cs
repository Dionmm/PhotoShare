using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoShare.DataAccess.Entities
{
    public class BannedWord :BaseEntity
    {
        public string Word { get; set; }
    }
}