using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using MetadataExtractor;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.OAuth;
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
        private IUnitOfWork _unitOfWork;
        private PhotoShareDbContext _context;
        private readonly IModelFactory _modelFactory;


        public UserController()
        {
            _modelFactory = new ModelFactory();
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
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public PhotoShareDbContext Context
        {
            get
            {
                return _context ?? Request.GetOwinContext().Request.Context.Get<PhotoShareDbContext>();
            }
            private set
            {
                _context = value;
            }
        }
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork ?? new UnitOfWork(Context);
            }
            private set
            {
                _unitOfWork = value;
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
            bool awaitingAdminConfirmation = model.Photographer;
            var defaultRole = "shopper";
            
            var user = new User() { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber, AwaitingAdminConfirmation = awaitingAdminConfirmation };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            await UserManager.AddToRoleAsync(user.Id, defaultRole);

            //This should log the user in once registered. Don't have the time to see why it doesn't work
            /*ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
               OAuthDefaults.AuthenticationType);
            var authProps = new Dictionary<string, string>
            {
                {"userName", user.UserName},
                {"firstName", user.FirstName},
                {"role", defaultRole}
            };
            AuthenticationProperties properties = new AuthenticationProperties(authProps);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);

            var token = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);*/

            var email = new Email
            {
                ConfirmationCode = UserManager.GenerateEmailConfirmationToken(user.Id),
                Recipient = user.Email
            };

            await email.Send();
            return Ok();
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

        [Route("ChangeEmail")]
        public async Task<IHttpActionResult> ChangeEmail(ChangeEmailBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user.Email != model.NewEmail)
            {
                user.Email = model.NewEmail;
                if (UnitOfWork.Save() == 0)
                {
                    return InternalServerError();
                }
            }


            var email = new Email
            {
                ConfirmationCode = UserManager.GenerateEmailConfirmationToken(user.Id),
                Recipient = user.Email
            };

            await email.Send();

            return Ok();
        }

        [Route("ChangeName")]
        public async Task<IHttpActionResult> ChangeName(ChangeNameBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user.FirstName != model.FirstName || user.LastName != model.LastName)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                if (UnitOfWork.Save() == 0)
                {
                    return InternalServerError();
                }
            }

            return Ok();
        }

        [Route("ChangeDescription")]
        public async Task<IHttpActionResult> ChangeDescription(ChangeDescriptionBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user.ProfileDescription != model.Description)
            {
                user.ProfileDescription = model.Description;
                if (UnitOfWork.Save() == 0)
                {
                    return InternalServerError();
                }
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
        public IHttpActionResult GetUserInfoById(string id)
        {
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = _modelFactory.Create(user, false);
            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Name/{username}")]
        public IHttpActionResult GetUserInfoByUsername(string username)
        {
            var user = UserManager.FindByName(username);
            if (user == null)
            {
                return NotFound();
            }

            var model = _modelFactory.Create(user, false);
            return Ok(model);
        }

        public IHttpActionResult GetMyUserInfo()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user == null)
            {
                return NotFound();
            }

            var model = _modelFactory.Create(user, true);
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
                        if (UnitOfWork.Save() == 0)
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
                        if (UnitOfWork.Save() == 0)
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
