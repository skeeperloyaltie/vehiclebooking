namespace OnlineVehicleRentalSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Updated to string to match IdentityUser primary key type
        public int VehicleId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}
