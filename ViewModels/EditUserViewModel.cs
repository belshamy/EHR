using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EHRsystem.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100, ErrorMessage = "First Name cannot exceed 100 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(100, ErrorMessage = "Last Name cannot exceed 100 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool IsEmailConfirmed { get; set; }

        [Display(Name = "Lockout Enabled")]
        public bool IsLockoutEnabled { get; set; }

        [Display(Name = "Locked Until")]
        public DateTimeOffset? LockoutEnd { get; set; }

        [Display(Name = "Requires Password Change on Next Login")]
        public bool RequiresPasswordChange { get; set; }

        // These are the properties AdminController needs to exist in your ViewModel
        public List<string> UserRoles { get; set; } = new List<string>();

        // For displaying roles in a multi-select dropdown
        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();

        // To bind selected roles from the form
        [Display(Name = "Selected Roles")]
        public List<string>? SelectedRoles { get; set; }
    }
}