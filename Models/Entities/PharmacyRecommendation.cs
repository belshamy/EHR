using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHRsystem.Models.Entities
{
    public class PharmacyRecommendation
    {
        [Key]
        public int RecommendationId { get; set; }

        [Required]
        public int PrescriptionId { get; set; }

        [ForeignKey("PrescriptionId")]
        public virtual Prescription Prescription { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string PharmacyName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? EstimatedPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
