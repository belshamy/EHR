using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EHRsystem.Models.Entities;

namespace EHRsystem.Models.Entities
{
    public class Vital
    {
        [Key]
        public int VitalId { get; set; }

        [Required]
        public string PatientId { get; set; } = string.Empty;

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;

        [Column(TypeName = "decimal(5,2)")]
        public decimal? BloodPressureSystolic { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? BloodPressureDiastolic { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? HeartRate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Temperature { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Weight { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Height { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? BMI { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? RespiratoryRate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? OxygenSaturation { get; set; }

        public DateTime RecordedDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Additional Patient reference for the second error
        public virtual Patient VitalPatient { get; set; } = null!;
    }
}
