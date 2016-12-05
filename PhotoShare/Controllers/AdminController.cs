using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PhotoShare.App_Start;
using PhotoShare.DataAccess;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.Models;
using PhotoShare.Models.AdminModels;

namespace PhotoShare.Controllers
{
    [Authorize(Roles = "administrator")]
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        private  IUnitOfWork _unitOfWork;
        private  PhotoShareDbContext _context;
        private ApplicationUserManager _userManager;
        private readonly IModelFactory _modelFactory;

        public AdminController()
        {
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

        public IHttpActionResult Get()
        {
            var users = UserManager.Users.ToList();
            var userModels = _modelFactory.Create(users);

            return Ok(userModels);
        }

        public async Task<IHttpActionResult> ChangeUserRole(ChangeUserRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return BadRequest("No user found");
            }

            var roles = await UserManager.GetRolesAsync(user.Id);
            if (roles.Count != 0)
            {
                var role = roles.ToArray();
                var deletionResult = await UserManager.RemoveFromRolesAsync(user.Id, role);
                if (!deletionResult.Succeeded)
                {
                    return InternalServerError();
                }
            }

            var addRoleResult = await UserManager.AddToRoleAsync(user.Id, model.Role);
            if (!addRoleResult.Succeeded)
            {
                return InternalServerError();
            }

            return Ok("Added to role");
        }

    }
}
