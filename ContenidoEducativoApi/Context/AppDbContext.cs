using ContenidoEducativoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContenidoEducativoApi.Context
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Contenido> Contenidos { get; set; }
    }
}
