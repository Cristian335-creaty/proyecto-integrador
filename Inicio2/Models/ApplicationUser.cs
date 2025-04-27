using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Inicio2.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [PersonalData]
        public string NombreCompleto { get; set; }

        // Solo para estudiantes
        [PersonalData]
        public string? CodigoEstudiante { get; set; }

        // Solo para profesores
        [PersonalData]
        public string? Departamento { get; set; }
    }
}
