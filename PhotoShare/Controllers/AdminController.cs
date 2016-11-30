using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using PhotoShare.App_Start;
using PhotoShare.DataAccess;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.Models;

namespace PhotoShare.Controllers
{
    [Authorize(Roles = "administrator")]
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PhotoShareDbContext _context = new PhotoShareDbContext();
        private ApplicationUserManager _userManager;
        private readonly IModelFactory _modelFactory;

        public AdminController()
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

        public IHttpActionResult Get()
        {
            var users = UserManager.Users.ToList();
            var userModels = _modelFactory.Create(users);

            return Ok(userModels);
        }

    }
}
