using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineVehicleRentalSystem.Models;
using System.Threading.Tasks;

namespace OnlineVehicleRentalSystem.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(UserManager<User> userManager, ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Profile
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Profile: User not found.");
                return NotFound("User not found.");
            }

            return View(currentUser);
        }

        // POST: Profile/Update
        [HttpPost]
        public async Task<IActionResult> Update(User model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Profile: ModelState is invalid.");
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Profile: User not found.");
                return NotFound("User not found.");
            }

            // Update user details
            user.Email = model.Email;
            user.Name = model.Name;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("Profile: User profile updated successfully.");
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Index", model);
        }
    }
}
