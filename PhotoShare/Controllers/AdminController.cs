using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using PhotoShare.App_Start;
using PhotoShare.DataAccess;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;
using PhotoShare.Models;

namespace PhotoShare.Controllers
{
    [Authorize(Roles = "administrator")]
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PhotoShareDbContext _context = new PhotoShareDbContext();
        private readonly ApplicationUserManager _userManager;
        private readonly IModelFactory _modelFactory;

        public AdminController()
        {
            _unitOfWork = new UnitOfWork(_context);
            _userManager = new ApplicationUserManager(new UserStore<User>(_context));
            _modelFactory = new ModelFactory();
        }

        public IHttpActionResult Get()
        {
            var users = _userManager.Users.ToList();
            var userModels = users.Select(_modelFactory.Create);
            
            return Ok(userModels);
        }

    }
}
