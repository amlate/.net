using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AiJiaXi.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "AiJiaXi.Web.Controllers" });

            routes.MapRoute(
              name: "Baixime",
              url: "{controller}/{action}/{cityIdParm}/{countyIdParm}/{type}/{OpenId}",
              defaults: new { controller = "Baixime", action = "Index", cityId = UrlParameter.Optional, countyId= UrlParameter.Optional, type = UrlParameter.Optional, OpenId = UrlParameter.Optional });


            routes.MapRoute(
            name: "Other",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Other", action = "index", id = UrlParameter.Optional });


        }
    }
}
