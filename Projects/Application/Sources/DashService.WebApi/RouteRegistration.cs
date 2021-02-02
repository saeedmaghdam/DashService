using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DashService.WebApi
{
    public class RouteRegistration
    {
        public static void RegisterRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(name: "default", "{controller}/{action}/{id?}", defaults: new { controller = "Home", action = "Index" });
            endpoints.MapControllerRoute(name: "default", "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
