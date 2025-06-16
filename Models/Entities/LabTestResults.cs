    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using EHRsystem.Models.Entities; // Ensure LabTest entity is accessible

    namespace EHRsystem.Models.Entities
    {
        public class LabTestResults
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public int LabTestId { get; set; }
            [ForeignKey("LabTestId")]
            public virtual LabTest LabTest { get; set; } = null!; // Initialized to prevent CS8618

            [Required]
            [StringLength(255)]
            public string ResultValue { get; set; } = string.Empty; // Initialized to prevent CS8618

            [StringLength(1000)]
            public string Notes { get; set; } = string.Empty; // Initialized to prevent CS8618
            
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        }
    }
    