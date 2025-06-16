using System.ComponentModel.DataAnnotations;
public class DoctorSummaryViewModel
{
    public string? DoctorId { get; set; }  // Nullable - no initialization needed

    public string FullName { get; set; } = string.Empty;  // Initialize

    public string Specialization { get; set; } = string.Empty;  // Initialize

    public int YearsOfExperience { get; set; }

    public decimal Rating { get; set; }

    public bool IsAvailable { get; set; }
}
