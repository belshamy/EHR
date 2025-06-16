using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering; // <--- ADD THIS USING DIRECTIVE
using System; // For DateTime and DateTimeOffset

namespace EHRsystem.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; } // Changed to nullable string for better flexibility

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; } // Changed to nullable string for better flexibility

        [Display(Name = "Address")]
        public string? Address { get; set; } = string.Empty; // Changed to nullable string for better flexibility

        [Display(Name = "Emergency Contact")]
        public string? EmergencyContact { get; set; } // Changed to nullable string for better flexibility

        [Display(Name = "Two Factor Authentication")]
        public bool TwoFactorEnabled { get; set; }

        // Existing role properties - if these are intended to hold just string names of roles
        // and not for dropdowns, keep them as List<string>.
        // If they are part of dropdown binding, change them to List<SelectListItem> too.
        public List<string> Roles { get; set; } = new List<string>();
        // public List<string> AvailableRoles { get; set; } = new List<string>(); // Reconsider if this is for dropdown

        // ADD THESE MISSING PROPERTIES that your AdminController is trying to access:

        [Display(Name = "Email Confirmed")]
        public bool IsEmailConfirmed { get; set; }

        [Display(Name = "Lockout Enabled")]
        public bool IsLockoutEnabled { get; set; }

        [Display(Name = "Lockout End")]
        public DateTimeOffset? LockoutEnd { get; set; }

        // These properties are typically used for displaying and selecting roles in a UI.
        // They MUST be of type List<SelectListItem> if you are populating them with SelectListItem objects.
        [Display(Name = "User Roles")]
        public List<string> UserRoles { get; set; } = new List<string>(); // Keep as string if it's just the names of assigned roles

        [Display(Name = "All Available Roles")]
        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>(); // <--- CHANGED TO List<SelectListItem>

        [Display(Name = "Selected Roles")]
        public List<string> SelectedRoles { get; set; } = new List<string>(); // Keep as string if this is where the *values* of selected roles go
    }
}