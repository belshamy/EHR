using System; // Make sure this is included for DateTime and DateTimeOffset
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EHRsystem.Models.ViewModels
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; } = string.Empty; // Initialize to prevent nullability warnings

        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "First Name")] // Added based on AdminController trying to assign
        public string? FirstName { get; set; } // Can be nullable if not always present

        [Display(Name = "Last Name")] // Added based on AdminController trying to assign
        public string? LastName { get; set; } // Can be nullable if not always present

        [Display(Name = "Address")] // Added based on AdminController trying to assign
        public string? Address { get; set; } // Can be nullable

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; } // Made nullable as per common practice/warnings

        // Removed existing 'Role' string as controller maps a List<string> Roles
        // [Display(Name = "Role")]
        // public string Role { get; set; }

        [Display(Name = "Department")]
        public string? Department { get; set; } // Made nullable as per common practice/warnings

        [Display(Name = "Position")]
        public string? Position { get; set; } // Made nullable as per common practice/warnings

        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; } // Made nullable as per common practice/warnings

        [Display(Name = "Specialization")]
        public string? Specialization { get; set; } // Made nullable as per common practice/warnings

        [Display(Name = "Years of Experience")]
        public int? YearsOfExperience { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; } // Made nullable if it can be null

        [Display(Name = "Last Login Date")] // Renamed from LastLogin to match controller assignment
        public DateTimeOffset? LastLoginDate { get; set; } // Changed type to DateTimeOffset? to match user.LastLogin

        // [Display(Name = "Is Active")] // You might need to add this property to ApplicationUser if you want to use it
        // public bool IsActive { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool IsEmailConfirmed { get; set; } // Renamed from EmailConfirmed to match controller assignment

        [Display(Name = "Is Lockout Enabled")] // Added based on AdminController trying to assign
        public bool IsLockoutEnabled { get; set; }

        [Display(Name = "Lockout End")]
        public DateTimeOffset? LockoutEnd { get; set; }

        [Display(Name = "Access Failed Count")]
        public int AccessFailedCount { get; set; }

        // Collections for roles and claims
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; } = new List<string>();

        [Display(Name = "Claims")]
        public List<ClaimViewModel> Claims { get; set; } = new List<ClaimViewModel>();

        // Define ClaimViewModel as an inner class
        public class ClaimViewModel
        {
            public string Type { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }
    }
}