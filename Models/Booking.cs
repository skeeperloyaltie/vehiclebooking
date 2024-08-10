namespace OnlineVehicleRentalSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Updated to string to match IdentityUser primary key type
        public int VehicleId { get; set; }
        public DateTime RentalDate { get; set; } // Changed datetime2 to datetime
        public DateTime ReturnDate { get; set; } // Changed datetime2 to datetime

        // Navigation properties
        public User User { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}