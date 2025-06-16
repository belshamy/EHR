using System.ComponentModel.DataAnnotations;

namespace EHRsystem.ViewModels
{
    public class HomeIndexViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new List<RecentActivityViewModel>();
        public List<UpcomingAppointmentViewModel> UpcomingAppointments { get; set; } = new List<UpcomingAppointmentViewModel>();
        public List<DoctorSummaryViewModel> AvailableDoctors { get; set; } = new List<DoctorSummaryViewModel>();
    }

    public class DoctorSummaryViewModel
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; } = string.Empty;  // Initialize
        public string Specialization { get; set; } = string.Empty;  // Initialize
        public int YearsOfExperience { get; set; }
        public decimal Rating { get; set; }
        public bool IsAvailable { get; set; }
        public string? ProfilePicture { get; set; }  // Nullable - no initialization needed
    }

    public class RecentActivityViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;  // Initialize
        public string Title { get; set; } = string.Empty;  // Initialize
        public string Description { get; set; } = string.Empty;  // Initialize
        public DateTime Date { get; set; }
        public string Icon { get; set; } = string.Empty;  // Initialize
    }

    public class UpcomingAppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;  // Initialize
        public string DoctorName { get; set; } = string.Empty;  // Initialize
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Status { get; set; } = string.Empty;  // Initialize
        public string? Reason { get; set; }  // Nullable - no initialization needed

        [Display(Name = "Appointment Date & Time")]
        public DateTime AppointmentDateTime => AppointmentDate.Add(AppointmentTime);
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }  // Nullable - no initialization needed
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
