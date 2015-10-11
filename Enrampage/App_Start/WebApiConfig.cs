using System.Web.Http;
using System.Web.Mvc;

namespace Enrampage
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{action}/{id}",
                defaults: new { controller = "Rant", id = UrlParameter.Optional }
            );
        }
    }
}
