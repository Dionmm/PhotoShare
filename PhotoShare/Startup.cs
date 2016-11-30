using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using PhotoShare.Hubs;
using PhotoShare.Providers;

[assembly: OwinStartup(typeof(PhotoShare.Startup))]

namespace PhotoShare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            System.Diagnostics.Debug.WriteLine("Begin Init");

            app.Use(typeof(OwinMiddleWareQueryStringExtractor));

            System.Diagnostics.Debug.WriteLine("End Init");

            //This is used to server the index.html file in the project root
            const string rootFolder = ".";
            var fileSystem = new PhysicalFileSystem(rootFolder);
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = fileSystem
            };

            app.UseFileServer(options);


            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.MapSignalR();
            app.UseCors(CorsOptions.AllowAll);
            ConfigureAuth(app);
            app.UseWebApi(config);
        }
    }
}