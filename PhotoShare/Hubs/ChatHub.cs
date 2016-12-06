using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using PhotoShare.App_Start;
using PhotoShare.DataAccess;
using PhotoShare.DataAccess.DataContext;
using PhotoShare.DataAccess.Entities;
using PhotoShare.Models;

namespace PhotoShare.Hubs
{
    [Authorize(Roles = "administrator,photographer")]
    public class ChatHub : Hub
    {

        private ApplicationUserManager _userManager;
        private User _currentUser;
        private PhotoShareDbContext _context = new PhotoShareDbContext();
        private IUnitOfWork _unitOfWork;
        private IModelFactory _modelFactory = new ModelFactory();

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? new ApplicationUserManager(new UserStore<User>(_context)); }
            set { _userManager = value; }
        }

        public User CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }

        public override Task OnConnected()
        {
            try
            {
                var username = Context.User.Identity.Name;
                if (username != null)
                {
                    _unitOfWork = new UnitOfWork(_context);
                    CurrentUser = UserManager.FindByName(username);
                    var messages = _unitOfWork.Messages.GetMostRecentMessages(10);
                    var models = messages.Select(_modelFactory.Create);
                    Clients.Caller.loadMessages(models);
                    Clients.All.addNewMessage("Server", $"{CurrentUser.UserName} has joined the chat");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No username");
                    Clients.Caller.error("Username not found");
                }

            }
            catch (Exception)
            {

                throw;
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            try
            {
                var username = Context.User.Identity.Name;
                if (username != null)
                {
                    CurrentUser = UserManager.FindByName(username);

                    Clients.All.addNewMessage("Server", $"{CurrentUser.UserName} has left the chat");
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            try
            {
                var username = Context.User.Identity.Name;
                if (username != null)
                {
                    _unitOfWork = new UnitOfWork(_context);
                    CurrentUser = UserManager.FindByName(username);
                    var messages = _unitOfWork.Messages.GetMostRecentMessages(10);
                    var models = messages.Select(_modelFactory.Create);
                    Clients.Caller.loadMessages(models);
                    Clients.All.addNewMessage("Server", $"{CurrentUser.UserName} has reconnected");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No username");
                    Clients.Caller.error("Username not found");
                }

            }
            catch (Exception)
            {

                throw;
            }

            return base.OnReconnected();
        }

        public void Send(string message)
        {
            System.Diagnostics.Debug.WriteLine("ChatBot Begin");

            try
            {
                var username = Context.User.Identity.Name;
                if (username != null)
                {
                    CurrentUser = UserManager.FindByName(username);
                    Clients.All.addNewMessage(CurrentUser.UserName, message);
                    _unitOfWork = new UnitOfWork(_context);
                    _unitOfWork.Messages.Add(new Message
                    {
                        Content = message,
                        Hidden = false,
                        CreatedDateTime = DateTime.Now,
                        UpdatedDateTime = DateTime.Now,
                        User = CurrentUser
                    });
                    if (_unitOfWork.Save() == 0)
                    {
                        Clients.All.error("Something went wrong");
                    }

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No username");
                    Clients.Caller.error("Username not found");

                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void HideMessage(int messageId)
        {
            System.Diagnostics.Debug.WriteLine("ChatBot Begin");

            try
            {
                var username = Context.User.Identity.Name;
                if (username != null)
                {
                    CurrentUser = UserManager.FindByName(username);
                    var roles = UserManager.GetRoles(CurrentUser.Id);
                    var role = roles.ToArray();
                    if (role[0] == "administrator")
                    {
                        _unitOfWork = new UnitOfWork(_context);
                        var message = _unitOfWork.Messages.Get(messageId);
                        if (message != null)
                        {
                            message.Hidden = true;
                            if (_unitOfWork.Save() == 0)
                            {
                                Clients.All.error("Something went wrong");
                            }
                            else
                            {
                                Clients.All.removeMessage(messageId);
                            }
                        }
                        
                    }
                    else
                    {
                        Clients.Caller.error("Not Authorised");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No username");
                    Clients.Caller.error("Username not found");

                }
            }
            catch (Exception)
            {
                Clients.Caller.error("Something went wrong on the server");
            }
        }


    }
}