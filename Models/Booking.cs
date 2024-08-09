namespace OnlineVehicleRentalSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
        // Additional fields as needed
    }
}
