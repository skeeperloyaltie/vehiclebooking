using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace OnlineVehicleRentalSystem.Models
{
    public class User : IdentityUser
    {
        // The 'Name' property is required and initialized with a default value.
        public required string Name { get; set; } = string.Empty;
        
        // Override the 'PasswordHash' property for password storage.
        public override string PasswordHash { get; set; } = string.Empty;

        // Navigation property for the bookings associated with the user.
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
