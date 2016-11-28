﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MetadataExtractor;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PhotoShare.App_Start;
using PhotoShare.DataAccess;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;
using PhotoShare.Models;
using PhotoShare.Models.PhotoModels;
using Directory = MetadataExtractor.Directory;

namespace PhotoShare.Controllers
{
    [RoutePrefix("api/Photo")]
    public class PhotoController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PhotoShareDbContext _context = new PhotoShareDbContext();
        private ApplicationUserManager _userManager;
        private readonly IModelFactory _modelFactory;

        public PhotoController()
        {
            _unitOfWork = new UnitOfWork(_context);
            _modelFactory = new ModelFactory();
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /*GET Requests*/

        public IHttpActionResult Get()
        {
            var photos = _unitOfWork.Photos.GetAll();
            var models = _modelFactory.Create(photos);

            return Ok(models);
        }

        public IHttpActionResult Get(int id)
        {
            var photo = _unitOfWork.Photos.Get(id);
            if (photo == null)
            {
                return NotFound();
            }
            var model = _modelFactory.Create(photo);

            return Ok(model);
        }

        /*POST Requests*/

        /* This is a complete hack due to ASP no longer binding
         * HttpPostedFileBase to a model when passed as a parameter,
         * instead it returns a 415 'Unsupported media type'. Without the knowledge
         * of how to write a custom media formatter for images, this is the fallback.
         * So, to bypass this the client will first POST the image file to AddPhotoFile() 
         * where it is saved to the Azure Storage Account and a Db entry will be made
         * with the URI and user's details (accessed through Identity). Once the
         * client receives a success callback a second PUT will be made to UpdatePhoto()
         * with the extra data (name, price, exif data, etc.).
         */

        [Authorize(Roles = "administrator,photographer")]
        [HttpPost]
        public IHttpActionResult AddPhotoFile()
        {
            try
            {
                using (var fileStream = HttpContext.Current.Request.Files[0].InputStream)
                {
                    var fileName = HttpContext.Current.Request.Files[0].FileName;
                    User currentUser = UserManager.FindById(User.Identity.GetUserId());

                    if (currentUser == null)
                    {
                        return InternalServerError();
                    }
                    

                    var blobHandler = new BlobHandler(currentUser.UserName);

                    //Reads the meta data attached to the photo.
                    //This must go after the blob upload as this appears to alter the file input stream
                    IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(fileStream);

                    //Creates an optimised thumbnail for the image
                    var thumbnail = CreateThumbnail(fileStream);

                    //Generates a GUID + file extension to be used as the blobName
                    var blobName = CreateBlobName(fileName);


                    //Uploads photo and thumbnail to the user's container and returns the uris
                    var uri = blobHandler.Upload(fileStream, blobName["Original"]);
                    var thumbNailUri = blobHandler.Upload(thumbnail, blobName["Thumbnail"]);


                    //Removes the file extension from the fileName. This
                    //photoName is then used when creating the photo below
                    var photoName = RemoveFileExtension(fileName);


                    //Add the Photo to DB
                    //Doesn't use ModelFactory because i would need to create a new PhotoModel anyway
                    //which is similarly slow and messy
                    var photo = new Photo
                    {
                        Name = photoName,
                        Address = uri,
                        OptimisedVersionAddress = thumbNailUri,
                        User = currentUser,
                        CreatedDateTime = DateTime.Now,
                        UpdatedDateTime = DateTime.Now
                    };
                    _unitOfWork.Photos.Add(photo);

                    //Extracts the metadata for the photo and 
                    //creates a new ExifData for each, then saved
                    //to DB
                    foreach (var directory in directories)
                    {
                        foreach (var tag in directory.Tags)
                        {
                            var exifModel = new ExifDataModel
                            {
                                Name = tag.Name,
                                Value = tag.Description
                            };
                            _unitOfWork.ExifData.Add(_modelFactory.Create(exifModel, photo));
                        }
                    }


                    if (_unitOfWork.Save() == 0)
                    {
                        return InternalServerError();
                    }

                    var model = _modelFactory.Create(photo);

                    return Ok(model);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /*PUT Requests*/

        [Authorize(Roles = "administrator,photographer")]
        [HttpPut]
        public IHttpActionResult UpdatePhoto(int id, SinglePhotoModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (model.UserId != User.Identity.GetUserId() && !User.IsInRole("administrator"))
            {
                return Unauthorized();
            }

            var photo = _unitOfWork.Photos.Get(id);
            if (photo == null)
            {
                return NotFound();
            }

            photo.Name = model.Name;
            photo.Price = model.Price;
            photo.UpdatedDateTime = DateTime.Now;
            if (model.Exif != null)
            {
                foreach (var exifModel in model.Exif)
                {
                    var exifData = _modelFactory.Create(exifModel, photo);
                    _unitOfWork.ExifData.Add(exifData);
                }
            }
            

            _unitOfWork.Save();
            
            return Ok("Photo Updated");
        }

        /*DELETE Requests*/

        [Authorize(Roles = "administrator, photographer")]
        [HttpDelete]
        public IHttpActionResult DeletePhoto(int id)
        {
            var photo = _unitOfWork.Photos.Get(id);
            var model = _modelFactory.Create(photo);

            if (model.UserId != User.Identity.GetUserId() && !User.IsInRole("administrator"))
            {
                return Unauthorized();
            }

            _unitOfWork.Photos.Remove(photo);
            if (_unitOfWork.Save() == 0)
            {
                return InternalServerError();
            }
            
            return Ok("Photo Removed");
        }


        #region Helpers

        private static string RemoveFileExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        private static IDictionary<string, string> CreateBlobName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            var guid = Guid.NewGuid();
            return new Dictionary<string, string>
            {
                {"Original", $"{guid}{extension}"},
                {"Thumbnail", $"{guid}-thumbnail{extension}"}
            };
        }

        private static Stream CreateThumbnail(Stream fileStream)
        {
            //The file will be corrupted if not read from the beginning
            fileStream.Position = 0;
            using (var image = Image.FromStream(fileStream))
            {
                Stream outputStream = new MemoryStream();
                double width;
                double height;

                if (image.Width > image.Height)
                {
                    width = 500;
                    var ratio = image.Width / width;
                    height = image.Height / ratio;
                }
                else
                {
                    height = 500;
                    var ratio = image.Height / height;
                    width = image.Width / ratio;
                }

                var newImage = new Bitmap(Convert.ToInt32(width), Convert.ToInt32(height));
                var rectangle = new Rectangle(0, 0, Convert.ToInt32(width), Convert.ToInt32(height));

                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    
                    graphics.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                }
                ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

                foreach (var codec in encoders)
                {
                    if (codec.MimeType != "image/jpeg") continue;

                    var encoderParameters = new EncoderParameters
                    {
                        Param = { [0] = new EncoderParameter(Encoder.Quality, 90L) }
                    };

                    newImage.Save(outputStream, codec, encoderParameters);
                }

                return outputStream;
            }
        }

        #endregion
    }
}
