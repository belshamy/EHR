using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using EHRsystem.Models.Enums; 

namespace EHRsystem.Models.ViewModels
{
    public class EditAppointmentViewModel
    {
        public int Id { get; set; }
        
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
        public int DurationMinutes { get; set; }
        
        [Required]
        [Display(Name = "Reason for Visit")]
        public string ReasonForVisit { get; set; } = string.Empty;  // Initialize
        
        [Display(Name = "Notes")]
        public string Notes { get; set; } = string.Empty;  // Initialize
        
        [Display(Name = "Priority")]
        public string Priority { get; set; } = "Normal";  // Initialize with default
        
        [Display(Name = "Status")]
        public string Status { get; set; } = "Scheduled";  // Initialize with default
        
    }
}
