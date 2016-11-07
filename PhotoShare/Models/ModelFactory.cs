using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhotoShare.DataAccess.Entities;

namespace PhotoShare.Models
{
    public class ModelFactory : IModelFactory
    {
        public PhotoModel Create(Photo photo)
        {
            return new PhotoModel
            {
                Id = photo.Id,
                Name = photo.Name,
                Price = photo.Price,
                Address = photo.Address,
                OptimisedAddress = photo.OptimisedVersionAddress,
                UserId = photo.User.Id,
                UserName = photo.User.UserName
            };
        }

        public Photo Create(PhotoModel model, User currentUser)
        {
            return new Photo
            {
                Name = model.Name,
                Address = "my/url/" + model.Name,
                Price = model.Price,
                User = currentUser,
                CreatedDateTime = DateTime.Now, 
                UpdatedDateTime = DateTime.Now
            };
        }
    }
}