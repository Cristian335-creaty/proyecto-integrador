using System.ComponentModel.DataAnnotations;

namespace Inicio2.Models.ViewModels
{
    /// <summary>
    /// ViewModel for registering a new teacher.
    /// </summary>
    public class RegisterTeacherViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string? Department { get; set; }
    }

    /// <summary>
    /// ViewModel for displaying user information in lists.
    /// </summary>
    public class UserViewModel
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}