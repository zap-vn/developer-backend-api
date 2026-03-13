using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Application.Common.Interfaces;

namespace CRM.Authentication.Application.Users.Commands.CheckAccountAvailability
{
    public class CheckAccountAvailabilityCommandHandler : IRequestHandler<CheckAccountAvailabilityCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IOtpRepository _otpRepository;

        public CheckAccountAvailabilityCommandHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            IOtpRepository otpRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _otpRepository = otpRepository;
        }

        public async Task<bool> Handle(CheckAccountAvailabilityCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new Exception("error_missing_email|Vui lòng nhập Email hoặc Số điện thoại.");
            }

            string identifier = request.Email.Trim();
            bool isEmail = identifier.Contains("@");

            // 1. Check if exists as Email or Phone
            if (await _userRepository.EmailExistsAsync(identifier))
            {
                throw new Exception("error_duplicate_account|Email này đã được sử dụng. Vui lòng chọn email khác.");
            }

            if (await _userRepository.PhoneExistsAsync(identifier))
            {
                throw new Exception("error_duplicate_account|Số điện thoại này đã được sử dụng. Vui lòng chọn số khác.");
            }

            // 2. Handle Logic based on Provider
            string provider = string.IsNullOrWhiteSpace(request.Provider) ? "Email" : request.Provider;
            bool isSocial = provider == "Google" || provider == "Facebook" || provider == "Apple";

            if (isSocial)
            {
                // For Social: Just confirm account is available, no OTP needed here
                // Registration usually happens immediately or after profile confirmation in frontend
                return true;
            }

            // 3. If standard Email/Phone -> Generate 6-digit OTP
            var otpCode = new Random().Next(100000, 999999).ToString();

            // 4. Save OTP to database (CustomerOtps)
            var customerOtp = new CustomerOtp
            {
                Email = isEmail ? identifier : string.Empty,
                Phone = !isEmail ? identifier : string.Empty,
                OtpCode = otpCode,
                Purpose = "register",
                ExpiredAt = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow
            };
            await _otpRepository.CreateAsync(customerOtp);

            // 5. Send OTP
            try
            {
                if (isEmail)
                {
                    await _emailService.SendOtpEmailAsync(identifier, otpCode, "Guest");
                }
                else
                {
                    // Logic to send SMS if we have a service, for now just log
                    Console.WriteLine($"[SMS OTP] Send to {identifier}: {otpCode}");
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
