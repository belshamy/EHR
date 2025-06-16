using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EHRsystem.Data;
using EHRsystem.Models.Entities;
using EHRsystem.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations; // <--- ADD THIS LINE

namespace EHRsystem.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<AppointmentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // ViewModel for Index action
        public class AppointmentIndexViewModel
        {
            public List<Appointment> Appointments { get; set; } = new List<Appointment>();
            public string SearchString { get; set; } = "";
            public string StatusFilter { get; set; } = "";
            public DateTime? DateFilter { get; set; }
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
            public int TotalAppointments { get; set; }
            public int TotalPages { get; set; }
            public string UserRole { get; set; } = "";
        }

        // ViewModel for Create action
        public class CreateAppointmentViewModel
        {
            // This PatientId now represents the int PatientId of the medical Patient entity,
            // but is kept as string here to be compatible with HTML <select> element's value.
            [Required(ErrorMessage = "Patient is required.")]
            public string PatientId { get; set; } = ""; 
            public string PatientName { get; set; } = ""; // Used for display, not data binding
            
            [Required(ErrorMessage = "Doctor is required.")]
            public int DoctorId { get; set; }
            public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
            public List<SelectListItem> Patients { get; set; } = new List<SelectListItem>(); // Represents medical Patients
            
            [Required(ErrorMessage = "Appointment date is required.")]
            [DataType(DataType.DateTime)]
            public DateTime AppointmentDate { get; set; }
            
            [Required(ErrorMessage = "Reason is required.")]
            [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
            public string Reason { get; set; } = "";
            
            [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
            public string? Notes { get; set; }
            public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
        }

        // ViewModel for Edit action
        public class EditAppointmentViewModel
        {
            public int Id { get; set; }
            
            // PatientId from the medical Patient entity, kept as string for HTML form
            public string PatientId { get; set; } = ""; 
            public string PatientName { get; set; } = ""; // For display
            
            [Required(ErrorMessage = "Doctor is required.")]
            public int DoctorId { get; set; }
            public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
            
            [Required(ErrorMessage = "Appointment date is required.")]
            [DataType(DataType.DateTime)]
            public DateTime AppointmentDate { get; set; }
            
            [Required(ErrorMessage = "Reason is required.")]
            [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
            public string Reason { get; set; } = "";
            
            [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
            public string? Notes { get; set; }
            public AppointmentStatus Status { get; set; }
            public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
        }

        // GET: Appointment
        public async Task<IActionResult> Index(string searchString, string statusFilter, DateTime? dateFilter, int pageNumber = 1, int pageSize = 10)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var appointmentsQuery = _context.Appointments
                .Include(a => a.Patient) // Ensure Patient entity is loaded
                .Include(a => a.Doctor)
                .AsQueryable();

            // Filter based on user role
            if (!userRoles.Contains("Admin") && !userRoles.Contains("Doctor"))
            {
                // If the current user is a patient (ApplicationUser),
                // find their associated medical Patient entity's ID.
                var patientProfile = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == currentUser.Id);

                if (patientProfile != null)
                {
                    appointmentsQuery = appointmentsQuery.Where(a => a.PatientId == patientProfile.PatientId);
                }
                else
                {
                    // If a patient-role user has no associated medical Patient entity,
                    // they shouldn't see any appointments.
                    _logger.LogWarning("User {UserId} has 'Patient' role but no associated Patient medical entity found.", currentUser.Id);
                    appointmentsQuery = appointmentsQuery.Where(a => false); // Return no appointments
                }
            }
            else if (userRoles.Contains("Doctor") && !userRoles.Contains("Admin"))
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.UserId == currentUser.Id);

                if (doctor != null)
                {
                    appointmentsQuery = appointmentsQuery.Where(a => a.DoctorId == doctor.Id);
                }
                else
                {
                    _logger.LogWarning("Doctor user {UserId} has no associated Doctor entity found.", currentUser.Id);
                    appointmentsQuery = appointmentsQuery.Where(a => false); // Doctor should not see appointments if no profile
                }
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                appointmentsQuery = appointmentsQuery.Where(a =>
                    (a.Patient != null && (a.Patient.FirstName!.Contains(searchString) || a.Patient.LastName!.Contains(searchString))) ||
                    (a.Doctor != null && (a.Doctor.FirstName!.Contains(searchString) || a.Doctor.LastName!.Contains(searchString))) ||
                    a.Reason.Contains(searchString));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<AppointmentStatus>(statusFilter, out var status))
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.Status == status);
            }

            // Apply date filter
            if (dateFilter.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.AppointmentDate.Date == dateFilter.Value.Date);
            }

            // Order by appointment date
            appointmentsQuery = appointmentsQuery.OrderBy(a => a.AppointmentDate);

            var totalAppointments = await appointmentsQuery.CountAsync();
            var appointments = await appointmentsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new AppointmentIndexViewModel
            {
                Appointments = appointments,
                SearchString = searchString,
                StatusFilter = statusFilter,
                DateFilter = dateFilter,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalAppointments = totalAppointments,
                TotalPages = (int)Math.Ceiling((double)totalAppointments / pageSize),
                UserRole = userRoles.Any() ? userRoles.First() : "Patient" // Default to Patient if no roles
            };

            return View(viewModel);
        }

        // GET: Appointment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null) return NotFound();

            if (!await CanAccessAppointment(appointment))
            {
                return Forbid();
            }

            return View(appointment);
        }

        // GET: Appointment/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var viewModel = new CreateAppointmentViewModel();

            // Populate doctors dropdown
            await PopulateDropdownsForCreate(viewModel, userRoles.Contains("Admin"));

            // If user is not admin, pre-select the patient automatically
            if (!userRoles.Contains("Admin"))
            {
                var patientProfile = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == currentUser.Id);
                if (patientProfile != null)
                {
                    viewModel.PatientId = patientProfile.PatientId.ToString();
                    viewModel.PatientName = $"{patientProfile.FirstName} {patientProfile.LastName}";
                }
                else
                {
                    ModelState.AddModelError("", "Your patient profile could not be found. Please contact support.");
                }
            }
            // For admin, PatientId will be selected from the dropdown
            
            viewModel.AppointmentDate = DateTime.Now.AddHours(1); // Default to 1 hour from now

            return View(viewModel);
        }

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAppointmentViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            // Attempt to get the PatientId (int) from the model's string PatientId
            int patientIdInt;
            if (userRoles.Contains("Admin"))
            {
                if (!int.TryParse(model.PatientId, out patientIdInt))
                {
                    ModelState.AddModelError("PatientId", "Invalid Patient selected.");
                }
            }
            else // Regular user (patient role)
            {
                var patientProfile = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == currentUser.Id);
                if (patientProfile == null)
                {
                    ModelState.AddModelError("", "Your patient profile could not be found. Please contact support.");
                    patientIdInt = 0; // Indicate failure to find patient ID
                }
                else
                {
                    patientIdInt = patientProfile.PatientId;
                }
            }

            if (ModelState.IsValid && patientIdInt > 0) // Ensure patientIdInt is valid
            {
                // Validate appointment time
                if (model.AppointmentDate <= DateTime.Now)
                {
                    ModelState.AddModelError("AppointmentDate", "Appointment date must be in the future.");
                    await PopulateDropdownsForCreate(model, userRoles.Contains("Admin"));
                    return View(model);
                }

                // Check if doctor is available at the requested time
                var conflictingAppointment = await _context.Appointments
                    .AnyAsync(a => a.DoctorId == model.DoctorId &&
                                 a.AppointmentDate == model.AppointmentDate &&
                                 a.Status != AppointmentStatus.Cancelled);

                if (conflictingAppointment)
                {
                    ModelState.AddModelError("AppointmentDate", "Doctor is not available at the selected time.");
                    await PopulateDropdownsForCreate(model, userRoles.Contains("Admin"));
                    return View(model);
                }

                var appointment = new Appointment
                {
                    PatientId = patientIdInt, // Assign the int PatientId
                    DoctorId = model.DoctorId,
                    AppointmentDate = model.AppointmentDate,
                    Reason = model.Reason,
                    Notes = model.Notes,
                    Status = AppointmentStatus.Scheduled,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Appointment created: {AppointmentId} by user {UserId}", appointment.Id, currentUser.Id);

                TempData["SuccessMessage"] = "Appointment created successfully!";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsForCreate(model, userRoles.Contains("Admin"));
            return View(model);
        }

        // GET: Appointment/Edit/5
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound();

            if (!await CanAccessAppointment(appointment))
            {
                return Forbid();
            }

            var viewModel = new EditAppointmentViewModel
            {
                Id = appointment.Id,
                // Convert int PatientId to string for the ViewModel (for dropdown/form compatibility)
                PatientId = appointment.PatientId.ToString(), 
                PatientName = $"{appointment.Patient?.FirstName} {appointment.Patient?.LastName}",
                DoctorId = appointment.DoctorId,
                AppointmentDate = appointment.AppointmentDate,
                Reason = appointment.Reason,
                Notes = appointment.Notes, // Include notes in edit
                Status = appointment.Status
            };

            await PopulateDropdownsForEdit(viewModel);
            return View(viewModel);
        }

        // POST: Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Edit(int id, EditAppointmentViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var appointment = await _context.Appointments.FindAsync(id);
                    if (appointment == null) return NotFound();

                    if (!await CanAccessAppointment(appointment))
                    {
                        return Forbid();
                    }

                    // Update appointment properties
                    appointment.DoctorId = model.DoctorId;
                    appointment.AppointmentDate = model.AppointmentDate;
                    appointment.Reason = model.Reason;
                    appointment.Notes = model.Notes;
                    appointment.Status = model.Status;
                    appointment.UpdatedAt = DateTime.UtcNow;

                    _context.Update(appointment);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Appointment updated: {AppointmentId} by user {UserId}",
                        appointment.Id, User.Identity?.Name ?? "UnknownUser");

                    TempData["SuccessMessage"] = "Appointment updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await PopulateDropdownsForEdit(model);
            return View(model);
        }

        // GET: Appointment/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Appointment deleted: {AppointmentId} by user {UserId}",
                    id, User.Identity?.Name ?? "UnknownUser");

                TempData["SuccessMessage"] = "Appointment deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Appointment/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string cancellationReason)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            if (!await CanAccessAppointment(appointment))
            {
                return Forbid();
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.Notes = string.IsNullOrEmpty(appointment.Notes)
                ? $"Cancelled: {cancellationReason}"
                : $"{appointment.Notes}\nCancelled: {cancellationReason}";
            appointment.UpdatedAt = DateTime.UtcNow;

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Appointment cancelled: {AppointmentId} by user {UserId}",
                id, User.Identity?.Name ?? "UnknownUser");

            TempData["SuccessMessage"] = "Appointment cancelled successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointment/MyAppointments (for patients)
        [Authorize(Roles = "Patient")] // Ensure only patients can access this specific view
        public async Task<IActionResult> MyAppointments()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            // Find the medical Patient entity associated with the current ApplicationUser
            var patientProfile = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == currentUser.Id);

            if (patientProfile == null)
            {
                _logger.LogWarning("User {UserId} attempted to view appointments but has no associated Patient medical entity.", currentUser.Id);
                // Return an empty list or a message if the patient profile is not found
                TempData["ErrorMessage"] = "Your patient profile could not be found. Please contact support.";
                return View(new List<Appointment>()); 
            }

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientProfile.PatientId) // Filter by the int PatientId
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        // GET: Appointment/DoctorSchedule (for doctors)
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> DoctorSchedule(DateTime? date)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == currentUser.Id);
            if (doctor == null && !userRoles.Contains("Admin"))
            {
                return NotFound("Doctor profile not found.");
            }

            var selectedDate = date ?? DateTime.Today;
            var appointmentsQuery = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.AppointmentDate.Date == selectedDate.Date);

            if (!userRoles.Contains("Admin") && doctor != null)
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.DoctorId == doctor.Id);
            }

            var appointments = await appointmentsQuery
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            ViewBag.SelectedDate = selectedDate;
            return View(appointments);
        }

        #region Private Methods

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        private async Task<bool> CanAccessAppointment(Appointment appointment)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return false;

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            // Admin can access all appointments
            if (userRoles.Contains("Admin")) return true;

            // Patient can access their own appointments (via their associated Patient entity)
            var patientProfile = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == currentUser.Id);
            if (patientProfile != null && appointment.PatientId == patientProfile.PatientId) return true;

            // Doctor can access appointments assigned to them
            if (userRoles.Contains("Doctor"))
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.UserId == currentUser.Id);

                if (doctor != null && appointment.DoctorId == doctor.Id)
                    return true;
            }

            return false;
        }

        private async Task PopulateDropdownsForCreate(CreateAppointmentViewModel model, bool isAdmin)
        {
            var doctors = await _context.Doctors
                .Where(d => d.IsActive)
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .ToListAsync();

            model.Doctors = doctors.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(), // Doctor.Id is int, convert to string for SelectListItem
                Text = $"Dr. {d.FirstName} {d.LastName} - {d.Specialization}"
            }).ToList();

            if (isAdmin)
            {
                // For admin, populate the patients dropdown with actual medical Patient entities
                var patients = await _context.Patients
                    .OrderBy(p => p.FirstName)
                    .ThenBy(p => p.LastName)
                    .ToListAsync();

                model.Patients = patients.Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(), // Patient.PatientId is int, convert to string
                    Text = $"{p.FirstName} {p.LastName} (DOB: {p.DateOfBirth:yyyy-MM-dd})"
                }).ToList();
            }
        }

        private async Task PopulateDropdownsForEdit(EditAppointmentViewModel model)
        {
            var doctors = await _context.Doctors
                .Where(d => d.IsActive)
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .ToListAsync();

            model.Doctors = doctors.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"Dr. {d.FirstName} {d.LastName} - {d.Specialization}",
                Selected = d.Id == model.DoctorId
            }).ToList();

            model.StatusOptions = Enum.GetValues(typeof(AppointmentStatus))
                .Cast<AppointmentStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = s == model.Status
                }).ToList();
        }

        #endregion
    }
}