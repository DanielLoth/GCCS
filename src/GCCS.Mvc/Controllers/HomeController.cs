using Microsoft.AspNetCore.Mvc;

namespace GCCS.Mvc.Controllers
{
    [Route("mvc")]
    public class HomeController : Controller
    {
        [Route("hello")]
        public IActionResult Index()
        {
            return View();
        }
    }
}