using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inicio2.Models;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Inicio2.Controllers
{
    [Authorize] // Requires authentication for all actions
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogWarning("Authenticated user not found in the database");
                    return RedirectToAction("Error", "Home");
                }

                // Role-based redirection (using the correct role names)
                if (User.IsInRole("Admin"))
                {
                    _logger.LogInformation("Redirecting admin to control panel");
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                else if (User.IsInRole("Teacher"))
                {
                    _logger.LogInformation("Showing teacher view");
                    return View("IndexTeacher", user);
                }
                else if (User.IsInRole("Student"))
                {
                    _logger.LogInformation("Showing student view");
                    return View("IndexStudent", user);
                }

                // If the user has no assigned roles
                _logger.LogWarning("User without assigned roles");
                return View("Index", user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HomeController/Index");
                return RedirectToAction("Error", "Home");
            }
        }

        [AllowAnonymous] // Accessible without authentication
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        [Authorize(Roles = "Admin,Teacher,Student")] // Only for these roles
        public IActionResult ControlPanel()
        {
            return View();
        }
    }
}