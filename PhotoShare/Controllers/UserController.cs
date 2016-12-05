using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MetadataExtractor;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PhotoShare.App_Start;
using PhotoShare.DataAccess;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;
using PhotoShare.EmailProvider;
using PhotoShare.ImageHandling;
using PhotoShare.Models;
using PhotoShare.Models.AccountBindingModels;

namespace PhotoShare.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private ApplicationUserManager _userManager;
        private readonly IModelFactory _modelFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PhotoShareDbContext _context = new PhotoShareDbContext();
        public UserController()
        {
            _modelFactory = new ModelFactory();
            _unitOfWork = new UnitOfWork(_context);
        }
        public UserController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
            
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? new ApplicationUserManager(new UserStore<User>(_context));
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Check if the user wants to be registered as a photographer
            //users must wait on admin confirmation before being assigned
            //photographer role
            bool awaitingAdminConfirmation = model.Photographer == "true";
            
            var user = new User() { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber, AwaitingAdminConfirmation = awaitingAdminConfirmation };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            await UserManager.AddToRoleAsync(user.Id, "shopper");

            var email = new Email
            {
                ConfirmationCode = UserManager.GenerateEmailConfirmationToken(user.Id),
                Recipient = user.Email
            };

            await email.Send();
            return Ok("Email sent");
        }

        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [Route("EmailConfirm")]
        public IHttpActionResult EmailConfirm(string code)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var result = UserManager.ConfirmEmail(user.Id, code);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.ToString());
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetUserInfo(string id)
        {
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = _modelFactory.Create(user);
            return Ok(model);
        }

        [Route("ProfilePhoto")]
        [HttpPost]
        public IHttpActionResult AddProfilePhoto()
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

                    var profileName = $"profile{Path.GetExtension(fileName)}";
                    var thumbnail = new ImageResize().ToJpg(fileStream, 200, 200);

                    var blobHandler = new BlobHandler(currentUser.UserName);
                    var uri = blobHandler.Upload(thumbnail, profileName);

                    if (currentUser.ProfilePhoto != uri)
                    {
                        currentUser.ProfilePhoto = uri;
                        if (_unitOfWork.Save() == 0)
                        {
                            return InternalServerError();
                        }
                    }

                    return Ok(currentUser.ProfilePhoto);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("BackgroundPhoto")]
        [HttpPost]
        public IHttpActionResult AddBackgroundPhoto()
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

                    var backgroundName = $"background{Path.GetExtension(fileName)}";
                    var thumbnail = new ImageResize().ToJpg(fileStream, 1920, 1080);

                    var blobHandler = new BlobHandler(currentUser.UserName);
                    var uri = blobHandler.Upload(thumbnail, backgroundName);

                    if (currentUser.BackgroundPhoto != uri)
                    {
                        currentUser.BackgroundPhoto = uri;
                        if (_unitOfWork.Save() == 0)
                        {
                            return InternalServerError();
                        }
                    }

                    return Ok(uri);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region ErrorHandling

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        #endregion

    }
}
