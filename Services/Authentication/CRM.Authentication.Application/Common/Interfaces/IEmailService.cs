using System.Threading.Tasks;

namespace CRM.Authentication.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendOtpEmailAsync(string to, string otp, string? merchantName = null);
        Task SendResetOtpEmailAsync(string to, string otp, string? merchantName = null);
        Task SendResetLinkEmailAsync(string to, string link, string? merchantName = null);
    }
}
