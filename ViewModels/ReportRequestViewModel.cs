using System.ComponentModel.DataAnnotations;
public class ReportRequestViewModel
{
    [Required]
    public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-1);

    [Required]
    public DateTime EndDate { get; set; } = DateTime.Today;

    [Required]
    public string ReportType { get; set; } = "Appointments";

    public string Department { get; set; } = "All";
    public string DoctorId { get; set; } = "All";
}
