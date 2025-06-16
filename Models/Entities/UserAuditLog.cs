using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Needed for ApplicationUser

namespace EHRsystem.Models.Entities
{
    public class UserAuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!; // Initialize: foreign key to ApplicationUser

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!; // Initialize: navigation property, assuming it's required

        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Initialize

        [Required]
        [StringLength(255)]
        public string Action { get; set; } = string.Empty; // Initialize

        [StringLength(1000)]
        public string? Details { get; set; }

        // --- ADDED PROPERTIES TO RESOLVE CS0117 ERRORS ---
        [StringLength(45)] // Standard length for IPv6, covers IPv4 too
        public string? IpAddress { get; set; }

        [StringLength(500)] // Sufficient length for most User-Agent strings
        public string? UserAgent { get; set; }
        // --- END ADDED PROPERTIES ---
    }
}
