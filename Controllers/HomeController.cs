using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // Needed if you plan to access User Manager directly
using Microsoft.AspNetCore.Authorization; // Needed if you plan to use [Authorize]
using EHRsystem.Models.Entities;
using EHRsystem.Data; // Ensure this points to where ApplicationDbContext is defined
using EHRsystem.Models.ViewModels; // Ensure this points to where your ViewModels are defined
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using System; // For DateTime, Exception
using System.Collections.Generic; // For List

namespace EHRsystem.Controllers
{
    public class HomeController : Controller
    {
        // FIX: Changed EHRDbContext to ApplicationDbContext
        private readonly ApplicationDbContext _context; 
        private readonly ILogger<HomeController> _logger;
        // private readonly UserManager<ApplicationUser> _userManager; // Uncomment if needed later for user-specific data

        public HomeController(
            ApplicationDbContext context, 
            ILogger<HomeController> logger
            /*, UserManager<ApplicationUser> userManager */ // Uncomment if you uncomment the private readonly field
            )
        {
            _context = context;
            _logger = logger;
            // _userManager = userManager; // Assign if uncommented
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Create a ViewModel with dashboard data
                var dashboardData = new DashboardViewModel
                {
                    TotalPatients = await _context.Patients.CountAsync(),
                    TotalDoctors = await _context.Doctors.CountAsync(),
                    TotalAppointments = await _context.Appointments.CountAsync(),
                    TotalMedicalRecords = await _context.MedicalRecords.CountAsync(),
                    
                    // Get recent appointments with related data
                    RecentAppointments = await _context.Appointments
                        .Include(a => a.Patient) // Ensure Patient navigation property is loaded
                        .Include(a => a.Doctor)   // Ensure Doctor navigation property is loaded
                        .OrderByDescending(a => a.AppointmentDate)
                        .Take(5)
                        .Select(a => new AppointmentSummaryViewModel
                        {
                            Id = a.Id,
                            AppointmentDate = a.AppointmentDate,
                            // Ensure Patient and Doctor are not null before accessing Name.
                            // Assuming Patient has a Name property (or combine FirstName/LastName).
                            PatientName = a.Patient != null ? $"{a.Patient.FirstName} {a.Patient.LastName}" : "Unknown Patient",
                            // Assuming Doctor has a Name property (or combine FirstName/LastName).
                            DoctorName = a.Doctor != null ? $"{a.Doctor.FirstName} {a.Doctor.LastName}" : "Unknown Doctor",
                            Status = a.Status.ToString() // Convert enum to string
                        })
                        .ToListAsync(),

                    // Get recent medical records
                    RecentMedicalRecords = await _context.MedicalRecords
                        .Include(mr => mr.Patient) // Ensure Patient navigation property is loaded
                        .Include(mr => mr.Doctor)   // Ensure Doctor navigation property is loaded
                        .OrderByDescending(mr => mr.RecordDate)
                        .Take(5)
                        .Select(mr => new MedicalRecordSummaryViewModel
                        {
                            Id = mr.Id,
                            RecordDate = mr.RecordDate,
                            // Ensure Patient and Doctor are not null before accessing Name.
                            PatientName = mr.Patient != null ? $"{mr.Patient.FirstName} {mr.Patient.LastName}" : "Unknown Patient",
                            DoctorName = mr.Doctor != null ? $"{mr.Doctor.FirstName} {mr.Doctor.LastName}" : "Unknown Doctor",
                            ChiefComplaint = mr.ChiefComplaint ?? "No complaint recorded"
                        })
                        .ToListAsync()
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                
                // Return an empty ViewModel to prevent null reference exceptions in the view
                var emptyDashboard = new DashboardViewModel
                {
                    TotalPatients = 0,
                    TotalDoctors = 0,
                    TotalAppointments = 0,
                    TotalMedicalRecords = 0,
                    RecentAppointments = new List<AppointmentSummaryViewModel>(),
                    RecentMedicalRecords = new List<MedicalRecordSummaryViewModel>()
                };

                // You might want to show an error message to the user
                ViewBag.ErrorMessage = "Unable to load dashboard data. Please try again later.";
                
                return View(emptyDashboard);
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
