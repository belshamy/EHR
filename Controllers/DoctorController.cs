// F:\EHRsystem\Controllers\DoctorController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EHRsystem.Data;
using EHRsystem.Models.Entities;
using EHRsystem.Models.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using EHRsystem.ViewModels; // <--- THIS IS THE CRUCIAL LINE FOR HEALTHSUMMARYVIEWMODEL

namespace EHRsystem.Controllers
{
    [Authorize(Roles = "Doctor,Admin")]
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<DoctorController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public class DoctorDashboardViewModel
        {
            public Doctor Doctor { get; set; } = null!;
            public List<Appointment> TodaysAppointments { get; set; } = new List<Appointment>();
            public List<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
            public List<MedicalRecord> RecentMedicalRecords { get; set; } = new List<MedicalRecord>();
            public DoctorStatsViewModel Stats { get; set; } = new DoctorStatsViewModel(); 
            public List<string> PatientAlerts { get; set; } = new List<string>();
        }

        public class DoctorProfileViewModel
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty; 
            public string Specialization { get; set; } = string.Empty;
            public string LicenseNumber { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string Biography { get; set; } = string.Empty;
            public string Education { get; set; } = string.Empty;
            public string Certifications { get; set; } = string.Empty;
            public List<SelectListItem> Specializations { get; set; } = new List<SelectListItem>(); 
            public bool IsActive { get; set; } = true;
            public string UserId { get; set; } = string.Empty;
        }

        public class DoctorPatientsViewModel
        {
            public List<Patient> Patients { get; set; } = new List<Patient>(); 
            public string SearchString { get; set; } = string.Empty; 
            public string FilterBy { get; set; } = string.Empty;
            public string SortBy { get; set; } = string.Empty;
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
            public int TotalPatients { get; set; }
            public int TotalPages { get; set; }
        }

        public class PatientOverviewViewModel
        {
            public Patient Patient { get; set; } = null!;
            public List<Appointment> Appointments { get; set; } = new List<Appointment>();
            public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
            public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
            public List<LabResult> LabResults { get; set; } = new List<LabResult>();
            public HealthSummaryViewModel HealthSummary { get; set; } = new HealthSummaryViewModel(); 
        }

        public class PatientProfileForDoctorViewModel
        {
            public Patient Patient { get; set; } = null!;
            public List<Appointment> Appointments { get; set; } = new List<Appointment>();
            public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
            public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
            public List<LabResult> LabResults { get; set; } = new List<LabResult>();
            public HealthSummaryViewModel HealthSummary { get; set; } = new HealthSummaryViewModel(); 
            public Doctor Doctor { get; set; } = null!;
        }

        public class DoctorStatsViewModel
        {
            public int TotalPatientsAssigned { get; set; }
            public int CompletedAppointmentsLastMonth { get; set; }
            public decimal AverageRating { get; set; }
        }

        // GET: Doctor/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var doctor = await _context.Doctors
                .Include(d => d.Appointments) 
                    .ThenInclude(a => a.Patient)
                .Include(d => d.MedicalRecords) 
                    .ThenInclude(mr => mr.Patient)
                .FirstOrDefaultAsync(d => d.UserId == currentUser.Id);

            if (doctor == null) return NotFound("Doctor profile not found.");

            var today = DateTime.Today;
            var upcomingAppointments = doctor.Appointments
                                            .Where(a => a.AppointmentDate.Date >= today)
                                            .OrderBy(a => a.AppointmentDate)
                                            .ToList();

            var recentMedicalRecords = doctor.MedicalRecords
                                            .OrderByDescending(mr => mr.RecordDate)
                                            .Take(5)
                                            .ToList();

            var stats = new DoctorStatsViewModel
            {
                TotalPatientsAssigned = await _context.Patients
                    .CountAsync(p => p.User != null && p.User.PatientAppointments != null && p.User.PatientAppointments.Any(a => a.DoctorId == doctor.Id)),
                
                CompletedAppointmentsLastMonth = await _context.Appointments
                                                            .CountAsync(a => a.DoctorId == doctor.Id &&
                                                                             a.Status == AppointmentStatus.Completed &&
                                                                             a.AppointmentDate >= DateTime.Today.AddMonths(-1)),
                AverageRating = 4.5m 
            };


            var viewModel = new DoctorDashboardViewModel
            {
                Doctor = doctor, 
                TodaysAppointments = upcomingAppointments.Where(a => a.AppointmentDate.Date == today).ToList(),
                UpcomingAppointments = upcomingAppointments.Where(a => a.AppointmentDate.Date > today).ToList(),
                RecentMedicalRecords = recentMedicalRecords,
                Stats = stats,
                PatientAlerts = new List<string>() 
            };

            return View(viewModel);
        }

        // ... (rest of your DoctorController methods) ...
    }
}
