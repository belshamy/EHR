// F:\EHRsystem\Services\IEmailSender.cs - THIS FILE MUST CONTAIN ONLY THIS INTERFACE
namespace EHRsystem.Services
{
    public interface IEmailSender
    {
        Task SendConfirmationEmail(string email, string callbackUrl);
        Task SendPasswordResetEmail(string email, string resetLink);
    }
}