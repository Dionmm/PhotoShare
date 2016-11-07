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
        private readonly ApplicationUserManager _userManager;
        private readonly IModelFactory _modelFactory;
        public PhotoController()
        {
            _unitOfWork = new UnitOfWork(_context);
            _userManager = new ApplicationUserManager(new UserStore<User>(_context));
            _modelFactory = new ModelFactory();
        }

        public IHttpActionResult Get()
        {
            var photos = _unitOfWork.Photos.GetAll();
            var models = photos.Select(_modelFactory.Create);
            
            return Ok(models);
        }
        
        public IHttpActionResult Get(int id)
        {
            var photo = _unitOfWork.Photos.Get(id);
            var model = _modelFactory.Create(photo);

            return Ok(model);
        }
        
        [Authorize(Roles = "administrator,photographer")]
        public IHttpActionResult AddPhoto(PhotoModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User currentUser = _userManager.FindById(User.Identity.GetUserId());
            var photo = _modelFactory.Create(model, currentUser);
            _unitOfWork.Photos.Add(photo);

            if (_unitOfWork.Save() == 0)
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
            var model = _modelFactory.Create(photo);

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
