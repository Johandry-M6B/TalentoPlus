using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.DTOs;
using TalentoPlus.Core.Entities;

namespace TalentoPlus.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        
        // Helper to create initial admin user (for demo purposes)
        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            var user = new User { UserName = "admin@talentoplus.com", Email = "admin@talentoplus.com", FirstName = "Admin", LastName = "User" };
            var result = await _userManager.CreateAsync(user, "Admin123!");
            if (result.Succeeded)
            {
                return Content("Admin user created");
            }
            return Content("Error creating admin: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
