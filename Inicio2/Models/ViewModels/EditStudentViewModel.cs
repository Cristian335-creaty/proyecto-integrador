using System.ComponentModel.DataAnnotations;

namespace Inicio2.Models.ViewModels
{
    
    /// ViewModel for editing student information.
    
    public class EditStudentViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Student code is required")]
        public string StudentCode { get; set; }
    }
}