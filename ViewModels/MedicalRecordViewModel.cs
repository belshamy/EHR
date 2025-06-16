using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
public class MedicalRecordViewModel
{
    public int Id { get; set; }

    [Required]
    public string PatientId { get; set; } = string.Empty;

    [Required]
    public string VisitReason { get; set; } = string.Empty;

    [Required]
    public string Diagnosis { get; set; } = string.Empty;

    public string TreatmentPlan { get; set; } = string.Empty;

    public string PrescribedMedications { get; set; } = string.Empty;

    public DateTime RecordDate { get; set; } = DateTime.Now;

    // For dropdowns
    public List<SelectListItem> Patients { get; set; } = new();
}
