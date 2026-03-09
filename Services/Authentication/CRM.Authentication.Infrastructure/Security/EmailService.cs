using System;
using System.Threading.Tasks;
using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CRM.Authentication.Infrastructure.Security
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly MailSettings _mailSettings;

        public EmailService(ILogger<EmailService> logger, IOptions<MailSettings> mailSettings)
        {
            _logger = logger;
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(_mailSettings.Host))
                {
                    _logger.LogWarning("[EMAIL_SERVICE] Mail settings not configured. Logging instead.");
                    LogToConsole(to, subject, body);
                    return;
                }

                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Email);
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = body };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                
                // Allow insecure connections if not using SSL (for local testing/relay)
                var secureOptions = _mailSettings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
                if (_mailSettings.Port == 25 || _mailSettings.Port == 587)
                {
                    secureOptions = SecureSocketOptions.StartTls;
                }

                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, secureOptions);
                
                if (!string.IsNullOrEmpty(_mailSettings.Password))
                {
                    await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
                }

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"[EMAIL_SERVICE] Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[EMAIL_SERVICE] Failed to send email to {to}");
                // Fallback to console for debugging in dev
                LogToConsole(to, subject, body);
            }
        }

        private void LogToConsole(string to, string subject, string body)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"[FALLBACK] SENDING MAIL TO: {to}");
            Console.WriteLine($"SUBJECT: {subject}");
            Console.WriteLine($"BODY: {body}");
            Console.WriteLine("--------------------------------------------------");
        }

        public async Task SendOtpEmailAsync(string to, string otp)
        {
            string body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
                    <h2 style='color: #007bff; text-align: center;'>Xác thực tài khoản CRM</h2>
                    <p>Xin chào,</p>
                    <p>Bạn vừa yêu cầu lấy lại mật khẩu. Mã OTP của bạn là:</p>
                    <div style='background-color: #f8f9fa; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; color: #333; letter-spacing: 5px; border-radius: 5px;'>
                        {otp}
                    </div>
                    <p style='color: #666; margin-top: 20px;'>Mã này có hiệu lực trong <b>5 phút</b>. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='font-size: 12px; color: #999; text-align: center;'>Đây là tin nhắn tự động, vui lòng không trả lời.</p>
                </div>";

            await SendEmailAsync(to, "Mã xác thực OTP của bạn", body);
        }
    }
}
