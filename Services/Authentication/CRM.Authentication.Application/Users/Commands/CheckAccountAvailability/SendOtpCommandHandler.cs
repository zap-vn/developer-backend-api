using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Application.Common.Interfaces;

namespace CRM.Authentication.Application.Users.Commands.CheckAccountAvailability
{
    public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IPhoneService _phoneService;
        private readonly IOtpRepository _otpRepository;

        public SendOtpCommandHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            IPhoneService phoneService,
            IOtpRepository otpRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _phoneService = phoneService;
            _otpRepository = otpRepository;
        }

        public async Task<bool> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Account))
            {
                throw new Exception("error_missing_account|Vui lòng nhập Email hoặc Số điện thoại.");
            }

            string identifier = request.Account.Trim();
            bool isEmail = identifier.Contains("@");

            // Normalize phone number if dialing_code is present
            if (!isEmail && !string.IsNullOrEmpty(request.DialingCode))
            {
                if (identifier.StartsWith("0")) identifier = identifier.Substring(1);
                identifier = request.DialingCode + identifier;
            }

            // Generate 6-digit OTP
            var otpCode = new Random().Next(100000, 999999).ToString();

            // Save OTP to database
            var customerOtp = new CustomerOtp
            {
                email = isEmail ? identifier : string.Empty,
                phone = !isEmail ? identifier : string.Empty,
                otp_code = otpCode,
                purpose = "login",
                expired_at = DateTime.UtcNow.AddMinutes(2),
                created_at = DateTime.UtcNow
            };
            await _otpRepository.CreateAsync(customerOtp);

            // Send OTP
            try
            {
                if (isEmail)
                {
                    await _emailService.SendOtpEmailAsync(identifier, otpCode, "Guest");
                }
                else
                {
                    await _phoneService.SendSmsOtpAsync(identifier, otpCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to send OTP: {ex.Message}");
                throw new Exception("error_sending_otp|Không thể gửi mã xác thực. Vui lòng thử lại sau.");
            }

            return true;
        }
    }
}
