using System;
using System.Threading.Tasks;
using CRM.Authentication.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CRM.Authentication.Infrastructure.Security
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // GIẢ LẬP GỬI MAIL: Thực tế bạn sẽ dùng SmtpClient hoặc SendGrid/Mailgun ở đây
            _logger.LogInformation($"[EMAIL_SERVICE] Sending email to {to}");
            _logger.LogInformation($"[EMAIL_SERVICE] Subject: {subject}");
            _logger.LogInformation($"[EMAIL_SERVICE] Body: {body}");
            
            // In ra console để người dùng nhìn thấy ngay
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"SENDING MAIL TO: {to}");
            Console.WriteLine($"SUBJECT: {subject}");
            Console.WriteLine($"BODY: {body}");
            Console.WriteLine("--------------------------------------------------");

            await Task.CompletedTask;
        }

        public async Task SendOtpEmailAsync(string to, string otp)
        {
            await SendEmailAsync(to, "Mã xác thực OTP của bạn", $"Mã OTP của bạn là: {otp}. Mã này có hiệu lực trong 5 phút.");
        }
    }
}
