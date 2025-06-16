// F:\EHRsystem\Models\Entities\Appointment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EHRsystem.Models.Enums;
// Make sure to include the namespace for ApplicationUser if it's different
// using Microsoft.AspNetCore.Identity; // Or wherever ApplicationUser is defined

namespace EHRsystem.Models.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        // Foreign key for the actual Patient entity (the medical patient)
        [Required]
        public int PatientId { get; set; } // This must be int to match Patient.PatientId
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!; // Navigation property to the Patient entity

        // If you still need a link to ApplicationUser, name it clearly, e.g., PatientUserId
        // [Required] // This might be optional if not every appointment has a direct ApplicationUser link
        // public string ApplicationUserId { get; set; } = null!;
        // [ForeignKey("ApplicationUserId")]
        // public virtual ApplicationUser ApplicationUser { get; set; } = null!;


        [Required]
        public int DoctorId { get; set; } // Foreign key to Doctor
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Notes { get; set; } // Nullable

        public AppointmentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}