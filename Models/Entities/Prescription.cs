using System;
using System.ComponentModel.DataAnnotations;
// Removed: using System.ComponentModel.DataAnnotations.Schema; // No longer needed here

namespace EHRsystem.Models.Entities
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; } // Foreign key to Patient.PatientId (int)
        // Removed: [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!; // Navigation property

        [Required]
        public int DoctorId { get; set; }
        // Removed: [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Medication { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Instructions { get; set; } = string.Empty;

        [Required]
        public DateTime PrescriptionDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Dosage { get; set; }

        [StringLength(200)]
        public string? Frequency { get; set; }

        [StringLength(200)]
        public string? Duration { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}