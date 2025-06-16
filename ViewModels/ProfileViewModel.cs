using System.ComponentModel.DataAnnotations;

namespace EHRsystem.Models.ViewModels
{
    public class ProfileViewModel
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
        
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }  // Nullable - no initialization needed
        
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;  // Initialize
        
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;  // Initialize
        
        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; } = string.Empty;  // Initialize
        
        [Display(Name = "Two Factor Authentication")]
        public bool TwoFactorEnabled { get; set; }
        
        // Computed property for full name
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
