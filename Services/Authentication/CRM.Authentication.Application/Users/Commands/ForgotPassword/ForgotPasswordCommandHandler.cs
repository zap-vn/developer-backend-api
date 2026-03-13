using MediatR;
using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Application.Users.DTOs;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Domain.Entities;
using Microsoft.Extensions.Localization;
using CRM.BuildingBlocks.Localization;
using System.Security.Cryptography;
using System.Text;

using CRM.BuildingBlocks.Exceptions;
using Microsoft.Extensions.Options;
using CRM.Authentication.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace CRM.Authentication.Application.Users.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ForgotPasswordResponseDto>
    {
        public string Email { get; set; } = string.Empty;

        public ForgotPasswordCommand() { }
        public ForgotPasswordCommand(string email) => Email = email;
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetRepository _resetRepository;
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly MailSettings _mailSettings;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;
 
        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordResetRepository resetRepository,
            IEmailService emailService,
            IStringLocalizer<SharedResource> localizer,
            IOptions<MailSettings> mailSettings,
            ILogger<ForgotPasswordCommandHandler> logger)
        {
            _userRepository = userRepository;
            _resetRepository = resetRepository;
            _emailService = emailService;
            _localizer = localizer;
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task<ForgotPasswordResponseDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email?.Trim() ?? string.Empty;
            // 1. Find User
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                // We throw 404 as per skill requirement or security preference
                throw new KeyNotFoundException("USER_NOT_FOUND");
            }

            // Allow social users to set a local password via the reset flow
            // This enables hybrid login (Social + Email/Password) 
            // if (string.IsNullOrEmpty(user.Password) && ... ) block removed to support this requirement.


            // 2. Check Rate Limit (Tạm thời tăng lên 10 để bạn test cho thoải mái)
            int recentRequests = await _resetRepository.GetRecentRequestCountAsync(email, DateTime.UtcNow.AddHours(-1));
            if (recentRequests >= 10)
            {
                throw new TooManyRequestsException();
            }

            // 3. Generate 6-digit OTP for Forgot Password
            var otp = new Random().Next(100000, 999999).ToString();
            string otpHash = HashString(otp);

            // 4. Generate Reset Token (acts as the ID for the session)
            string resetToken = Guid.NewGuid().ToString("N");
             
            // Gửi mail OTP cho người dùng với template mới
            await _emailService.SendResetOtpEmailAsync(email, otp, user.MerchantName); 
            _logger.LogInformation("[EMAIL_SENT] OTP Reset Password cho User {Email} là: {Otp}", email, otp); 
 
            // 5. Save Request
            var resetRequest = new PasswordResetRequest
            {
                UserGuid = $"Customer/{user._key}",
                Email = email,
                Method = "email",
                ResetToken = resetToken,
                OtpHash = otpHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(2) // OTP expires in 2 mins as per image
            };

            await _resetRepository.CreateAsync(resetRequest);

            return new ForgotPasswordResponseDto
            {
                Success = true,
                Message = "Mã xác thực đặt lại mật khẩu đã được gửi qua email của bạn",
                ResetToken = resetToken,
                ExpiresIn = 120 // 120 seconds (2 mins)
            };
        }

        private string HashString(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
