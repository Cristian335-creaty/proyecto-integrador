using Estudiante.Services;
using Inicio2.Data;
using Inicio2.Models;
using Inicio2.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


/// Entry point for the web application. Configures services, middleware, and database seeding.
/// All comments and variable names are in English to comply with LSP Clean Code principles.

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext configuration
builder.Services.AddDbContext<bd_universityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity configuration with roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;

    // Additional user configurations
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<bd_universityContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // For Identity UI support

// 3. Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. MVC and Razor Pages configuration
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register Judge0Service as singleton
builder.Services.AddSingleton<Judge0Service>();

// Register Contenido services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IContenidoService, ContenidoService>();

var app = builder.Build();

// HTTP pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    await SeedDatabaseAsync(app); // Only in development
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Enable sessions
app.UseAuthentication();
app.UseAuthorization();

// Route mapping
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();


/// Seeds the database with initial roles and an admin user. Only runs in development.


async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<bd_universityContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure the database is created
        await context.Database.EnsureCreatedAsync();

        // Create basic roles
        string[] roles = { "Admin", "Teacher", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create initial admin user
        var adminEmail = "admin@university.edu";
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

                // Optionally add all roles to the admin user
                // await userManager.AddToRolesAsync(adminUser, roles);
            }
        }

        // You can add more seed data here if needed

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}