using System.Collections.Generic;
using PhotoShare.DataAccess.Entities;
using PhotoShare.Models.AdminModels;
using PhotoShare.Models.PhotoModels;

namespace PhotoShare.Models
{
    interface IModelFactory
    {
        SinglePhotoModel Create(Photo photo);
        Photo Create(SinglePhotoModel model, User currentUser);
        IEnumerable<MultiPhotoModel> Create(IEnumerable<Photo> photos);
        UserModel Create(User user);
        IEnumerable<AdminUserModel> Create(IEnumerable<User> users);
        ExifData Create(ExifDataModel model, Photo photo);
        ExifDataModel Create(ExifData exifData);
        Purchase Create(PurchaseModel model);
        MessageModel Create(Message message);
    }
}
