// F:\EHRsystem\Models\ViewModels\ManageUsersViewModel.cs
using EHRsystem.Models.Entities; // Still good to include if you reference ApplicationUser elsewhere in ViewModels
using System.Collections.Generic;

namespace EHRsystem.Models.ViewModels
{
    public class ManageUsersViewModel
    {
        // This list should hold the individual UserManagementViewModel objects
        public List<UserManagementViewModel> Users { get; set; } = new List<UserManagementViewModel>(); 
        
        // Pagination Properties (as used in AdminController)
        public string? SearchString { get; set; } // Corrected from SearchTerm for controller consistency
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalUsers { get; set; }
        public int TotalPages { get; set; } // Derived from TotalUsers and PageSize

        // New properties you added (will need to be populated in AdminController if used)
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        
        public string? SelectedRole { get; set; } // Made nullable
        public List<string> AvailableRoles { get; set; } = new List<string>();
    }
}