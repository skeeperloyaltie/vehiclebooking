using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineVehicleRentalSystem.Data;
using OnlineVehicleRentalSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineVehicleRentalSystem.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ManageBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Vehicle)
                .ToListAsync();

            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> AddBooking()
        {
            // Fetch available vehicles to book
            var vehicles = await _context.Vehicles.ToListAsync();
            ViewBag.Vehicles = vehicles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBooking(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageBookings));
            }

            // If the model state is invalid, reload vehicles and return to the form
            var vehicles = await _context.Vehicles.ToListAsync();
            ViewBag.Vehicles = vehicles;
            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> EditBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var vehicles = await _context.Vehicles.ToListAsync();
            ViewBag.Vehicles = vehicles;
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> EditBooking(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageBookings));
            }

            var vehicles = await _context.Vehicles.ToListAsync();
            ViewBag.Vehicles = vehicles;
            return View(booking);
        }

        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageBookings));
        }
    }
}
