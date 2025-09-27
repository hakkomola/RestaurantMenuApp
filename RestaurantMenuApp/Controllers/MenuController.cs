using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantMenuApp.Data;
using RestaurantMenuApp.Models;
using System.Security.Claims;

namespace RestaurantMenuApp.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public MenuController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private int CurrentRestaurantId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ===============================
        // INDEX - Restoran Sahibi Menüleri (Category bazlı)
        // ===============================
        public IActionResult Index()
        {
            var rid = CurrentRestaurantId();

            // Restorana ait kategoriler ve menüleri yükle
            var categories = _db.Categories
                .Where(c => c.RestaurantId == rid)
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var cat in categories)
            {
                cat.MenuItems = _db.MenuItems
                    .Where(m => m.CategoryId == cat.Id)
                    .ToList();
            }

            return View(categories); // View: IEnumerable<Category>
        }

        // ===============================
        // CREATE MENU ITEM
        // ===============================
        [HttpGet]
        public IActionResult Create()
        {
            var rid = CurrentRestaurantId();
            ViewBag.Categories = _db.Categories
                .Where(c => c.RestaurantId == rid)
                .OrderBy(c => c.Name)
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(MenuItem model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(model);

            var rid = CurrentRestaurantId();
            model.RestaurantId = rid;

            if (imageFile != null && imageFile.Length > 0 && imageFile.ContentType.StartsWith("image/"))
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", rid.ToString());
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                imageFile.CopyTo(stream);
                model.ImageUrl = $"/uploads/{rid}/{fileName}";
            }

            _db.MenuItems.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ===============================
        // EDIT MENU ITEM
        // ===============================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _db.MenuItems.Find(id);
            if (item == null || item.RestaurantId != CurrentRestaurantId()) return Forbid();

            ViewBag.Categories = _db.Categories
                .Where(c => c.RestaurantId == item.RestaurantId)
                .OrderBy(c => c.Name)
                .ToList();

            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(int id, MenuItem model, IFormFile? imageFile)
        {
            var item = _db.MenuItems.Find(id);
            if (item == null || item.RestaurantId != CurrentRestaurantId()) return Forbid();

            item.Title = model.Title;
            item.Description = model.Description;
            item.Price = model.Price;
            item.CategoryId = model.CategoryId;

            if (imageFile != null && imageFile.Length > 0 && imageFile.ContentType.StartsWith("image/"))
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", item.RestaurantId.ToString());
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                imageFile.CopyTo(stream);
                item.ImageUrl = $"/uploads/{item.RestaurantId}/{fileName}";
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ===============================
        // DELETE MENU ITEM
        // ===============================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _db.MenuItems.Find(id);
            if (item == null || item.RestaurantId != CurrentRestaurantId()) return Forbid();
            _db.MenuItems.Remove(item);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ===============================
        // PUBLIC MENU PAGE
        // ===============================
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Public(int restaurantId)
        {
            var restaurant = _db.Restaurants.FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant == null) return NotFound();

            var categories = _db.Categories
                .Where(c => c.RestaurantId == restaurantId)
                .Include(c => c.MenuItems)
                .OrderBy(c => c.Name)
                .ToList();

            ViewBag.RestaurantName = restaurant.Name;
            return View(categories); // View expects IEnumerable<Category>
        }
    }
}
