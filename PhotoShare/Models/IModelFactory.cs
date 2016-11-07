using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoShare.DataAccess.Entities;

namespace PhotoShare.Models
{
    interface IModelFactory
    {
        PhotoModel Create(Photo photo);
        Photo Create(PhotoModel model, User currentUser);
    }
}
