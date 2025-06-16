using System.ComponentModel.DataAnnotations;

namespace EHRsystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalMedicalRecords { get; set; }
        
        public List<AppointmentSummaryViewModel> RecentAppointments { get; set; } = new List<AppointmentSummaryViewModel>();
        public List<MedicalRecordSummaryViewModel> RecentMedicalRecords { get; set; } = new List<MedicalRecordSummaryViewModel>();
    }

    public class AppointmentSummaryViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }
        
        [Display(Name = "Patient")]
        public string PatientName { get; set; } = string.Empty;
        
        [Display(Name = "Doctor")]
        public string DoctorName { get; set; } = string.Empty;
        
        public string Status { get; set; } = string.Empty;
    }

    public class MedicalRecordSummaryViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Record Date")]
        public DateTime RecordDate { get; set; }
        
        [Display(Name = "Patient")]
        public string PatientName { get; set; } = string.Empty;
        
        [Display(Name = "Doctor")]
        public string DoctorName { get; set; } = string.Empty;
        
        [Display(Name = "Chief Complaint")]
        public string ChiefComplaint { get; set; } = string.Empty;
    }
}