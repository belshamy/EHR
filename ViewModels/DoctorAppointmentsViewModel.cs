using EHRsystem.Models.Entities;
using EHRsystem.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EHRsystem.ViewModels
{
    public class DoctorAppointmentsViewModel
    {
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();  // Initialize
        
        public AppointmentStatus? StatusFilter { get; set; }  // Nullable - no initialization needed
        public DateTime? DateFilter { get; set; }  // Nullable - no initialization needed
        
        public int PageNumber { get; set; } = 1;  // Default to first page
        public int PageSize { get; set; } = 10;  // Default page size
        
        public int TotalAppointments { get; set; }
        
        public int TotalPages =>  // Calculate dynamically
            (int)Math.Ceiling((double)TotalAppointments / PageSize);
    }
}
