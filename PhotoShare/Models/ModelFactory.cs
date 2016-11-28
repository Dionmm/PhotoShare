using System;
using System.Collections.Generic;
using System.Linq;
using PhotoShare.DataAccess.Entities;
using PhotoShare.Models.PhotoModels;

namespace PhotoShare.Models
{
    public class ModelFactory : IModelFactory
    {
        public SinglePhotoModel Create(Photo photo)
        {
            ICollection<ExifDataModel> collection = new List<ExifDataModel>();
            if (photo.ExifData != null)
            {
                foreach (var exif in photo.ExifData)
                {
                    collection.Add(Create(exif));
                }
            }
            

            return new SinglePhotoModel
            {
                Id = photo.Id,
                Name = photo.Name,
                Price = photo.Price,
                Address = photo.Address,
                OptimisedAddress = photo.OptimisedVersionAddress,
                UserId = photo.User.Id,
                UserName = photo.User.UserName,
                Exif = collection
            };
        }

        public Photo Create(SinglePhotoModel model, User currentUser)
        {
            return new Photo
            {
                Name = model.Name,
                Address = model.Address,
                OptimisedVersionAddress = model.OptimisedAddress,
                Price = model.Price,
                User = currentUser,
                CreatedDateTime = DateTime.Now,
                UpdatedDateTime = DateTime.Now
            };
        }

        public IEnumerable<MultiPhotoModel> Create(IEnumerable<Photo> photos)
        {

            return photos.Select(n => new MultiPhotoModel
            {
                Id = n.Id,
                Address = n.Address,
                Name = n.Name,
                OptimisedAddress = n.OptimisedVersionAddress,
                Price = n.Price,
                UserId = n.User.Id,
                UserName = n.User.UserName,
                UserFirstName = n.User.FirstName,
                UserLastName = n.User.LastName
            });;
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


        public ExifData Create(ExifDataModel model, Photo photo)
        {
            return new ExifData
            {
                ExifName = model.Name,
                ExifValue = model.Value,
                Photo = photo,
                CreatedDateTime = DateTime.Now,
                UpdatedDateTime = DateTime.Now
            };
        }

        public ExifDataModel Create(ExifData exifData)
        {
            return new ExifDataModel
            {
                Name = exifData.ExifName,
                Value = exifData.ExifValue
            };
        }

        public Purchase Create(PurchaseModel model)
        {
            throw new NotImplementedException();
        }
    }
}