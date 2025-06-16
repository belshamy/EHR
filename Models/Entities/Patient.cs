using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Still needed for [Table] if you use it, or [Column] etc.

namespace EHRsystem.Models.Entities
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; } // Primary key is PatientId (int)

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

        [StringLength(500)]
        public string? Address { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(5)]
        public string? BloodType { get; set; }

        [StringLength(200)]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(14)]
        public string? NationalId { get; set; }

        [StringLength(100)]
        public string? Governorate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // --- Foreign key to ApplicationUser (string UserId) ---
        public string? UserId { get; set; } // Matches ApplicationUser.Id (string)
        [ForeignKey("UserId")] // This attribute is fine on the FK property itself
        public virtual ApplicationUser? User { get; set; } // Nullable navigation property

        // --- Navigation properties (Collections for Patient) ---
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
        public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();
        public virtual ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
        public virtual ICollection<MedicalFile> MedicalFiles { get; set; } = new List<MedicalFile>();
        public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    }
}