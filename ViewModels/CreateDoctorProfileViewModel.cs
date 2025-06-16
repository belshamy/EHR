using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EHRsystem.Models.ViewModels
{
    public class CreateDoctorProfileViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Specialization")]
        public string Specialization { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; } = string.Empty;
        
        [Display(Name = "Years of Experience")]
        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
        public int YearsOfExperience { get; set; }
        
        [Display(Name = "Education")]
        public string Education { get; set; } = string.Empty;
        
        [Display(Name = "Bio")]
        public string Bio { get; set; } = string.Empty;
        
        [Display(Name = "Consultation Fee")]
        [Range(0, double.MaxValue, ErrorMessage = "Consultation fee must be a positive number")]
        public decimal ConsultationFee { get; set; }
        
        [Display(Name = "Available From")]
        [DataType(DataType.Time)]
        public TimeSpan AvailableFrom { get; set; } = new TimeSpan(9, 0, 0); // 9 AM
        
        [Display(Name = "Available To")]
        [DataType(DataType.Time)]
        public TimeSpan AvailableTo { get; set; } = new TimeSpan(17, 0, 0); // 5 PM
        
        [Display(Name = "Available Days")]
        public List<string> AvailableDays { get; set; } = new List<string>();
        
        public List<SelectListItem> DaysOfWeek { get; } = new List<SelectListItem> // Made read-only
        {
            new SelectListItem { Value = "Monday", Text = "Monday" },
            new SelectListItem { Value = "Tuesday", Text = "Tuesday" },
            new SelectListItem { Value = "Wednesday", Text = "Wednesday" },
            new SelectListItem { Value = "Thursday", Text = "Thursday" },
            new SelectListItem { Value = "Friday", Text = "Friday" },
            new SelectListItem { Value = "Saturday", Text = "Saturday" },
            new SelectListItem { Value = "Sunday", Text = "Sunday" }
        };
    }
}
