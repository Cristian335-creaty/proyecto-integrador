using System.ComponentModel.DataAnnotations;

namespace Inicio2.Models.ViewModels
{
 
    /// ViewModel for editing teacher information.
 
    public class EditTeacherViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; }
    }
}