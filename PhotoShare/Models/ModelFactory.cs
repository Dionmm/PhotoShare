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


        public UserModel Create(User user)
        {
            return new UserModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfileDescription = user.ProfileDescription,
                ProfilePhoto = user.ProfilePhoto
            };
        }


        public ExifData Create(ExifDataModel model)
        {
            return new ExifData
            {
                ExifName = model.Name,
                ExifValue = model.Value,
                Photo = model.Photo,
                CreatedDateTime = DateTime.Now,
                UpdatedDateTime = DateTime.Now
            };
        }

        public ExifDataModel Create(ExifData exifData)
        {
            return new ExifDataModel
            {
                Name = exifData.ExifName,
                Value = exifData.ExifValue,
                Photo = exifData.Photo
            };
        }

    }
}