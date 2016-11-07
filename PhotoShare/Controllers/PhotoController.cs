using System;
using System.Collections.Generic;
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
        public PhotoController()
        {
            _unitOfWork = new UnitOfWork(_context);
        }

        public IHttpActionResult Get()
        {
            var photos = _unitOfWork.Photos.GetAll();
            var model = new List<PhotoModel>();
            foreach (var photo in photos)
            {
                var temp = new PhotoModel
                {
                    Id = photo.Id,
                    Name = photo.Name,
                    Price = photo.Price,
                    Address = photo.Address,
                    OptimisedAddress = photo.OptimisedVersionAddress,
                    UserId = photo.User.Id,
                    UserName = photo.User.UserName
                };
                model.Add(temp);
            }

            return Ok(model);
        }
        
        public IHttpActionResult Get(int id)
        {
            var photo = _unitOfWork.Photos.Get(id);
            var model = new PhotoModel
            {
                Id = photo.Id,
                Name = photo.Name,
                Price = photo.Price,
                Address = photo.Address,
                OptimisedAddress = photo.OptimisedVersionAddress,
                UserId = photo.User.Id,
                UserName = photo.User.UserName
            };


            return Ok(model);
        }
        
        [Authorize(Roles = "administrator,photgrapher")]
        public IHttpActionResult AddPhoto(PhotoModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var userManager = new ApplicationUserManager(new UserStore<User>(_context));
            User currentUser = userManager.FindById(User.Identity.GetUserId());
            var photo = new Photo
            {
                Name = model.Name,
                Address = "my/url/" + model.Name,
                Price = model.Price,
                User = currentUser,
                CreatedDateTime = DateTime.Now,
                UpdatedDateTime = DateTime.Now
            };
            _context.Photos.Add(photo);
            if (_context.SaveChanges() == 0)
            {
                return InternalServerError();
            }
            return Ok("Photo Saved");
        }

        [Authorize(Roles = "administrator, photographer")]
        [HttpDelete]
        public IHttpActionResult DeletePhoto(int id)
        {
            var photo = _unitOfWork.Photos.Get(id);
            var model = new PhotoModel
            {
                Name = photo.Name,
                Price = photo.Price,
                UserId = photo.User.Id,
                UserName = photo.User.UserName
            };

            if (model.UserId != User.Identity.GetUserId())
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
    }
}
