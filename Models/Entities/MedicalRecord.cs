using System;
using System.ComponentModel.DataAnnotations;
// Removed: using System.ComponentModel.DataAnnotations.Schema; // No longer needed here

namespace EHRsystem.Models.Entities
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime RecordDate { get; set; }

        [StringLength(500)]
        public string? ChiefComplaint { get; set; }

        [StringLength(2000)]
        public string? Symptoms { get; set; }

        [StringLength(2000)]
        public string? Treatment { get; set; }

        [StringLength(4000)]
        public string? Notes { get; set; }

        // Foreign key properties
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        // Navigation properties - **[ForeignKey] attributes REMOVED**
        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;

        // Audit properties
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Constructor to set default values
        public MedicalRecord()
        {
            RecordDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}