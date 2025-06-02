using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Inicio2.Models
{
    
    /// Represents an application user with additional properties for students and teachers.
    
    public class ApplicationUser : IdentityUser
    {
        
        /// Full name of the user (required for all users).
        
        [Required]
        [PersonalData]
        public string FullName { get; set; }

        /// Student code (only for students).

        [PersonalData]
        public string? StudentCode { get; set; }

        
        /// Department (only for teachers).
        
        [PersonalData]
        public string? Department { get; set; }
    }
}
