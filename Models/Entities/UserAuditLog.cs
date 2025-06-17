using System;
using System.ComponentModel.DataAnnotations;

namespace EHRsystem.Models.Entities
{
    public class UserAuditLog
    {
        public int Id { get; set; } // Primary Key
        public string UserId { get; set; } = string.Empty; // User who performed the action, or "N/A"
        public DateTime Timestamp { get; set; }

        [Required]
        [StringLength(100)]
        public string ActivityType { get; set; } = string.Empty; // e.g., "Login", "Logout", "UserCreated", "PasswordReset"

        [StringLength(500)]
        public string Details { get; set; } = string.Empty; // More specific info about the activity

        [StringLength(50)]
        public string? IpAddress { get; set; }

        // Navigation property to the ApplicationUser (optional, can be null for failed logins)
        public ApplicationUser? User { get; set; }
    }
}