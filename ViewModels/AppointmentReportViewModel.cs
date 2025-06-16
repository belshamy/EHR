using System.ComponentModel.DataAnnotations;

public class AppointmentReportViewModel
{
    public DateTime Date { get; set; }
    public int Scheduled { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public int NoShow { get; set; }
}
