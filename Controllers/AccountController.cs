using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using EHRsystem.Models.Entities;
using EHRsystem.Data;
using System.Security.Claims;
using EHRsystem.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using EHRsystem.Services; 
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EHRsystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            ILogger<AccountController> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        #region Login/Logout

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    await LogFailedLoginAttempt(model.Email, "User not found");
                    return View(model);
                }

                // Check if account is active
                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact administrator.");
                    await LogFailedLoginAttempt(model.Email, "Account deactivated");
                    return View(model);
                }

                // Check for account lockout
                if (user.AccountLockedUntil.HasValue && user.AccountLockedUntil > DateTime.UtcNow)
                {
                    ModelState.AddModelError(string.Empty,
                        $"Account is temporarily locked until {user.AccountLockedUntil.Value.ToLocalTime():yyyy-MM-dd HH:mm}. Please try again later.");
                    await LogFailedLoginAttempt(model.Email, "Account locked");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Update user login tracking
                    user.LastLogin = DateTime.UtcNow;
                    user.FailedLoginAttempts = 0;
                    user.AccountLockedUntil = null;
                    await _userManager.UpdateAsync(user);

                    // Log successful login
                    await LogUserActivity(user.Id, "Login", "User logged in successfully");
                    _logger.LogInformation("User {Email} logged in successfully", model.Email);

                    // Check if password change is required
                    if (user.RequiresPasswordChange)
                    {
                        TempData["InfoMessage"] = "You are required to change your password.";
                        return RedirectToAction("ChangePassword");
                    }

                    // Redirect based on user role
                    var userRoles = await _userManager.GetRolesAsync(user);
                    return RedirectBasedOnRole(userRoles, returnUrl);
                }

                if (result.IsLockedOut)
                {
                    // Handle lockout
                    await LogFailedLoginAttempt(model.Email, "Account locked out by system");
                    _logger.LogWarning("User {Email} account locked out", model.Email);
                    return RedirectToAction("Lockout");
                }
                else
                {
                    // Record failed login attempt
                    if (user != null)
                    {
                        user.FailedLoginAttempts++;
                        if (user.FailedLoginAttempts >= 5)
                        {
                            user.AccountLockedUntil = DateTime.UtcNow.AddMinutes(15);
                        }
                        await _userManager.UpdateAsync(user);
                    }

                    await LogFailedLoginAttempt(model.Email, "Invalid password");
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }
            }

            // If ModelState is not valid, return the view with validation errors
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = _userManager.GetUserId(User);
            await _signInManager.SignOutAsync();

            if (!string.IsNullOrEmpty(userId))
            {
                await LogUserActivity(userId, "Logout", "User logged out");
                _logger.LogInformation("User {UserId} logged out", userId);
            }

            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        #endregion

        #region Password Management

        // You will need to add/paste your ChangePassword action here.
        // If you have one, copy it from AccountController.old and paste here.

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
                        else
                        {
                            _logger.LogError("Failed to generate callback URL for password reset");
                            TempData["ErrorMessage"] = "Failed to generate reset link. Please try again later.";
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
                        TempData["ErrorMessage"] = "Failed to send reset email. Please try again later.";
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
                TempData["ErrorMessage"] = "Invalid password reset token.";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel { Code = token, Email = email }; // Mapped token to Code
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

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password); // Used model.Code
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

        #region Helper Methods
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await LogUserActivity(user.Id, "EmailConfirmed", "Email confirmed successfully");
                TempData["SuccessMessage"] = "Email confirmed successfully! You can now log in.";
                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = "Error confirming your email. The link may have expired.";
            return RedirectToAction("Register"); // Assuming you have a Register action
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmation(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email address is required";
                return RedirectToAction("Register");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("Register");
            }

            if (user.EmailConfirmed)
            {
                TempData["InfoMessage"] = "Your email is already confirmed";
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
                    TempData["Email"] = email; // For potential re-resend
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to generate confirmation link. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resend confirmation email to {Email}", user.Email);
                TempData["ErrorMessage"] = "Failed to resend confirmation email. Please try again later.";
            }

            return RedirectToAction("RegisterConfirmation");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void PopulateRolesList(RegisterViewModel model)
        {
            var roles = _roleManager.Roles
                .Where(r => r.Name != "SuperAdmin") // Prevent creating superadmins
                .Select(r => r.Name)
                .ToList();

            model.AvailableRoles = roles.Select(r => new SelectListItem { Value = r, Text = r }).ToList(); // Convert to SelectListItem
        }

        private IActionResult RedirectBasedOnRole(IList<string> userRoles, string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Redirect based on primary role (order matters here if a user has multiple roles)
            if (userRoles.Contains("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            if (userRoles.Contains("Doctor"))
            {
                return RedirectToAction("Dashboard", "Doctor");
            }
            if (userRoles.Contains("Nurse"))
            {
                return RedirectToAction("Dashboard", "Nurse");
            }
            if (userRoles.Contains("Patient"))
            {
                return RedirectToAction("Dashboard", "Patient");
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task LogUserActivity(string userId, string action, string details)
        {
            try
            {
                var auditLog = new UserAuditLog
                {
                    UserId = userId,
                    Action = action,
                    Details = details,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    Timestamp = DateTime.UtcNow
                };

                _context.UserAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log user activity for user {UserId}", userId);
            }
        }

        private async Task LogFailedLoginAttempt(string email, string reason)
        {
            try
            {
                var auditLog = new UserAuditLog
                {
                    UserId = email, // Use email as identifier for failed attempts
                    Action = "FailedLogin",
                    Details = $"Failed login attempt: {reason}",
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    Timestamp = DateTime.UtcNow
                };

                _context.UserAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log failed login attempt for {Email}", email);
            }
        }

        // Example modification for SendConfirmationEmail (find your actual method and adjust)
        private async Task<bool> SendEmailConfirmation(ApplicationUser user, string callbackUrl)
        {
            // Fix CS8604: Ensure user.Email is not null before sending.
            if (user.Email != null) 
            {
                await _emailSender.SendConfirmationEmail(user.Email, callbackUrl);
                return true;
            }
            _logger.LogWarning("Attempted to send confirmation email to null email for user {UserId}", user.Id);
            return false;
        }

        #endregion
    }
}