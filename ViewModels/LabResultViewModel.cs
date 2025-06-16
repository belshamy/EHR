using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class LabTestOrderViewModel
{
    [Required]
    public string PatientId { get; set; } = string.Empty;

    [Required]
    public string TestType { get; set; } = string.Empty;

    [Required]
    public DateTime OrderedDate { get; set; } = DateTime.Now;

    public DateTime? ScheduledDate { get; set; }

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;

    public List<SelectListItem>?AvailableTests { get; set; } = new();
}

public class LabResultViewModel
{
    public int TestId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public DateTime TestDate { get; set; }
    public string Result { get; set; } = string.Empty;
    public string NormalRange { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string OrderedBy { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
