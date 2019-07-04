﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace FileServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(

                name: "FileRoute",

                routeTemplate: "api/{controller}/{*filename}",

                defaults: new { filename = RouteParameter.Optional }

            );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
