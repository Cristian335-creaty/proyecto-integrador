using Inicio2.Data;
using Inicio2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de DbContext
builder.Services.AddDbContext<bd_universidadContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

// 2. Configuración de Identity con roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;

    // Configuraciones adicionales de usuario
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<bd_universidadContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Para soporte de interfaces de Identity

// 3. Configuración de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Configuración de MVC y Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    await SeedDatabase(app); // Solo en desarrollo
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Habilitar sesiones
app.UseAuthentication();
app.UseAuthorization();

// Mapeo de rutas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// Función para inicializar la base de datos
async Task SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<bd_universidadContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Asegurar que la base de datos esté creada
        await context.Database.EnsureCreatedAsync();

        // Crear roles básicos
        string[] roles = { "Admin", "Docente", "Estudiante" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Crear usuario administrador inicial
        var adminEmail = "admin@universidad.edu";
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
                await userManager.AddToRoleAsync(adminUser, "Admin");

                // Agregar todos los roles si es necesario (opcional)
                // await userManager.AddToRolesAsync(adminUser, roles);
            }
        }

        // Puedes agregar más datos de prueba aquí según sea necesario

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos");
    }
}