using Microsoft.AspNetCore.Mvc;

namespace HandballResults.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
