using System.Web.Mvc;

namespace Enrampage.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            TempData["Initialize"] = true;
            ViewBag.Success = TempData["Success"]?.ToString();
            ViewBag.Error = TempData["Error"]?.ToString();
            return View();
        }
    }
}