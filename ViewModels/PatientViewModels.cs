using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using EHRsystem.Models.Enums; 

namespace EHRsystem.ViewModels
{
    public class PatientProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        
        public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }
        public string? ChronicConditions { get; set; }
        public string? CurrentMedications { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Age")]
        public int Age => CalculateAge(DateOfBirth);
        
        private static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public class CreatePatientViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-30); // Default: 30 years ago

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = "Other"; // Default value

        [Display(Name = "Address")]
        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name = "Emergency Contact")]
        [StringLength(100)]
        public string? EmergencyContact { get; set; }

        [Display(Name = "Emergency Contact Phone")]
        [Phone]
        [RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Invalid phone number format")]
        public string? EmergencyContactPhone { get; set; }

        [Display(Name = "Blood Type")]
        public string? BloodType { get; set; }

        [Display(Name = "Allergies")]
        [StringLength(500)]
        public string? Allergies { get; set; }

        [Display(Name = "Chronic Conditions")]
        [StringLength(500)]
        public string? ChronicConditions { get; set; }

        [Display(Name = "Current Medications")]
        [StringLength(500)]
        public string? CurrentMedications { get; set; }

        // For dropdowns
        public List<SelectListItem> GenderOptions { get; } = new List<SelectListItem> { };
     
        public List<SelectListItem> BloodTypeOptions { get; } = new List<SelectListItem>
        {
            new("A+", "A+"),
            new("A-", "A-"),
            new("B+", "B+"),
            new("B-", "B-"),
            new("AB+", "AB+"),
            new("AB-", "AB-"),
            new("O+", "O+"),
            new("O-", "O-"),
            new("Unknown", "Unknown")
        };
    }
}
