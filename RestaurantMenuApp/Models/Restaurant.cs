using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantMenuApp.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Password { get; set; } = ""; // Daha sonra hash yapılacak

        public List<MenuItem> MenuItems { get; set; } = new();
    }
}
