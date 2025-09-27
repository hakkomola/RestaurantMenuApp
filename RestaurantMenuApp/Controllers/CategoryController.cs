using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantMenuApp.Data;
using RestaurantMenuApp.Models;
using System.Security.Claims;

namespace RestaurantMenuApp.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public CategoryController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private int CurrentRestaurantId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string name, IFormFile? imageFile)
        {
            var category = new Category
            {
                RestaurantId = CurrentRestaurantId(), // 🔒 Formdan değil, login’den al
                Name = name
            };

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "categories");
                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                imageFile.CopyTo(stream);

                category.ImageUrl = "/uploads/categories/" + fileName;
            }

            _db.Categories.Add(category);
            _db.SaveChanges();

            return RedirectToAction("Index", "Menu");
        }
    }
}
