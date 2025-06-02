using Inicio2.Models;
using Microsoft.AspNetCore.Identity;

namespace Inicio2.Data
{
    
    /// Provides methods to seed initial roles and admin user into the database.
    
    public static class SeedData
    {
    
        /// Initializes the database with default roles and an admin user.
       public static async Task Initialize(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Define default roles
            string[] roles = { "Admin", "Teacher", "Student" };

            // Create roles if they do not exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create initial admin user
            var adminEmail = "admin@universidad.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Main Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}