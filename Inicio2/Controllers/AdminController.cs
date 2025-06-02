using Inicio2.Models;
using Inicio2.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Inicio2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        
        /// AdminController constructor.
        
        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        
        /// Returns the admin dashboard view.
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        
        /// Returns the view to register a new teacher.
        
        [HttpGet]
        public IActionResult RegisterTeacher()
        {
            return View();
        }

        
        /// Handles the registration of a new teacher.
        
        [HttpPost]
        public async Task<IActionResult> RegisterTeacher(RegisterTeacherModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Department = model.Department
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Teacher");
                    ViewBag.SuccessMessage = "Teacher created successfully";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        
        /// Returns the list of users with their roles.
        
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var users = _userManager.Users.ToList();
            var usersWithRoles = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "No role"
                });
            }

            return View(usersWithRoles);
        }

        
        /// Returns the view to edit a teacher.
        
        [HttpGet]
        public async Task<IActionResult> EditTeacher(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditTeacherViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                Department = user.Department
            };
            return View(model);
        }

        
        /// Handles the update of a teacher's information.
        
        [HttpPost]
        public async Task<IActionResult> EditTeacher(string id, EditTeacherViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Department = model.Department;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction("UserList");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        
        /// Returns the view to edit a student.
        
        [HttpGet]
        public async Task<IActionResult> EditStudent(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditStudentViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                StudentCode = user.StudentCode
            };
            return View(model);
        }

        
        /// Handles the update of a student's information.
        
        [HttpPost]
        public async Task<IActionResult> EditStudent(string id, EditStudentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.StudentCode = model.StudentCode;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction("UserList");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        
        /// Handles the deletion of a user (except Admins).
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("UserList");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return RedirectToAction("UserList");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                TempData["Error"] = "Cannot delete a user with Admin role.";
                return RedirectToAction("UserList");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Error deleting the user.";
            }

            return RedirectToAction("UserList");
        }
    }

    
    /// ViewModel for registering a new teacher.
    
    public class RegisterTeacherModel
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

    
    /// ViewModel for displaying user information in lists.
    
    public class UserInfoViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}