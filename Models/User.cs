using Microsoft.AspNetCore.Identity;


namespace OnlineVehicleRentalSystem.Models
{
    public class User : IdentityUser
    {
        // Add any additional properties you need for your application

        
        public required string Name { get; set; }
        
        public required string Password { get; set; }
        // Additional fields as needed
    }
}
