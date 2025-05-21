using Estudiante.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Estudiante.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Problema> Problemas { get; set; }
    }
}
