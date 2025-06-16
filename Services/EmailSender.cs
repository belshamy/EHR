// F:\EHRsystem\Services\EmailSender.cs - THIS FILE MUST CONTAIN ONLY THE CLASS IMPLEMENTATION
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace EHRsystem.Services
{
    // This EmailSettings class can be here or in a separate file (e.g., Models/Configuration)
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587; 
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty; 
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = "EHR System";
    }

    public class EmailSender : IEmailSender // THIS IS WHERE IT IMPLEMENTS THE INTERFACE
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendConfirmationEmail(string email, string callbackUrl)
        {
            _logger.LogInformation("Sending confirmation email to {Email} with link: {CallbackUrl}", email, callbackUrl);
            await ExecuteEmailSend(email, "Confirm your email - EHR System",
                $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>Confirm Account</a>.");
        }

        public async Task SendPasswordResetEmail(string email, string resetLink)
        {
            _logger.LogInformation("Sending password reset email to {Email} with link: {ResetLink}", email, resetLink);
            await ExecuteEmailSend(email, "Reset your password - EHR System",
                $"Please reset your password by clicking this link: <a href='{resetLink}'>Reset Password</a>.");
        }

        private async Task ExecuteEmailSend(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer) ||
                string.IsNullOrEmpty(_emailSettings.SmtpUsername) ||
                string.IsNullOrEmpty(_emailSettings.SmtpPassword) ||
                string.IsNullOrEmpty(_emailSettings.SenderEmail))
            {
                _logger.LogError("EmailSender is not configured. Missing SMTP server, username, password, or sender email in appsettings.json.");
                throw new InvalidOperationException("Email sender not configured properly. Check appsettings.json or User Secrets.");
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
                }
                catch (SmtpException ex)
                {
                    _logger.LogError(ex, "SMTP error when sending email to {ToEmail}. Error code: {ErrorCode}. Message: {ErrorMessage}", toEmail, ex.StatusCode, ex.Message);
                    throw; 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "General error when sending email to {ToEmail}. Message: {ErrorMessage}", toEmail, ex.Message);
                    throw;
                }
            }
        }
    }
}