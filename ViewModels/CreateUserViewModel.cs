using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectListItem
// Make sure System is included if you use DateTime or DateTimeOffset later
// using System; 

namespace EHRsystem.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!; // Using null! for non-nullable, required string

        // --- ADDED PROPERTIES ---
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;
        
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;
        // -----------------------

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; } // Can be nullable, as it's Phone, not Required

        // --- ADDED PROPERTY ---
        [Display(Name = "Address")]
        public string? Address { get; set; } // Can be nullable if not always required
        // --------------------

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!; // This might be used for a single selected role initially

        [Display(Name = "Department")]
        public string? Department { get; set; }

        [Display(Name = "Position")]
        public string? Position { get; set; }

        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }

        [Display(Name = "Specialization")]
        public string? Specialization { get; set; }

        // --- ADD THESE FOR ROLE MANAGEMENT IN ADMIN UI ---
        public IEnumerable<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>(); // This is correct
        public List<string> SelectedRoles { get; set; } = new List<string>(); // For multi-select roles
    }
}