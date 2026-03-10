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
 
        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordResetRepository resetRepository,
            IEmailService emailService,
            IStringLocalizer<SharedResource> localizer,
            IOptions<MailSettings> mailSettings)
        {
            _userRepository = userRepository;
            _resetRepository = resetRepository;
            _emailService = emailService;
            _localizer = localizer;
            _mailSettings = mailSettings.Value;
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

            // 3. Generate Reset Token
            string resetToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
 
            // 4. Construct Reset Link
            string resetLink = $"{_mailSettings.FrontendResetPasswordUrl}?token={resetToken}";
             
            // Gửi mail LINK cho người dùng
            await _emailService.SendResetLinkEmailAsync(email, resetLink); 
            Console.WriteLine($"[EMAIL_SENT] Reset Link for {email}: {resetLink}"); 
 
            // 5. Save Request
            var resetRequest = new PasswordResetRequest
            {
                UserGuid = $"Customer/{user._key}",
                Email = email,
                Method = "email",
                ResetToken = resetToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15) // Tăng lên 15 phút cho link
            };

            await _resetRepository.CreateAsync(resetRequest);

            return new ForgotPasswordResponseDto
            {
                Success = true,
                Message = "Link đặt lại mật khẩu đã được gửi qua email của bạn",
                ResetToken = resetToken,
                ExpiresIn = 900 // 15 mins
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
