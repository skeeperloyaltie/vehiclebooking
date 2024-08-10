using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineVehicleRentalSystem.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        public string Year { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
