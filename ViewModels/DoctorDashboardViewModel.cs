using System.Collections.Generic;
using EHRsystem.Models.Entities; // Needed for Doctor, Appointment, MedicalRecord
// Removed the redundant using EHRsystem.ViewModels as classes are now in the same namespace or defined below.

namespace EHRsystem.ViewModels
{
    public class DoctorDashboardViewModel
    {
        // Initialized to prevent CS8618 warnings
        public Doctor Doctor { get; set; } = null!; // Assuming Doctor is an entity, needs initialization. Adjust if nullable (Doctor?)
        public List<AppointmentViewModel> UpcomingAppointments { get; set; } = new List<AppointmentViewModel>(); 
        public List<string> PatientAlerts { get; set; } = new List<string>(); 
        public HealthSummaryViewModel CurrentPatientHealthSummary { get; set; } = new HealthSummaryViewModel(); 
        // Add any other properties this ViewModel is expected to have and initialize them
        public List<Appointment> TodaysAppointments { get; set; } = new List<Appointment>();
        public List<MedicalRecord> RecentMedicalRecords { get; set; } = new List<MedicalRecord>();
        public DoctorStatsViewModel Stats { get; set; } = new DoctorStatsViewModel(); // DoctorStatsViewModel is now defined below
    }

    // DoctorStatsViewModel definition moved here to resolve CS0246 error
    public class DoctorStatsViewModel
    {
        public int TotalPatientsAssigned { get; set; }
        public int CompletedAppointmentsLastMonth { get; set; }
        public decimal AverageRating { get; set; }
        // Add other properties for stats, initialize any non-nullable ones
    }

    // HealthSummaryViewModel and AppointmentViewModel are assumed to be defined either in their own files
    // within this same EHRsystem.ViewModels namespace, or are also nested here if tightly coupled.
    // Ensure HealthSummaryViewModel is correctly defined and initialized, e.g.:
    // public class HealthSummaryViewModel { public string Diagnosis { get; set; } = string.Empty; public string TreatmentPlan { get; set; } = string.Empty; }
    // Ensure AppointmentViewModel is correctly defined and initialized, e.g.:
    // public class AppointmentViewModel { public int Id { get; set; } public string PatientName { get; set; } = string.Empty; ... }
}
