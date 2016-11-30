using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using PhotoShare.App_Start;

namespace PhotoShare.Providers
{
    public class OwinMiddleWareQueryStringExtractor: OwinMiddleware
    {
        public OwinMiddleWareQueryStringExtractor(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            System.Diagnostics.Debug.WriteLine("MiddleWare Begin " + context.Request.Path.Value);


            if (context.Request.Path.Value.StartsWith("/signalr"))
            {
                string bearerToken = context.Request.Query.Get("bearer_token");
                var x = context;
                var um = HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                if (bearerToken != null)
                {
                    var tk = Startup.OAuthOptions.AccessTokenFormat.Unprotect(bearerToken);
                    var principal = new ClaimsPrincipal(tk.Identity);
                    Thread.CurrentPrincipal = principal;
                    context.Request.User = principal;

                }
            }

            await Next.Invoke(context);
        }
    }
}