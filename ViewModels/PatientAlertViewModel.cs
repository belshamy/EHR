using System.ComponentModel.DataAnnotations;

  public class PatientAlertViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        public string? PatientName { get; set; }
        
        [Required]
        [Display(Name = "Alert Type")]
        public string AlertType { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Alert Message")]
        public string Message { get; set; } = string.Empty;
        
        [Display(Name = "Severity")]
        public string Severity { get; set; } = "Medium";
        
        [Display(Name = "Is Acknowledged")]
        public bool IsAcknowledged { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public string? AcknowledgedBy { get; set; }
        
        // Computed properties
        public string SeverityBadgeClass => Severity switch
        {
            "High" => "badge-danger",
            "Medium" => "badge-warning",
            "Low" => "badge-info",
            _ => "badge-secondary"
        };
        
        public bool IsUrgent => Severity == "High" && !IsAcknowledged;
        public string TimeAgo => GetTimeAgo(CreatedAt);
        
        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            
            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days ago";
            
            return dateTime.ToString("MMM dd, yyyy");
        }
    }
