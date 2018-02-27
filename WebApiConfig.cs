using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace RedditSolution
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "GetLogin",
                routeTemplate: "api/{controller}/{username} {password}"
            );

            config.Routes.MapHttpRoute(
                name: "GetListings",
                routeTemplate: "api/{controller}/Reddit/{userToken}",
                defaults: new { action = "GetListings" }
            );

            config.Routes.MapHttpRoute(
                name: "GetFavorites",
                routeTemplate: "api/{controller}/User/{userToken}",
                defaults: new { action = "GetFavorites" }
            );

            config.Routes.MapHttpRoute(
                name: "GetRegisteredToken",
                routeTemplate: "api/{controller}/Register/{username} {password}",
                defaults: new { action = "GetRegisteredToken" }
            );

            config.Routes.MapHttpRoute(
                name: "GetSavedResponse",
                routeTemplate: "api/{controller}/FavoriteItem/{accessToken} {reddit_id} {tag}",
                defaults: new { action = "GetSavedResponse" }
            );

        }
    }
}
