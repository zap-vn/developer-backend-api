using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Application.Common.Models;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Security
{
    public class PhoneService : IPhoneService
    {
        private readonly ILogger<PhoneService> _logger;
        private readonly IVietGuyService _vietGuyService;
        private readonly IEmailSettingRepository _emailSettingRepository;

        public PhoneService(
            ILogger<PhoneService> logger,
            IVietGuyService vietGuyService,
            IEmailSettingRepository emailSettingRepository)
        {
            _logger = logger;
            _vietGuyService = vietGuyService;
            _emailSettingRepository = emailSettingRepository;
        }

        public async Task SendSmsOtpAsync(string phone, string otp, string? customerGuid = null)
        {
            _logger.LogInformation($"[PHONE] Sending SMS OTP to {phone} for Customer: {customerGuid ?? "default"}");

            // Fetch dynamic settings from DB
            var guidToUse = customerGuid ?? "Customer/1"; // Default as per image if none given
            var setting = await _emailSettingRepository.GetByCustomerGuidAsync(guidToUse);

            if (setting != null && !string.IsNullOrEmpty(setting.account_name))
            {
                _logger.LogInformation($"[PHONE] Using Vietguy Account: {setting.account_name} from Database.");
                
                // Use BodySMS template if available
                string messageTemplate = setting.body_sms ?? "Ma OTP cua ban la: {otp}";
                string message = messageTemplate.Replace("{otp}", otp).Replace("{otp.code}", otp);
                
                await _vietGuyService.SendSmsAsync(phone, message, setting);
            }
            else
            {
                _logger.LogWarning($"[PHONE] No SMS settings found for {guidToUse} in 'email_setting' collection.");
                // Fallback or error
            }
        }

        public async Task SendZaloOtpAsync(string phone, string otp)
        {
            // Implementation for Zalo if needed, currently focusing on Vietguy
            _logger.LogInformation($"[PHONE] Zalo not yet integrated with database-driven settings.");
            await Task.CompletedTask;
        }

        public async Task<bool> VerifyOtpAsync(string phone, string otp)
        {
            // Implement verification logic (e.g., check from IOtpRepository)
            await Task.CompletedTask;
            return true;
        }
    }
}
