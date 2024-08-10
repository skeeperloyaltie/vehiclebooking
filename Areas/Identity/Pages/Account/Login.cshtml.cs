// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OnlineVehicleRentalSystem.Models;

namespace OnlineVehicleRentalSystem.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<User> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            _logger.LogInformation("Login attempt for email: {Email}", Input.Email);

            if (ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    _logger.LogInformation("User found: {UserId}", user.Id);

                    // Check if the user is an admin
                    var isAdmin = await _signInManager.UserManager.IsInRoleAsync(user, "Admin");
                    _logger.LogInformation("Is Admin: {IsAdmin}", isAdmin);

                    // Attempt to sign in
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Login successful for user: {UserId}", user.Id);

                        if (isAdmin)
                        {
                            _logger.LogInformation("Redirecting Admin user: {UserId} to Admin Dashboard", user.Id);
                            return LocalRedirect("~/Admin/Index"); // Redirect admin to the admin dashboard
                        }

                        _logger.LogInformation("Redirecting regular user: {UserId} to returnUrl: {ReturnUrl}", user.Id, returnUrl);
                        return LocalRedirect(returnUrl); // Redirect regular user to the original returnUrl or default
                    }

                    if (result.RequiresTwoFactor)
                    {
                        _logger.LogInformation("Login requires two-factor authentication for user: {UserId}", user.Id);
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }

                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out: {UserId}", user.Id);
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        _logger.LogWarning("Invalid login attempt for user: {UserId}", user.Id);
                        ModelState.AddModelError(string.Empty, "Invalid login password.");
                    }
                }
                else
                {
                    _logger.LogWarning("No user found with email: {Email}", Input.Email);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid for login attempt with email: {Email}", Input.Email);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
