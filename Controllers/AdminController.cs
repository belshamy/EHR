using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EHRsystem.Models.Entities;
using EHRsystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq; // Keep this as you use Select, OrderBy, Skip, Take etc.
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace EHRsystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Admin/ManageUsers
        [HttpGet]
        public async Task<IActionResult> ManageUsers(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(searchString)) || // Added null check
                    (u.LastName != null && u.LastName.Contains(searchString)) ||  // Added null check
                    (u.Email != null && u.Email.Contains(searchString))); // Added null check
            }

            var totalUsers = await usersQuery.CountAsync();
            var users = await usersQuery
                .OrderBy(u => u.Email) // Order by something predictable for pagination
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
                    FirstName = user.FirstName!,
                    LastName = user.LastName!,
                    Email = user.Email!,
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
                return BadRequest("User ID is required");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            var editUserViewModel = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                Email = user.Email!,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                IsLockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                UserRoles = userRoles.ToList(),
                AllRoles = allRoles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList()
            };

            return View(editUserViewModel);
        }

        // POST: Admin/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload roles for the view
                var allRoles = await _roleManager.Roles.ToListAsync();
                model.AllRoles = allRoles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;
            user.LockoutEnabled = model.IsLockoutEnabled;

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
                    // Reload roles for the view before returning
                    var allRolesForEmailError = await _roleManager.Roles.ToListAsync();
                    model.AllRoles = allRolesForEmailError.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
                    return View(model);
                }

                // Set UserName to match the new Email as it's often used as the login name
                await _userManager.SetUserNameAsync(user, model.Email);
                user.EmailConfirmed = false; // Reset email confirmation on email change
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Handle role changes
                if (model.SelectedRoles != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var rolesToRemove = currentRoles.Except(model.SelectedRoles);
                    var rolesToAdd = model.SelectedRoles.Except(currentRoles);

                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
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

            // Reload roles for the view in case of other update errors
            var allRolesReload = await _roleManager.Roles.ToListAsync();
            model.AllRoles = allRolesReload.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
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
            if (!ModelState.IsValid)
            {
                var allRoles = await _roleManager.Roles.ToListAsync();
                model.AllRoles = allRoles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = $"{model.FirstName} {model.LastName}".Trim(), // Populate FullName from First/Last
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true // Admin created users are automatically confirmed
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Add roles if selected
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

            var allRolesReload = await _roleManager.Roles.ToListAsync();
            model.AllRoles = allRolesReload.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
            return View(model);
        }

        // POST: Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is required");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent admin from deleting themselves
            if (user.Id == _userManager.GetUserId(User)!)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account";
                return RedirectToAction("ManageUsers");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} deleted by admin {AdminId}", id, User.Identity!.Name);
                TempData["SuccessMessage"] = "User deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user";
                _logger.LogError("Failed to delete user {UserId}", id);
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
                return BadRequest("User ID is required");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent admin from locking themselves
            if (user.Id == _userManager.GetUserId(User)!)
            {
                TempData["ErrorMessage"] = "You cannot lock your own account";
                return RedirectToAction("ManageUsers");
            }

            var lockoutEnd = DateTimeOffset.UtcNow.AddMinutes(lockoutMinutes);
            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} locked until {LockoutEnd} by admin {AdminId}",
                    id, lockoutEnd, User.Identity!.Name);
                TempData["SuccessMessage"] = $"User locked for {lockoutMinutes} minutes";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to lock user";
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
                return BadRequest("User ID is required");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} unlocked by admin {AdminId}", id, User.Identity!.Name);
                TempData["SuccessMessage"] = "User unlocked successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to unlock user";
            }

            return RedirectToAction("ManageUsers");
        }

        // GET: Admin/UserDetails/5
        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is required");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            // Removed claims related logic as ClaimViewModel is not being created
            // var claims = await _userManager.GetClaimsAsync(user); // No longer needed if not displaying claims

            var userDetailsViewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                Email = user.Email!,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                IsLockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLogin,
                Roles = roles.ToList()
                // Removed Claims assignment as ClaimViewModel is not being created
                // Claims = claims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }).ToList()
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
                TempData["ErrorMessage"] = "User ID and new password are required";
                return RedirectToAction("ManageUsers");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset for user {UserId} by admin {AdminId}", id, User.Identity!.Name);
                TempData["SuccessMessage"] = "Password reset successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reset password: " + string.Join(", ", result.Errors.Select(e => e.Description)!);
            }

            return RedirectToAction("EditUser", new { id = id });
        }
    }
}