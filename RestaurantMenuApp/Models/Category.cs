namespace RestaurantMenuApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }  // Her restoranın kendi kategorisi olabilir
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        // Navigation property
        public ICollection<MenuItem>? MenuItems { get; set; }
    }
}