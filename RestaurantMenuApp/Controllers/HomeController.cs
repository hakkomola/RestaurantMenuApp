using Microsoft.AspNetCore.Mvc;

namespace RestaurantMenuApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
