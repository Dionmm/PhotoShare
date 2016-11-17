using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.Provider;
using PhotoShare.App_Start;
using PhotoShare.DataAccess;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;
using PhotoShare.Models;
using Stripe;

namespace PhotoShare.Controllers
{
    [RoutePrefix("api/Purchase")]
    [Authorize]
    public class PurchaseController : ApiController
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly PhotoShareDbContext _context = new PhotoShareDbContext();
        private readonly ApplicationUserManager _userManager;
        private readonly IModelFactory _modelFactory;

        public PurchaseController()
        {
            _unitOfWork = new UnitOfWork(_context);
            _userManager = new ApplicationUserManager(new UserStore<User>(_context));
            _modelFactory = new ModelFactory();
        }

        [HttpPost]
        public IHttpActionResult PurchasePhoto(int id, string token)
        {
            var photo = _unitOfWork.Photos.Get(id);
            if (photo == null)
            {
                return NotFound();
            }

            var currentUser = _userManager.FindById(User.Identity.GetUserId());
            if (currentUser == null)
            {
                return InternalServerError();
            }

            if (CreateStripeCharge(photo.Price, photo.Name, token) != "succeeded")
            {
                return BadRequest();
            }

            if (AddPurchase(photo, currentUser) == 0)
            {
                return InternalServerError();
            }

            return Ok("Payment Successful");
        }

        #region Helpers

        private static string CreateStripeCharge(decimal amount, string photoName, string token, string currency = "gbp")
        {
            var charge = new StripeChargeCreateOptions
            {
                Amount = Convert.ToInt32(amount * 100),
                Description = "Purchase of " + photoName,
                SourceTokenOrExistingSourceId = token,
                Currency = currency
            };

            var chargeService = new StripeChargeService();
            var stripeCharge = chargeService.Create(charge);

            //Can return 'succeeded', 'pending' or 'failed'
            return stripeCharge.Status;
        }

        private int AddPurchase(Photo photo, User user)
        {
            var purchase = new Purchase
            {
                Price = photo.Price,
                CreatedDateTime = DateTime.Now,
                UpdatedDateTime = DateTime.Now,
                Photo = photo,
                User = user
            };
            _unitOfWork.Purchases.Add(purchase);
            return _unitOfWork.Save();
        }
        #endregion

    }
}
