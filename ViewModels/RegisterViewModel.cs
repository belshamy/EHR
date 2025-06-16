using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectListItem
using System.ComponentModel.DataAnnotations;

namespace EHRsystem.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "Department")]
        public string? Department { get; set; } // Nullable, as it might not be required for all roles

        [Display(Name = "Position")]
        public string? Position { get; set; } // Nullable

        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; } // Nullable

        [Display(Name = "Specialization")]
        public string? Specialization { get; set; } // Nullable

        // --- ADD THESE PROPERTIES ---
        public IEnumerable<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>();
        [Required] // Usually a role is required for registration
        [Display(Name = "User Role")]
        public string SelectedRole { get; set; } = null!; // This will hold the selected role's value
    }
}