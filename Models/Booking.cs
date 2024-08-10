namespace OnlineVehicleRentalSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Updated to string to match IdentityUser primary key type
        public int VehicleId { get; set; }
        public int? VehicleId1 { get; set; } // Consider nullable int for VehicleId1
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Vehicle Vehicle { get; set; }
        public Vehicle Vehicle1 { get; set; } // Navigation property for VehicleId1
    }
}
