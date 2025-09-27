using System.ComponentModel.DataAnnotations;

namespace RestaurantMenuApp.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        [Required]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }

        public int CategoryId { get; set; }        // Yeni alan
        public Category? Category { get; set; }   // Navigation property

    }
}
