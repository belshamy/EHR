using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EHRsystem.Models.ViewModels
{
    public class CreateAppointmentViewModel
    {
        [Required]
        [Display(Name = "Patient")]
        public string PatientId { get; set; } = string.Empty;  // Initialize
        
        [Required]
        [Display(Name = "Doctor")]
        public string DoctorId { get; set; } = string.Empty;  // Initialize
        
        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [Display(Name = "Duration (minutes)")]
        [Range(15, 480, ErrorMessage = "Duration must be between 15 minutes and 8 hours")]
        public int DurationMinutes { get; set; } = 30;
        
        [Required]
        [Display(Name = "Reason for Visit")]
        public string ReasonForVisit { get; set; } = string.Empty;  // Initialize
        
        [Display(Name = "Notes")]
        public string Notes { get; set; } = string.Empty;  // Initialize with empty string
        
        [Display(Name = "Priority")]
        public string Priority { get; set; } = "Normal";
        
        // For dropdowns - already properly initialized
        public List<SelectListItem> Patients { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Priorities { get; set; } = new List<SelectListItem>();
    }
}
