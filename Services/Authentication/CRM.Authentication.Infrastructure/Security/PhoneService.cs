using CRM.Authentication.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Security
{
    public class PhoneService : IPhoneService
    {
        private readonly ILogger<PhoneService> _logger;

        public PhoneService(ILogger<PhoneService> logger)
        {
            _logger = logger;
        }

        public async Task SendSmsOtpAsync(string phone, string otp)
        {
            _logger.LogInformation($"[SMS_OTP] Sending to {phone}: {otp}");
            // Real SMS gateway API call would go here
            await Task.CompletedTask;
        }

        public async Task SendZaloOtpAsync(string phone, string otp)
        {
            _logger.LogInformation($"[ZALO_OTP] Sending to {phone}: {otp}");
            // Real Zalo OA API call would go here
            await Task.CompletedTask;
        }
    }
}
