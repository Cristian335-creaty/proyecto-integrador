using Inicio2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Inicio2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CrearProfesor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearProfesor(CrearProfesorModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    NombreCompleto = model.NombreCompleto,
                    Departamento = model.Departamento
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Docente");
                    ViewBag.SuccessMessage = "Profesor creado exitosamente";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ListaUsuarios()
        {
            var usuarios = _userManager.Users.ToList();
            var usuariosConRoles = new List<UsuarioInfoViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                usuariosConRoles.Add(new UsuarioInfoViewModel
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email,
                    Rol = roles.FirstOrDefault() ?? "Sin rol"
                });
            }

            return View(usuariosConRoles);
        }
    }

    public class CrearProfesorModel
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El departamento es requerido")]
        public string Departamento { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
    }

    public class UsuarioInfoViewModel
    {
        public string Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
    }
}