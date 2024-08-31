using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineVehicleRentalSystem.Data;
using OnlineVehicleRentalSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineVehicleRentalSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Admin Dashboard
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

        // Manage Vehicles
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

        // Manage Users
        public IActionResult ManageUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.Name = model.Name;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ManageUsers");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ManageUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction("ManageUsers");
        }

        // Manage Bookings
        public IActionResult ViewBookings()
        {
            var bookings = _context.Bookings.Include(b => b.User).Include(b => b.Vehicle).ToList();
            return View(bookings);
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
    }
}
