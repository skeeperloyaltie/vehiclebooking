using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineVehicleRentalSystem.Data;
using OnlineVehicleRentalSystem.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineVehicleRentalSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _context;

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                // Logic to prepare the dashboard overview
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading the dashboard.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> VehicleListings()
        {
            try
            {
                // Fetch vehicle listings from database
                var vehicles = await _context.Vehicles.ToListAsync();
                return View(vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading vehicle listings.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    // If no search query is provided, return the user to the vehicle listings page or show all vehicles
                    return RedirectToAction("VehicleListings");
                }

                // Search for vehicles that match the query
                var searchResults = await _context.Vehicles
                    .Where(v => v.Model.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync();

                // If search results are found, redirect to a view that displays the search results
                return View("SearchResults", searchResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing search.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


        public IActionResult Bookings()
        {
            try
            {
                // Logic for booking system
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing booking.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
      public IActionResult ViewBookings()
{
    try
    {
        // Fetch the bookings from the database
        var bookings = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Vehicle)
            .ToList();

        // Pass the bookings list to the view
        return View(bookings);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error loading bookings.");
        return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


        [HttpGet]
        public IActionResult EditBooking(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost]
        public IActionResult EditBooking(Booking model)
        {
            if (ModelState.IsValid)
            {
                _context.Bookings.Update(model);
                _context.SaveChanges();
                return RedirectToAction("ViewBookings");
            }
            return View(model);
        }

        public IActionResult DeleteBooking(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            return RedirectToAction("ViewBookings");
        }


        public IActionResult Payment()
        {
            try
            {
                // Logic for payment processing
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult Reviews()
        {
            try
            {
                // Logic to manage reviews and ratings
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reviews.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult Profile()
        {
            try
            {
                // Logic to display user profile and history
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
