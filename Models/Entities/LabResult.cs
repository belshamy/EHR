// F:\EHRsystem\Models\Entities\LabResult.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHRsystem.Models.Entities
{
    public class LabResult
    {
        [Key]
        public int Id { get; set; }

        // --- CHANGE HERE: PatientId should be int to link to EHRsystem.Models.Entities.Patient ---
        [Required]
        public int PatientId { get; set; } // Foreign key to Patient.PatientId (int)
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!; // Navigation property for the medical Patient

        [Required]
        public int LabTestId { get; set; }
        [ForeignKey("LabTestId")]
        public virtual LabTest LabTest { get; set; } = null!; // Initialize: Navigation property for LabTest

        [Required]
        public int OrderedByDoctorId { get; set; }
        [ForeignKey("OrderedByDoctorId")]
        public virtual Doctor OrderedByDoctor { get; set; } = null!; // Initialize: Navigation property for Doctor

        public DateTime ResultDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}