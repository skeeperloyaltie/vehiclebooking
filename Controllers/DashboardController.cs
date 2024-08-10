using Microsoft.AspNetCore.Identity;
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

        private readonly UserManager<User> _userManager;

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
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

        public async Task<IActionResult> ViewBookings()
        {
            try
            {
                // Fetch the bookings from the database
                var bookings = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Vehicle)
                    .ToListAsync();

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
        public IActionResult AddBooking()
        {
            ViewBag.Vehicles = _context.Vehicles.ToList(); // Populate the vehicle dropdown
            return View();
        }



        [HttpPost]
public async Task<IActionResult> AddBooking(Booking model)
{
    try
    {
        if (ModelState.IsValid)
        {
            // Set the UserId for the booking based on the current logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("AddBooking: User not found for username {UserName}.", User.Identity.Name);
                ModelState.AddModelError("", "User not found.");
                ViewBag.Vehicles = _context.Vehicles.ToList();
                return View(model);
            }
            model.UserId = currentUser.Id;
            _logger.LogInformation("AddBooking: UserId {UserId} set for booking.", currentUser.Id);

            // Validate that the selected vehicle exists
            var vehicle = await _context.Vehicles.FindAsync(model.VehicleId);
            if (vehicle == null)
            {
                _logger.LogWarning("AddBooking: Vehicle not found for VehicleId {VehicleId}.", model.VehicleId);
                ModelState.AddModelError("", "Vehicle not found.");
                ViewBag.Vehicles = _context.Vehicles.ToList();
                return View(model);
            }
            _logger.LogInformation("AddBooking: VehicleId {VehicleId} is valid.", model.VehicleId);

            // Add the booking to the database
            _context.Bookings.Add(model);
            await _context.SaveChangesAsync();
            _logger.LogInformation("AddBooking: Booking successfully saved to database with BookingId {BookingId}.", model.Id);

            // Redirect to the Bookings view after a successful booking
            return RedirectToAction("ViewBookings");
        }

        // Log the reason why the model state is invalid
        _logger.LogWarning("AddBooking: ModelState is invalid. Errors: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));

        // Repopulate the vehicle dropdown if the model is invalid
        ViewBag.Vehicles = _context.Vehicles.ToList();
        return View(model);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error adding booking.");
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
            ViewBag.Vehicles = _context.Vehicles.ToList(); // Populate the vehicle dropdown
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> EditBooking(Booking model)
        {
            if (ModelState.IsValid)
            {
                _context.Bookings.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewBookings");
            }
            ViewBag.Vehicles = _context.Vehicles.ToList(); // Repopulate the vehicle dropdown if the model is invalid
            return View(model);
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
