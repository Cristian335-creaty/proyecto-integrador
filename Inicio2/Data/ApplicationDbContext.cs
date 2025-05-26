using Inicio2.Models.Estudiantes;
using Microsoft.EntityFrameworkCore;
namespace Inicio2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Problema> Problemas { get; set; }
    }
}
