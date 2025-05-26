using System.ComponentModel.DataAnnotations;

namespace Inicio2.Models.ViewModels
{
    public class RegistrarProfesorViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }

        [Required]
        public string? NombreCompleto { get; set; }

        [Required]
        public string? Departamento { get; set; }
    }

    public class UsuarioViewModel
    {
        public string? Id { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Email { get; set; }
        public string? Rol { get; set; }
    }
}