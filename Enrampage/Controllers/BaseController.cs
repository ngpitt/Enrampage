using System.Security.Claims;
using System.Web.Mvc;

namespace Enrampage.Controllers
{
    public abstract class BaseController : Controller
    {
        protected ClaimsPrincipal CurrentUser
        {
            get { return (ClaimsPrincipal)User; }
        }
    }
}