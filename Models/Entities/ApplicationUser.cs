using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace EHRsystem.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "First Name")]
        [StringLength(75)]
        public string? FirstName { get; set; } 

        [Display(Name = "Last Name")]
        [StringLength(75)]
        public string? LastName { get; set; } 

        [Display(Name = "Address")]
        [StringLength(255)]
        public string? Address { get; set; } 

        [Display(Name = "Department")]
        [StringLength(100)]
        public string? Department { get; set; }

        [Display(Name = "Position")]
        [StringLength(100)]
        public string? Position { get; set; }

        [Display(Name = "License Number")]
        [StringLength(50)]
        public string? LicenseNumber { get; set; }

        [Display(Name = "Specialization")]
        [StringLength(100)]
        public string? Specialization { get; set; }

        [Display(Name = "Years of Experience")]
        public int? YearsOfExperience { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; } 

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Profile Picture")]
        public string? ProfilePictureUrl { get; set; }

        // --- Properties related to Identity account management ---
        public DateTimeOffset? AccountLockedUntil { get; set; }
        public int FailedLoginAttempts { get; set; } = 0;
        public bool RequiresPasswordChange { get; set; } = false;

        // --- Navigation properties for related entities ---
        // PatientAppointments (used in Appointment.cs)
        public virtual ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>(); 
        public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<UserAuditLog> AuditLogs { get; set; } = new List<UserAuditLog>(); // Added for UserAuditLog mapping
        public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>(); // Added for LabResult mapping
    }
}
