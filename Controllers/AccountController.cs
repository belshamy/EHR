using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using EHRsystem.Models;
using EHRsystem.Data;
using EHRsystem.Models.Entities;
using EHRsystem.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

// Define a placeholder IEmailSender if you don't have one configured yet,
// or ensure your actual IEmailSender service is correctly implemented and injected in Program.cs.
public interface IEmailSender
{
    Task SendConfirmationEmail(string email, string callbackUrl);
    Task SendPasswordResetEmail(string email, string callbackUrl);
}

namespace EHRsystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender; // Keeping this for other email functionalities like password reset

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext,
            ILogger<AccountController> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _logger = logger;
            _emailSender = emailSender;
        }

        // New action: User chooses Doctor or Patient role
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChooseRole()
        {
            return View();
        }


        #region Login/Logout

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null, string? role = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["RoleContext"] = role; // Pass role context to the view if needed for display
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            // The 'role' parameter from the URL is not typically used in the POST Login logic,
            // as role-based redirection happens after successful authentication via RedirectBasedOnRole.
            // If you need to enforce a specific role login, you'd add logic here.

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    await LogFailedLoginAttempt(model.Email, "User not found");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact administrator.");
                    await LogFailedLoginAttempt(model.Email, "Account deactivated");
                    return View(model);
                }

                if (user.AccountLockedUntil.HasValue && user.AccountLockedUntil > DateTime.UtcNow)
                {
                    ModelState.AddModelError(string.Empty,
                        $"Account is temporarily locked until {user.AccountLockedUntil.Value.ToLocalTime():yyyy-MM-dd HH:mm}. Please try again later.");
                    await LogFailedLoginAttempt(model.Email, "Account locked by custom logic");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    user.LastLogin = DateTime.UtcNow;
                    user.FailedLoginAttempts = 0;
                    user.AccountLockedUntil = null;
                    await _userManager.UpdateAsync(user);

                    await LogUserActivity(user.Id, "Login", "User logged in successfully");
                    _logger.LogInformation("User {Email} logged in successfully", model.Email);

                    if (user.RequiresPasswordChange)
                    {
                        TempData["InfoMessage"] = "You are required to change your password.";
                        return RedirectToAction("ChangePassword", "Manage"); // Assuming you have this action in a ManageController
                    }

                    var userRoles = await _userManager.GetRolesAsync(user);
                    return RedirectBasedOnRole(userRoles, returnUrl);
                }

                if (result.IsLockedOut)
                {
                    await LogFailedLoginAttempt(model.Email, "Account locked out by Identity system");
                    _logger.LogWarning("User {Email} account locked out", model.Email);
                    return RedirectToAction("Lockout");
                }
                else if (result.RequiresTwoFactor)
                {
                    _logger.LogWarning("User {Email} requires two-factor authentication", model.Email);
                    ModelState.AddModelError(string.Empty, "Two-factor authentication is required.");
                    return View(model);
                }
                else if (result.IsNotAllowed)
                {
                    var reason = "";
                    if (!user.EmailConfirmed) reason = "Email not confirmed.";
                    ModelState.AddModelError(string.Empty, $"Login not allowed. {reason}");
                    await LogFailedLoginAttempt(model.Email, $"Login not allowed: {reason}");
                    return View(model);
                }
                else
                {
                    if (user != null)
                    {
                        user.FailedLoginAttempts++;
                        if (user.FailedLoginAttempts >= 5)
                        {
                            user.AccountLockedUntil = DateTime.UtcNow.AddMinutes(15);
                            _logger.LogWarning("User {Email} account custom-locked for 15 minutes due to multiple failed attempts.", user.Email);
                        }
                        await _userManager.UpdateAsync(user);
                    }

                    await LogFailedLoginAttempt(model.Email, "Invalid password");
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = _userManager.GetUserId(User);
            var userEmail = User.Identity?.Name;

            await _signInManager.SignOutAsync();

            if (!string.IsNullOrEmpty(userId))
            {
                await LogUserActivity(userId, "Logout", $"User '{userEmail}' logged out");
                _logger.LogInformation("User {UserId} logged out", userId);
            }

            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        #endregion

        #region Registration

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? role = null) // Added optional role parameter
        {
            var model = new RegisterViewModel();
            if (!string.IsNullOrEmpty(role))
            {
                ViewData["PreselectedRole"] = role; // Pass to view for display or hidden field
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null, string? preselectedRole = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["PreselectedRole"] = preselectedRole; // Pass back to view on validation failure

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    CreatedDate = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    IsActive = true,
                    EmailConfirmed = true // FIX: Automatically confirm email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    string assignedRole = "User"; // Default role
                    if (!string.IsNullOrEmpty(preselectedRole) && (preselectedRole == "Doctor" || preselectedRole == "Patient"))
                    {
                        assignedRole = preselectedRole;
                    }
                    else if (!await _roleManager.RoleExistsAsync("User"))
                    {
                         await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    if (!await _roleManager.RoleExistsAsync(assignedRole))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(assignedRole));
                    }
                    await _userManager.AddToRoleAsync(user, assignedRole);

                    _logger.LogInformation("User created a new account with password and role {Role}.", assignedRole);
                    await LogUserActivity(user.Id, "Registration", $"User registered successfully as {assignedRole}");

                    // FIX: Removed email confirmation token generation and sending logic
                    TempData["SuccessMessage"] = "Account created successfully! Please log in."; // FIX: Notification message
                    return RedirectToAction("Login"); // FIX: Redirect directly to Login page
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        #endregion

        #region Password Management

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                TempData["InfoMessage"] = "If an account with that email exists, password reset instructions have been sent.";

                if (user != null && user.IsActive && user.EmailConfirmed)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ResetPassword", "Account",
                        new { email = user.Email, token }, protocol: HttpContext.Request.Scheme);

                    try
                    {
                        if (!string.IsNullOrEmpty(callbackUrl))
                        {
                            await _emailSender.SendPasswordResetEmail(user.Email!, callbackUrl);
                            await LogUserActivity(user.Id, "PasswordResetRequested", "Password reset requested");
                            _logger.LogInformation("Password reset email sent to {Email}", user.Email);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send password reset email to {Email}", model.Email);
                    }
                }
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? token = null, string? email = null)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Reset password attempted with missing token or email");
                TempData["ErrorMessage"] = "Invalid password reset token or email.";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel { Code = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                user.RequiresPasswordChange = false;
                await _userManager.UpdateAsync(user);

                await LogUserActivity(user.Id, "PasswordReset", "Password reset successfully");
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Email Confirmation (Removed from Register, but keeping these actions if still linked elsewhere or for future use)

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
        {
            // This action might no longer be strictly needed for new registrations if EmailConfirmed is true by default
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid email confirmation link.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found for confirmation.";
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await LogUserActivity(user.Id, "EmailConfirmed", "Email confirmed successfully");
                TempData["SuccessMessage"] = "Email confirmed successfully! You can now log in.";
                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = "Error confirming your email. The link may have expired or is invalid.";
            return RedirectToAction("Register");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterConfirmation()
        {
            // This view might become redundant if redirecting directly to Login
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmation(string? email)
        {
            // This action might become redundant if EmailConfirmed is true by default for new users
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email address is required to resend confirmation.";
                return RedirectToAction("Register");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found with that email.";
                return RedirectToAction("Register");
            }

            if (user.EmailConfirmed)
            {
                TempData["InfoMessage"] = "Your email is already confirmed.";
                return RedirectToAction("Login");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(
                "ConfirmEmail", "Account",
                new { userId = user.Id, token },
                protocol: HttpContext.Request.Scheme);

            try
            {
                if (!string.IsNullOrEmpty(callbackUrl))
                {
                    await _emailSender.SendConfirmationEmail(user.Email!, callbackUrl);
                    TempData["SuccessMessage"] = "Confirmation email resent successfully!";
                    TempData["Email"] = email;
                }
                else
                {
                    _logger.LogError("Failed to generate callback URL for email confirmation for {Email}", email);
                    TempData["ErrorMessage"] = "Account created, but failed to generate confirmation link. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email to {Email}", email);
                TempData["ErrorMessage"] = "Failed to resend confirmation email. Please try again later.";
            }

            return RedirectToAction("RegisterConfirmation");
        }

        #endregion

        #region Other Actions / Helper Methods

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectBasedOnRole(IList<string> userRoles, string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Prioritize roles for redirection
            if (userRoles.Contains("Admin"))
            {
                return RedirectToAction("ManageUsers", "Admin");
            }
            else if (userRoles.Contains("Doctor"))
            {
                return RedirectToAction("DoctorDashboard", "Dashboard"); // Assuming a DashboardController with DoctorDashboard
            }
            else if (userRoles.Contains("Patient"))
            {
                return RedirectToAction("PatientDashboard", "Dashboard"); // Assuming a DashboardController with PatientDashboard
            }

            return RedirectToAction("Index", "Home"); // Fallback
        }

        private async Task LogUserActivity(string? userId, string activityType, string details)
        {
            var auditLog = new UserAuditLog
            {
                UserId = userId ?? "N/A",
                Timestamp = DateTime.UtcNow,
                ActivityType = activityType,
                Details = details,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            _dbContext.UserAuditLogs.Add(auditLog);
            await _dbContext.SaveChangesAsync();
        }

        private async Task LogFailedLoginAttempt(string email, string details)
        {
            var auditLog = new UserAuditLog
            {
                UserId = "N/A",
                Timestamp = DateTime.UtcNow,
                ActivityType = "Login Failed",
                Details = $"Attempted login for: {email}. Reason: {details}",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            _dbContext.UserAuditLogs.Add(auditLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}
#endregion
