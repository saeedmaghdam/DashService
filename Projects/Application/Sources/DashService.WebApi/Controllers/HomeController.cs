using System.Web.Http;

namespace DashService.WebApi.Controllers
{
    public class HomeController : ApiController
    {
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public string Index()
        {
            return "Index";
        }
    }
}
