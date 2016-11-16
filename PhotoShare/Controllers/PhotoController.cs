using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PhotoShare.App_Start;
using PhotoShare.DataAccess;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;
using PhotoShare.Models;

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
            var models = photos.Select(_modelFactory.Create);

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

            //Generates a GUID + file exntension to be used as the blobName
            var blobName = CreateBlobName(file.FileName);

            //Uploads photo and returns the uri of the uploaded photo
            var uri = blobHandler.Upload(file, blobName);


            //Removes the file extension from the fileName. This
            //photoName is then used when creating the photo below
            var photoName = RemoveFileExtension(file.FileName);

            //Add the Photo to DB
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
        public IHttpActionResult UpdatePhoto(int id, PhotoModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var photo = _unitOfWork.Photos.Get(id);
            if (photo == null)
            {
                return NotFound();
            }

            if (model.UserId != User.Identity.GetUserId() && !User.IsInRole("administrator"))
            {
                return Unauthorized();
            }

            photo.Name = model.Name;
            photo.Price = model.Price;
            photo.UpdatedDateTime = DateTime.Now;

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
