using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EHRsystem.Models.ViewModels
{
    public class EditDoctorProfileViewModel
    {
        public string Id { get; set; } = string.Empty;  // Initialize
        
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;  // Initialize
        
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;  // Initialize
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;  // Initialize
        
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;  // Initialize
        
        [Required]
        [Display(Name = "Specialization")]
        public string Specialization { get; set; } = string.Empty;  // Initialize
        
        [Required]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; } = string.Empty;  // Initialize
        
        [Display(Name = "Years of Experience")]
        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
        public int YearsOfExperience { get; set; }
        
        [Display(Name = "Education")]
        public string Education { get; set; } = string.Empty;  // Initialize
        
        [Display(Name = "Bio")]
        public string Bio { get; set; } = string.Empty;  // Initialize
        
        [Display(Name = "Consultation Fee")]
        [Range(0, double.MaxValue, ErrorMessage = "Consultation fee must be a positive number")]
        public decimal ConsultationFee { get; set; }
        
        [Display(Name = "Available From")]
        [DataType(DataType.Time)]
        public TimeSpan AvailableFrom { get; set; } = new TimeSpan(9, 0, 0);  // Default: 9 AM
        
        [Display(Name = "Available To")]
        [DataType(DataType.Time)]
        public TimeSpan AvailableTo { get; set; } = new TimeSpan(17, 0, 0);  // Default: 5 PM
        
        [Display(Name = "Available Days")]
        public List<string> AvailableDays { get; set; } = new List<string>();  // Already initialized
        
        public List<SelectListItem> DaysOfWeek { get; } = new List<SelectListItem>  // Made read-only
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
