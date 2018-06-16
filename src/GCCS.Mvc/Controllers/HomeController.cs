using Microsoft.AspNetCore.Mvc;

namespace GCCS.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}