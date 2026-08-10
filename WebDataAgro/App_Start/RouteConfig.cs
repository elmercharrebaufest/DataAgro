using System.Web.Mvc;
using System.Web.Routing;

namespace WebDataAgro
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute(".well-known/{*pathInfo}");
            routes.IgnoreRoute("Services/{resource}.svc/{*pathInfo}");
            routes.IgnoreRoute("{resource}.svc/{*pathInfo}");

            // Habilitar Attribute Routing
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
