namespace EHRsystem.ViewModels
{
    public class HealthSummaryViewModel
    {
        // Initialized to prevent CS8618 warnings
        public string Diagnosis { get; set; } = string.Empty;
        public string TreatmentPlan { get; set; } = string.Empty;
        // Add any other properties this ViewModel is expected to have and initialize them
    }
}
