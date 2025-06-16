using System;
using System.ComponentModel.DataAnnotations;
// Removed: using System.ComponentModel.DataAnnotations.Schema; // No longer needed here

namespace EHRsystem.Models.Entities
{
    public class TestResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        // Removed: [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;

        [Required]
        public int LabTestId { get; set; } // This is the correct foreign key for LabTest
        // Removed: [ForeignKey("LabTestId")]
        public virtual LabTest LabTest { get; set; } = null!;

        [Required]
        public string Result { get; set; } = string.Empty;

        public string ReferenceRange { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public string Notes { get; set; } = string.Empty;

        [Display(Name = "Test Date")]
        public DateTime TestDate { get; set; }

        [Display(Name = "Result Date")]
        public DateTime? ResultDate { get; set; }

        [Required]
        public int OrderedByDoctorId { get; set; }
        // Removed: [ForeignKey("OrderedByDoctorId")]
        public virtual Doctor OrderedByDoctor { get; set; } = null!;

        [Display(Name = "Reviewed By")]
        public string? ReviewedBy { get; set; }

        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }

        public bool IsAbnormal { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}