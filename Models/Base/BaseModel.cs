using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHRsystem.Models.Base
{
    public abstract class BaseModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Use UTC for consistency
        
        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedAt { get; set; }
        
        [Required]
        public bool IsDeleted { get; set; } = false;
        
        // Optional: Track who made changes (useful for audit trails in medical systems)
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
        
        // Optional: For optimistic concurrency control
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
    
    // Alternative base model with string-based primary key (useful for some medical systems)
    public abstract class BaseModelWithStringId
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedAt { get; set; }
        
        [Required]
        public bool IsDeleted { get; set; } = false;
        
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
        
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
    
    // Specialized base model for medical records that require additional audit information
    public abstract class MedicalRecordBase : BaseModel
    {
        [MaxLength(100)]
        public string? LastModifiedByRole { get; set; } // Doctor, Nurse, Admin, etc.
        
        [Column(TypeName = "datetime2")]
        public DateTime? LastAccessedAt { get; set; }
        
        [MaxLength(100)]
        public string? LastAccessedBy { get; set; }
        
        // For HIPAA compliance - track access patterns
        public int AccessCount { get; set; } = 0;
        
        // Medical record status
        [MaxLength(50)]
        public string Status { get; set; } = "Active"; // Active, Archived, Locked, etc.
    }
}
