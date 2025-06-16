// F:\EHRsystem\Models\ViewModels\UserManagementViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Make sure this is included

namespace EHRsystem.Models.ViewModels
{
    public class UserManagementViewModel // This represents a single user entry
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool IsEmailConfirmed { get; set; }

        [Display(Name = "Lockout Enabled")]
        public bool IsLockoutEnabled { get; set; }

        [Display(Name = "Lockout End")]
        public DateTimeOffset? LockoutEnd { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}