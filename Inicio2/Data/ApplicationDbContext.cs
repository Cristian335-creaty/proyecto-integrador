using Inicio2.Models.Estudiantes;
using Microsoft.EntityFrameworkCore;
namespace Inicio2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Problem> Problems { get; set; }
    }
}
