using MediatR;
using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Application.Users.DTOs;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Domain.Entities;
using Microsoft.Extensions.Localization;
using CRM.BuildingBlocks.Localization;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Authentication.Application.Users.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResponseDto>;

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetRepository _resetRepository;
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordResetRepository resetRepository,
            IEmailService emailService,
            IStringLocalizer<SharedResource> localizer)
        {
            _userRepository = userRepository;
            _resetRepository = resetRepository;
            _emailService = emailService;
            _localizer = localizer;
        }

        public async Task<ForgotPasswordResponseDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Find User
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                // We throw 404 as per skill requirement or security preference
                throw new KeyNotFoundException("USER_NOT_FOUND");
            }

            // 2. Check Rate Limit
            int recentRequests = await _resetRepository.GetRecentRequestCountAsync(request.Email, DateTime.UtcNow.AddHours(-1));
            if (recentRequests >= 3)
            {
                throw new Exception("TOO_MANY_REQUESTS");
            }

            // 3. Generate OTP (6 digits)
            string otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            
            // Gửi mail OTP cho người dùng
            await _emailService.SendOtpEmailAsync(request.Email, otp); 
            Console.WriteLine($"[EMAIL_SENT] OTP for {request.Email}: {otp}"); 

            // 4. Generate Reset Token
            string resetToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

            // 5. Save Request
            var resetRequest = new PasswordResetRequest
            {
                UserGuid = $"Customer/{user.CustomerId}",
                Method = "email",
                OtpHash = HashString(otp),
                ResetToken = resetToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            await _resetRepository.CreateAsync(resetRequest);

            return new ForgotPasswordResponseDto
            {
                Success = true,
                Message = _localizer["auth_otp_sent"] ?? "OTP đã được gửi",
                ResetToken = resetToken,
                ExpiresIn = 300
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
