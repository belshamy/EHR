using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHRsystem.Models.Entities
{
    public class DoctorSchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; } // Foreign key to Doctor entity's int Id
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!; // Initialize: Navigation property for Doctor

        public DayOfWeek DayOfWeek { get; set; } // Enum for day of the week

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; } // Start time for the slot

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; } // End time for the slot

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
