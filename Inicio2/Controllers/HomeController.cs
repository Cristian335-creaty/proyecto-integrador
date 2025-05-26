using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inicio2.Models;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Inicio2.Controllers
{
    [Authorize] // Requiere autenticación para todas las acciones
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
                    _logger.LogWarning("Usuario autenticado no encontrado en la base de datos");
                    return RedirectToAction("Error", "Home");
                }

                // Redirección basada en roles (usando los nombres correctos de tus roles)
                if (User.IsInRole("Admin"))
                {
                    _logger.LogInformation("Redirigiendo administrador a panel de control");
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                else if (User.IsInRole("Docente"))
                {
                    _logger.LogInformation("Mostrando vista de docente");
                    return View("IndexProfesor", user);
                }
                else if (User.IsInRole("Estudiante"))
                {
                    _logger.LogInformation("Mostrando vista de estudiante");
                    return View("IndexEstudiante", user);
                }

                // Si no tiene ningún rol asignado
                _logger.LogWarning("Usuario sin roles asignados");
                return View("Index", user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en HomeController/Index");
                return RedirectToAction("Error", "Home");
            }
        }

        [AllowAnonymous] // Accesible sin autenticación
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }



        [Authorize(Roles = "Admin,Docente,Estudiante")] // Solo para estos roles
        public IActionResult PanelControl()
        {
            return View();
        }



    }
}