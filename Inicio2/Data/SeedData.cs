using Inicio2.Models;
using Microsoft.AspNetCore.Identity;

namespace Inicio2.Data
{
    public static class SeedData
    {
        public static async Task Initialize(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Crear roles básicos
            string[] roles = { "Administrador", "Profesor", "Estudiante" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Crear usuario administrador
            var adminEmail = "admin@universidad.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NombreCompleto = "Administrador Principal",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }
        }
    }
}
