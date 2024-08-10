using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineVehicleRentalSystem.Data;
using OnlineVehicleRentalSystem.Models;
using System.Linq;

namespace OnlineVehicleRentalSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var totalUsers = _context.Users.Count();
            var totalBookings = _context.Bookings.Count();
            return View(new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalBookings = totalBookings
            });
        }

        public IActionResult ManageVehicles()
        {
            var vehicles = _context.Vehicles.ToList();
            return View(vehicles);
        }

        [HttpGet]
        public IActionResult AddVehicle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddVehicle(Vehicle model)
        {
            if (ModelState.IsValid)
            {
                _context.Vehicles.Add(model);
                _context.SaveChanges();
                return RedirectToAction("ManageVehicles");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditVehicle(int id)
        {
            var vehicle = _context.Vehicles.Find(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        [HttpPost]
        public IActionResult EditVehicle(Vehicle model)
        {
            if (ModelState.IsValid)
            {
                _context.Vehicles.Update(model);
                _context.SaveChanges();
                return RedirectToAction("ManageVehicles");
            }
            return View(model);
        }

        public IActionResult DeleteVehicle(int id)
        {
            var vehicle = _context.Vehicles.Find(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();
            return RedirectToAction("ManageVehicles");
        }

        public IActionResult ViewBookings()
        {
            var bookings = _context.Bookings.ToList();
            return View(bookings);
        }
    }
}
