﻿using System.Web.Http;

namespace PhotoShare
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name:"Other",
                routeTemplate: "{*anything}",
                defaults: new {controller = "Home", action = "Index"}
            );
        }
    }
}
