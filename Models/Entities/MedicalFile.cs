// F:\EHRsystem\Models\Entities\MedicalFile.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHRsystem.Models.Entities
{
    public class MedicalFile
    {
        [Key]
        public int Id { get; set; }

        // --- CHANGE HERE: PatientId should be int to link to EHRsystem.Models.Entities.Patient ---
        [Required]
        public int PatientId { get; set; } // Foreign key to Patient.PatientId (int)
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!; // Navigation property for the medical Patient

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FileType { get; set; } = string.Empty;

        [StringLength(50)]
        public string? FileContentType { get; set; }

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}