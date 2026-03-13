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

        public async Task SendOtpEmailAsync(string to, string otp, string? merchantName = null)
        {
            // TODO: Fetch template from DB by (merchantName, "OTP"). 
            // If not found, fallback to "Default".
            
            _logger.LogInformation($"[EMAIL_SERVICE] Sending OTP email for Merchant: {merchantName ?? "Default"}");

            string otpPadded = otp.PadRight(6, '0');
            string merchantText = string.IsNullOrEmpty(merchantName) ? "ZAP.vn" : merchantName;

            string body = $@"
<div style='background-color: #f8fafc; padding: 40px 20px; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; border: 1px solid #edf2f7;'>
        <div style='padding: 40px 40px 30px;'>
            <div style='text-align: center; margin-bottom: 30px;'>
                <h1 style='margin: 0; font-size: 32px; font-weight: 900; letter-spacing: -1px; color: #000000;'>ZAP</h1>
            </div>
            
            <h2 style='color: #1a202c; text-align: center; font-size: 22px; font-weight: bold; margin: 0 0 24px;'>Xác thực tài khoản của bạn</h2>
            
            <p style='color: #4a5568; font-size: 15px; line-height: 1.6; margin: 0 0 32px; text-align: left;'>
                Chào bạn, cảm ơn bạn đã đăng ký tài khoản tại <strong>{merchantText}</strong>. Vui lòng sử dụng mã xác thực gồm 6 số bên dưới để hoàn tất quá trình đăng ký của bạn.
            </p>

            <table cellpadding='0' cellspacing='0' border='0' width='100%' style='margin-bottom: 40px;'>
                <tr>
                    <td align='center'>
                        <table cellpadding='0' cellspacing='8' border='0'>
                            <tr>
                                <td width='46' height='56' align='center' valign='middle' style='background-color: #f8fafc; border-radius: 8px; border-bottom: 3px solid #2563eb; font-size: 28px; font-weight: bold; color: #0f172a;'>{otpPadded[0]}</td>
                                <td width='46' height='56' align='center' valign='middle' style='background-color: #f8fafc; border-radius: 8px; border-bottom: 3px solid #2563eb; font-size: 28px; font-weight: bold; color: #0f172a;'>{otpPadded[1]}</td>
                                <td width='46' height='56' align='center' valign='middle' style='background-color: #f8fafc; border-radius: 8px; border-bottom: 3px solid #2563eb; font-size: 28px; font-weight: bold; color: #0f172a;'>{otpPadded[2]}</td>
                                <td width='20' align='center' valign='middle' style='font-size: 24px; font-weight: bold; color: #cbd5e0;'>-</td>
                                <td width='46' height='56' align='center' valign='middle' style='background-color: #f8fafc; border-radius: 8px; border-bottom: 3px solid #2563eb; font-size: 28px; font-weight: bold; color: #0f172a;'>{otpPadded[3]}</td>
                                <td width='46' height='56' align='center' valign='middle' style='background-color: #f8fafc; border-radius: 8px; border-bottom: 3px solid #2563eb; font-size: 28px; font-weight: bold; color: #0f172a;'>{otpPadded[4]}</td>
                                <td width='46' height='56' align='center' valign='middle' style='background-color: #f8fafc; border-radius: 8px; border-bottom: 3px solid #2563eb; font-size: 28px; font-weight: bold; color: #0f172a;'>{otpPadded[5]}</td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <table cellpadding='0' cellspacing='0' border='0' width='100%' style='background-color: #f8fafc; border-radius: 12px;'>
                <tr>
                    <td valign='top' style='padding: 24px 16px 24px 24px; width: 24px;'>
                        <div style='width: 22px; height: 22px; border: 1.5px solid #3b82f6; border-radius: 50%; text-align: center; line-height: 22px; font-size: 14px; color: #3b82f6; font-weight: bold; font-family: Georgia, serif; font-style: italic;'>i</div>
                    </td>
                    <td valign='top' style='padding: 24px 24px 24px 0;'>
                        <h4 style='margin: 0 0 8px 0; color: #1e293b; font-size: 15px; font-weight: 600;'>Lưu ý bảo mật</h4>
                        <p style='margin: 0; color: #475569; font-size: 14px; line-height: 1.6;'>
                            Mã xác thực này sẽ hết hạn sau 120 giây. Nếu bạn không yêu cầu đăng ký tài khoản này, bạn có thể an tâm bỏ qua email này. Tài khoản của bạn sẽ không được kích hoạt nếu chưa xác thực.
                        </p>
                    </td>
                </tr>
            </table>
        </div>
        
        <div style='background-color: #f8fafc; padding: 30px 40px; text-align: center; border-top: 1px solid #edf2f7;'>
            <div style='margin-bottom: 24px; color: #94a3b8; font-size: 20px;'>
                <span style='margin: 0 10px;'>&#127760;</span>
                <span style='margin: 0 10px;'>&#128279;</span>
                <span style='margin: 0 10px;'>&#9993;</span>
            </div>
            <div style='margin-bottom: 24px;'>
                <a href='#' style='color: #64748b; font-size: 13px; font-weight: 600; margin: 0 10px; text-decoration: none;'>Trung tâm trợ giúp</a>
                <a href='#' style='color: #64748b; font-size: 13px; font-weight: 600; margin: 0 10px; text-decoration: none;'>Điều khoản sử dụng</a>
                <a href='#' style='color: #64748b; font-size: 13px; font-weight: 600; margin: 0 10px; text-decoration: none;'>Quyền riêng tư</a>
            </div>
            <div style='color: #94a3b8; font-size: 12px; line-height: 1.6;'>
                © {DateTime.Now.Year} {merchantText}. Tất cả các quyền được bảo hộ.<br>
                Đây là email tự động, vui lòng không trả lời email này.
            </div>
        </div>
    </div>
</div>";

            await SendEmailAsync(to, $"Xác thực tài khoản của bạn", body);
        }

        public async Task SendResetOtpEmailAsync(string to, string otp, string? merchantName = null)
        {
            _logger.LogInformation($"[EMAIL_SERVICE] Sending Reset OTP email for Merchant: {merchantName ?? "Default"}");

            string otpPadded = otp.PadRight(6, '0');
            string merchantText = "ZAP"; // Hardcoded as per image brand requirement or use merchantName if dynamic branding is needed

            string body = $@"
<div style='background-color: #f4f7f9; padding: 40px 20px; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif;'>
    <div style='max-width: 520px; margin: 0 auto; background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -1px rgba(0, 0, 0, 0.03); border: 1px solid #eef2f6;'>
        <div style='padding: 48px 40px;'>
            <div style='text-align: center; margin-bottom: 32px;'>
                <h1 style='margin: 0; font-size: 36px; font-weight: 900; letter-spacing: -1.5px; color: #000000; font-family: Arial, sans-serif;'>ZAP</h1>
            </div>
            
            <h2 style='color: #111827; text-align: center; font-size: 26px; font-weight: 800; margin: 0 0 16px; letter-spacing: -0.5px;'>Xác thực yêu cầu đặt lại mật khẩu</h2>
            
            <p style='color: #4b5563; font-size: 16px; line-height: 1.6; margin: 0 0 32px; text-align: center;'>
                Chào bạn, chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản <strong>{merchantText}</strong> của bạn. Vui lòng nhập mã xác thực gồm 6 số bên dưới để tiếp tục.
            </p>

            <table cellpadding='0' cellspacing='0' border='0' width='100%' style='margin-bottom: 40px;'>
                <tr>
                    <td align='center'>
                        <table cellpadding='0' cellspacing='0' border='0'>
                            <tr>
                                <td align='center' valign='middle' style='padding: 0 4px;'>
                                    <div style='width: 54px; height: 68px; line-height: 68px; background-color: #f8fafc; border-radius: 10px; border-bottom: 4px solid #2563eb; font-size: 32px; font-weight: 800; color: #1e293b; box-shadow: 0 1px 2px rgba(0,0,0,0.05);'>{otpPadded[0]}</div>
                                </td>
                                <td align='center' valign='middle' style='padding: 0 4px;'>
                                    <div style='width: 54px; height: 68px; line-height: 68px; background-color: #f8fafc; border-radius: 10px; border-bottom: 4px solid #2563eb; font-size: 32px; font-weight: 800; color: #1e293b; box-shadow: 0 1px 2px rgba(0,0,0,0.05);'>{otpPadded[1]}</div>
                                </td>
                                <td align='center' valign='middle' style='padding: 0 4px;'>
                                    <div style='width: 54px; height: 68px; line-height: 68px; background-color: #f8fafc; border-radius: 10px; border-bottom: 4px solid #2563eb; font-size: 32px; font-weight: 800; color: #1e293b; box-shadow: 0 1px 2px rgba(0,0,0,0.05);'>{otpPadded[2]}</div>
                                </td>
                                <td width='24' align='center' valign='middle' style='font-size: 20px; font-weight: bold; color: #cbd5e1;'>-</td>
                                <td align='center' valign='middle' style='padding: 0 4px;'>
                                    <div style='width: 54px; height: 68px; line-height: 68px; background-color: #f8fafc; border-radius: 10px; border-bottom: 4px solid #2563eb; font-size: 32px; font-weight: 800; color: #1e293b; box-shadow: 0 1px 2px rgba(0,0,0,0.05);'>{otpPadded[3]}</div>
                                </td>
                                <td align='center' valign='middle' style='padding: 0 4px;'>
                                    <div style='width: 54px; height: 68px; line-height: 68px; background-color: #f8fafc; border-radius: 10px; border-bottom: 4px solid #2563eb; font-size: 32px; font-weight: 800; color: #1e293b; box-shadow: 0 1px 2px rgba(0,0,0,0.05);'>{otpPadded[4]}</div>
                                </td>
                                <td align='center' valign='middle' style='padding: 0 4px;'>
                                    <div style='width: 54px; height: 68px; line-height: 68px; background-color: #f8fafc; border-radius: 10px; border-bottom: 4px solid #2563eb; font-size: 32px; font-weight: 800; color: #1e293b; box-shadow: 0 1px 2px rgba(0,0,0,0.05);'>{otpPadded[5]}</div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <div style='background-color: #f8fafc; border-radius: 14px; padding: 24px; border: 1px solid #f1f5f9;'>
                <table cellpadding='0' cellspacing='0' border='0' width='100%'>
                    <tr>
                        <td valign='top' style='width: 24px; padding-right: 16px;'>
                            <div style='width: 22px; height: 22px; background-color: #3b82f6; border-radius: 50%; color: #ffffff; text-align: center; line-height: 22px; font-size: 14px; font-weight: bold;'>i</div>
                        </td>
                        <td valign='top'>
                            <h4 style='margin: 0 0 6px 0; color: #1e293b; font-size: 16px; font-weight: 700;'>Lưu ý bảo mật</h4>
                            <p style='margin: 0; color: #64748b; font-size: 14px; line-height: 1.6;'>
                                Mã này sẽ hết hạn sau <strong>120 giây</strong>. Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này để đảm bảo an toàn cho tài khoản.
                            </p>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        
        <div style='background-color: #fcfdfe; padding: 40px; text-align: center; border-top: 1px solid #f1f5f9;'>
            <div style='margin-bottom: 24px;'>
                <span style='margin: 0 12px; color: #94a3b8; font-size: 20px;'>🌐</span>
                <span style='margin: 0 12px; color: #94a3b8; font-size: 20px;'>🔗</span>
                <span style='margin: 0 12px; color: #94a3b8; font-size: 20px;'>✉️</span>
            </div>
            <div style='margin-bottom: 24px;'>
                <a href='#' style='color: #475569; font-size: 14px; font-weight: 600; margin: 0 10px; text-decoration: none;'>Trung tâm trợ giúp</a>
                <a href='#' style='color: #475569; font-size: 14px; font-weight: 600; margin: 0 10px; text-decoration: none;'>Điều khoản sử dụng</a>
                <a href='#' style='color: #475569; font-size: 14px; font-weight: 600; margin: 0 10px; text-decoration: none;'>Quyền riêng tư</a>
            </div>
            <div style='color: #94a3b8; font-size: 12px; line-height: 1.8;'>
                © {DateTime.Now.Year} ZAP.vn. Tất cả các quyền được bảo hộ.<br>
                Đây là email tự động, vui lòng không trả lời email này.
            </div>
        </div>
    </div>
</div>";

            await SendEmailAsync(to, "Xác thực yêu cầu đặt lại mật khẩu", body);
        }

        public async Task SendResetLinkEmailAsync(string to, string link, string? merchantName = null)
        {
             // TODO: Fetch template from DB by (merchantName, "ResetPassword").
            
            _logger.LogInformation($"[EMAIL_SERVICE] Sending Reset Link for Merchant: {merchantName ?? "Default"}");

            string body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
                    <h2 style='color: #007bff; text-align: center;'>{(string.IsNullOrEmpty(merchantName) ? "Xác thực tài khoản CRM" : $"Xác thực tài khoản {merchantName}")}</h2>
                    <p>Xin chào,</p>
                    <p>Bạn vừa yêu cầu lấy lại mật khẩu. Vui lòng nhấn vào nút bên dưới để đặt lại mật khẩu mới:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{link}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>Đặt lại mật khẩu</a>
                    </div>
                    <p style='color: #666;'>Hoặc copy link này vào trình duyệt: <br> <a href='{link}' style='color: #007bff; word-break: break-all;'>{link}</a></p>
                    <p style='color: #666; margin-top: 20px;'>Link này có hiệu lực trong <b>15 phút</b>.</p>
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='font-size: 12px; color: #999; text-align: center;'>Đây là tin nhắn tự động từ {(merchantName ?? "hệ thống CRM")}, vui lòng không trả lời.</p>
                </div>";

            await SendEmailAsync(to, "Link đặt lại mật khẩu của bạn", body);
        }
    }
}
