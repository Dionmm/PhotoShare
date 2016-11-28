using Microsoft.AspNet.SignalR;

namespace PhotoShare.Hubs
{
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}