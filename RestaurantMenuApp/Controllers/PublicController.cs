using Microsoft.AspNetCore.Mvc;
using RestaurantMenuApp.Data;

namespace RestaurantMenuApp.Controllers
{
    public class PublicController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PublicController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("/menu/{restaurantId}")]
        public IActionResult Menu(int restaurantId)
        {
            var restaurant = _db.Restaurants.FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant == null) return NotFound();

            var categories = _db.Categories
                .Where(c => c.RestaurantId == restaurantId)
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var cat in categories)
            {
                cat.MenuItems = _db.MenuItems
                    .Where(m => m.CategoryId == cat.Id)
                    .ToList();
            }

            ViewBag.RestaurantName = restaurant.Name;
            return View(categories); // View: List<Category>
        }
    }
}
