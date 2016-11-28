using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using MetadataExtractor;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoShare.App_Start;
using PhotoShare.DataAccess;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;
using PhotoShare.Models;
using PhotoShare.Models.PhotoModels;

namespace PhotoShare.Controllers
{
    [RoutePrefix("api/Photo")]
    public class PhotoController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PhotoShareDbContext _context = new PhotoShareDbContext();
        private readonly ApplicationUserManager _userManager;
        private readonly IModelFactory _modelFactory;

        public PhotoController()
        {
            _unitOfWork = new UnitOfWork(_context);
            _userManager = new ApplicationUserManager(new UserStore<User>(_context));
            _modelFactory = new ModelFactory();
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

        [Route("ByName/{name}")]
        public IHttpActionResult GetByName(string name)
        {
            var photo = _unitOfWork.Photos.GetPhotoByName(name);
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
            if (HttpContext.Current.Request.Files.Count <= 0)
            {
                return BadRequest();
            }

            User currentUser = _userManager.FindById(User.Identity.GetUserId());
            if (currentUser == null)
            {
                return InternalServerError();
            }

            var file = HttpContext.Current.Request.Files[0];
            var blobHandler = new BlobHandler();

            //Reads the meta data attached to the photo.
            //This must go after the blob upload as this appears to alter the file input stream
            IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(file.InputStream);
            

            //Generates a GUID + file extension to be used as the blobName
            var blobName = CreateBlobName(file.FileName);

            //Uploads photo and returns the uri of the uploaded photo
            var uri = blobHandler.Upload(file, blobName);

            

            //Removes the file extension from the fileName. This
            //photoName is then used when creating the photo below
            var photoName = RemoveFileExtension(file.FileName);

            //Add the Photo to DB
            //Doesn't use ModelFactory because i would need to create a new PhotoModel anyway
            //which is similarly slow and messy
            var photo = new Photo
            {
                Name = photoName,
                Address = uri,
                OptimisedVersionAddress = uri, //Temporary until images can be optimised
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
            return System.IO.Path.GetFileNameWithoutExtension(fileName);
        }

        private static string CreateBlobName(string fileName)
        {
            string extension = System.IO.Path.GetExtension(fileName);
            return Guid.NewGuid() + extension;
        }
        
        #endregion
    }
}
