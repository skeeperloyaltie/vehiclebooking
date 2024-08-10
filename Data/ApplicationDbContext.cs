using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineVehicleRentalSystem.Models;

namespace OnlineVehicleRentalSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example of configuring relationships or other properties

            // Configure the relationship between Booking and Vehicle
            modelBuilder.Entity<Booking>()
                .HasOne<User>(b => b.User)  // Each Booking has one User
                .WithMany(u => u.Bookings)  // Each User can have many Bookings
                .HasForeignKey(b => b.UserId);  // Foreign key on Booking is UserId

            modelBuilder.Entity<Booking>()
                .HasOne<Vehicle>(b => b.Vehicle)  // Each Booking is related to one Vehicle
                .WithMany()  // A vehicle can have many bookings
                .HasForeignKey(b => b.VehicleId);  // Foreign key on Booking is VehicleId

            // You can add more configurations here if necessary
        }
    }
}
