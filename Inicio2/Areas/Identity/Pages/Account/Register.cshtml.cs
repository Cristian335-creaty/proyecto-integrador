// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Inicio2.Models;
using Microsoft.AspNetCore.Authentication;

namespace Inicio2.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre completo es obligatorio")]
            [Display(Name = "Nombre Completo")]
            public string NombreCompleto { get; set; }

            [Required(ErrorMessage = "El email es obligatorio")]
            [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Código de Estudiante (opcional)")]
            public string CodigoEstudiante { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    NombreCompleto = Input.NombreCompleto,
                    CodigoEstudiante = Input.CodigoEstudiante
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // 1. Verificar/Crear rol Estudiante
                    if (!await _roleManager.RoleExistsAsync("Estudiante"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Estudiante"));
                    }

                    // 2. Asignar rol
                    await _userManager.AddToRoleAsync(user, "Estudiante");

                    // 3. Iniciar sesión
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // 4. REDIRECCIÓN ESPECÍFICA PARA ESTUDIANTES - Aquí va el punto 5
                    if (await _userManager.IsInRoleAsync(user, "Estudiante"))
                    {
                        return LocalRedirect("/Estudiantes/Index");
                    }

                    // Redirección normal para otros roles (si los hubiera)
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Si llegamos aquí, algo falló, volver a mostrar el formulario
            return Page();
        }
    }
}