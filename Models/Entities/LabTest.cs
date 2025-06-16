using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EHRsystem.Models.Entities
{
    public class LabTest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TestName { get; set; } = string.Empty; // Initialize

        [Required]
        [StringLength(50)]
        public string TestCode { get; set; } = string.Empty; // Initialize

        [StringLength(500)]
        public string Description { get; set; } = string.Empty; // Initialize

        [StringLength(100)]
        public string ReferenceRange { get; set; } = string.Empty; // Initialize

        [StringLength(20)]
        public string Unit { get; set; } = string.Empty; // Initialize

        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // Initialize

        [StringLength(50)]
        public string SampleType { get; set; } = string.Empty; // Initialize

        [StringLength(1000)]
        public string Instructions { get; set; } = string.Empty; // Initialize
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
