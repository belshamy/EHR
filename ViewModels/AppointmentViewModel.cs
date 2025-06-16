namespace EHRsystem.ViewModels
{
    public class AppointmentViewModel
    {
        // Initialized to prevent CS8618 warnings
        public string PatientName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Assuming Status is a string for the ViewModel
        // Add any other properties this ViewModel is expected to have and initialize them
    }
}
