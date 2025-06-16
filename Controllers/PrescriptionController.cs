using Microsoft.AspNetCore.Mvc;
using EHRsystem.Models.Entities;
using EHRsystem.Data;
using Microsoft.EntityFrameworkCore;

namespace EHRsystem.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Example action (adjust based on your actual code)
        public async Task<IActionResult> Index()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .ToListAsync();
            return View(prescriptions);
        }
    }
}
