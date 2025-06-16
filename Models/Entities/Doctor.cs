using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHRsystem.Models.Entities
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; } // Primary key as used in ApplicationDbContext

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string? Specialization { get; set; } // Example: "Cardiology", "Pediatrics"

        [StringLength(10)]
        public string? LicenseNumber { get; set; } // Example: Medical license number

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
        [Required]
        public string UserId { get; set; } = null!; // Must be required as per DbContext config

        [ForeignKey("UserId")] // This attribute is fine on the FK property itself
        public virtual ApplicationUser User { get; set; } = null!; // Navigation property

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        // This collection is crucial and added based on your TestResult configuration in ApplicationDbContext
        public virtual ICollection<TestResult> OrderedTests { get; set; } = new List<TestResult>();

        // You might also have other collections like DoctorSchedules, etc.
        // public virtual ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
    }
}