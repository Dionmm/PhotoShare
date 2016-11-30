using System;
using System.Security.Claims;
using System.Threading;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using PhotoShare.App_Start;
using PhotoShare.DataAccess.Entities;

namespace PhotoShare.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public void Send(string message)
        {
            //This is different 
            var currentUser = Context.User.Identity.Name;
            System.Diagnostics.Debug.WriteLine("ChatBot Begin");

            var b = Context;
            
            Clients.All.addNewMessageToPage(currentUser, message);
        }


    }
}