// Required Using Directives (MUST be at the very top of the file, before 'namespace')
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EHRsystem.Models; // THIS IS CRUCIAL for ApplicationUser
using EHRsystem.Models.Entities; // For UserAuditLog (even if mostly used in AccountController, it's good practice if any Admin actions log)
using EHRsystem.Models.ViewModels; // THIS IS CRUCIAL for EditUserViewModel, etc.
using EHRsystem.Data; // For ApplicationDbContext
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;

namespace EHRsystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AdminController> logger,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: Admin/ManageUsers
        [HttpGet]
        public async Task<IActionResult> ManageUsers(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (u.LastName != null && u.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (u.Email != null && u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
            }

            var totalUsers = await usersQuery.CountAsync();
            var users = await usersQuery
                .OrderBy(u => u.Email)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userViewModels = new List<UserManagementViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    IsEmailConfirmed = user.EmailConfirmed,
                    IsLockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    Roles = roles.ToList()
                });
            }

            var viewModel = new ManageUsersViewModel
            {
                Users = userViewModels,
                SearchString = searchString,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalUsers = totalUsers,
                TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize)
            };

            return View(viewModel);
        }

        // GET: Admin/EditUser/5
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "User ID is required for editing.";
                return RedirectToAction("ManageUsers");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for editing", id);
                TempData["ErrorMessage"] = $"User with ID '{id}' not found.";
                return RedirectToAction("ManageUsers");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            var editUserViewModel = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                IsLockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                RequiresPasswordChange = user.RequiresPasswordChange,
                UserRoles = userRoles.ToList(), // Directly uses UserRoles from your ViewModel
                AllRoles = allRoles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name, Selected = userRoles.Contains(r.Name!) }).ToList() // Directly uses AllRoles from your ViewModel
            };

            return View(editUserViewModel);
        }

        // POST: Admin/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            // Fetch the user FIRST, and handle null immediately
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = $"User with ID '{model.Id}' not found.";
                return RedirectToAction("ManageUsers");
            }

            // Always reload roles for the view, considering the current user's roles
            var allRoles = await _roleManager.Roles.ToListAsync();
            var currentUserRoles = await _userManager.GetRolesAsync(user);
            model.AllRoles = allRoles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name, Selected = currentUserRoles.Contains(r.Name!) }).ToList(); // Directly uses AllRoles from your ViewModel

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // --- Update user properties ---
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;
            user.LockoutEnabled = model.IsLockoutEnabled;
            user.EmailConfirmed = model.IsEmailConfirmed;
            user.RequiresPasswordChange = model.RequiresPasswordChange;

            // Handle email change
            if (user.Email != model.Email)
            {
                var emailChangeResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!emailChangeResult.Succeeded)
                {
                    foreach (var error in emailChangeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                await _userManager.SetUserNameAsync(user, model.Email);
                user.EmailConfirmed = model.IsEmailConfirmed;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Handle role changes
                var selectedRoles = model.SelectedRoles ?? new List<string>();

                var rolesToRemove = currentUserRoles.Except(selectedRoles).ToList();
                var rolesToAdd = selectedRoles.Except(currentUserRoles).ToList();

                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                }
                if (rolesToAdd.Any())
                {
                    await _userManager.AddToRolesAsync(user, rolesToAdd);
                }

                _logger.LogInformation("User {UserId} updated by admin {AdminId}", user.Id, User.Identity!.Name);
                TempData["SuccessMessage"] = "User updated successfully";
                return RedirectToAction("ManageUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Admin/CreateUser
        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var allRoles = await _roleManager.Roles.ToListAsync();
            var createUserViewModel = new CreateUserViewModel
            {
                AllRoles = allRoles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList()
            };

            return View(createUserViewModel);
        }

        // POST: Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            var allRoles = await _roleManager.Roles.ToListAsync();
            model.AllRoles = allRoles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                FullName = $"{model.FirstName} {model.LastName}".Trim()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (model.SelectedRoles != null && model.SelectedRoles.Any())
                {
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                }

                _logger.LogInformation("User {UserId} created by admin {AdminId}", user.Id, User.Identity!.Name);
                TempData["SuccessMessage"] = "User created successfully";
                return RedirectToAction("ManageUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // POST: Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "User ID is required for deletion.";
                return RedirectToAction("ManageUsers");
            }

            var user = await _userManager.FindByIdAsync(id); // Corrected from FindByIdByIdAsync
            if (user == null)
            {
                TempData["ErrorMessage"] = $"User with ID '{id}' not found.";
                return RedirectToAction("ManageUsers");
            }

            if (user.Id == _userManager.GetUserId(User)!)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction("ManageUsers");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} deleted by admin {AdminId}", id, User.Identity!.Name);
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to delete user {UserId}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return RedirectToAction("ManageUsers");
        }

        // POST: Admin/LockUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id, int lockoutMinutes = 30)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "User ID is required";
                return RedirectToAction("ManageUsers");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = $"User with ID '{id}' not found.";
                return RedirectToAction("ManageUsers");
            }

            if (user.Id == _userManager.GetUserId(User)!)
            {
                TempData["ErrorMessage"] = "You cannot lock your own account.";
                return RedirectToAction("ManageUsers");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                TempData["InfoMessage"] = $"User '{user.Email}' is already locked out.";
                return RedirectToAction("ManageUsers");
            }

            var lockoutEnd = DateTimeOffset.UtcNow.AddMinutes(lockoutMinutes);
            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} locked until {LockoutEnd} by admin {AdminId}",
                    id, lockoutEnd, User.Identity!.Name);
                TempData["SuccessMessage"] = $"User '{user.Email}' locked for {lockoutMinutes} minutes.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to lock user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to lock user {UserId}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return RedirectToAction("ManageUsers");
        }

        // POST: Admin/UnlockUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "User ID is required for unlocking.";
                return RedirectToAction("ManageUsers");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = $"User with ID '{id}' not found.";
                return RedirectToAction("ManageUsers");
            }

            if (!await _userManager.IsLockedOutAsync(user))
            {
                TempData["InfoMessage"] = $"User '{user.Email}' is not currently locked out.";
                return RedirectToAction("ManageUsers");
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);

                _logger.LogInformation("User {UserId} unlocked by admin {AdminId}", id, User.Identity!.Name);
                TempData["SuccessMessage"] = "User unlocked successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to unlock user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to unlock user {UserId}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return RedirectToAction("ManageUsers");
        }

        // GET: Admin/UserDetails/5
        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "User ID is required for details.";
                return RedirectToAction("ManageUsers");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for details", id);
                TempData["ErrorMessage"] = $"User with ID '{id}' not found.";
                return RedirectToAction("ManageUsers");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDetailsViewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                IsLockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLogin,
                Roles = roles.ToList()
            };

            return View(userDetailsViewModel);
        }

        // POST: Admin/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string newPassword)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(newPassword))
            {
                TempData["ErrorMessage"] = "User ID and new password are required.";
                return RedirectToAction("EditUser", new { id = id });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = $"User with ID '{id}' not found.";
                return RedirectToAction("ManageUsers");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                await _userManager.ResetAccessFailedCountAsync(user);
                user.RequiresPasswordChange = false;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Password reset for user {UserId} by admin {AdminId}", id, User.Identity!.Name);
                TempData["SuccessMessage"] = "Password reset successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reset password: " + string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to reset password for user {UserId}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return RedirectToAction("EditUser", new { id = id });
        }


        // GET: /Admin/ManageRoles
        [HttpGet]
        public IActionResult ManageRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // GET: /Admin/CreateRole
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        // POST: /Admin/CreateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError(string.Empty, "Role name cannot be empty.");
                return View();
            }

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                ModelState.AddModelError(string.Empty, $"Role '{roleName}' already exists.");
                return View();
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Role '{roleName}' created successfully.";
                return RedirectToAction(nameof(ManageRoles));
            }

            AddErrors(result);
            return View();
        }

        // GET: /Admin/DeleteRole/{id}
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: /Admin/DeleteRoleConfirmed/{id}
        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoleConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Check if any users are in this role before deleting (optional but good practice)
            // This line now correctly uses the null-forgiving operator '!'
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete role '{role.Name}' because there are {usersInRole.Count} users assigned to it. Please remove users from this role first.";
                return View(role); // Re-display the view with error
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Role '{role.Name}' deleted successfully.";
                return RedirectToAction(nameof(ManageRoles));
            }

            AddErrors(result);
            return View(role);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        #endregion
    }
}