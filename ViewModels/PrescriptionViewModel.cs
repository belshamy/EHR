using System.ComponentModel.DataAnnotations;
 public class PrescriptionViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public string DoctorId { get; set; } = string.Empty;
        
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        
        [Required]
        [Display(Name = "Medication Name")]
        public string MedicationName { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Dosage")]
        public string Dosage { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Frequency")]
        public string Frequency { get; set; } = string.Empty;
        
        [Display(Name = "Instructions")]
        public string? Instructions { get; set; }
        
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
        
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        
        [Display(Name = "Refills Remaining")]
        public int RefillsRemaining { get; set; }
        
        public string Status { get; set; } = "Active";
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Computed properties
        public bool IsActive => Status == "Active" && (EndDate == null || EndDate > DateTime.Now);
        public bool NeedsRefill => RefillsRemaining <= 1;
        public string StatusBadgeClass => Status switch
        {
            "Active" => "badge-success",
            "Completed" => "badge-info",
            "Cancelled" => "badge-danger",
            "Expired" => "badge-warning",
            _ => "badge-secondary"
        };
    }
