using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHRsystem.Models.Entities
{
    public class Diagnosis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string DiagnosisCode { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string DiagnosisName { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime DiagnosisDate { get; set; }

        [StringLength(50)]
        public string? Severity { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;
    }
}
