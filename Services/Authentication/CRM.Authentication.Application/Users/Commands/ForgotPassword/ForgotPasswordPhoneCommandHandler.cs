using MediatR;
using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Application.Users.DTOs;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Domain.Entities;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;
using CRM.BuildingBlocks.Exceptions;

namespace CRM.Authentication.Application.Users.Commands.ForgotPassword
{
    public record ForgotPasswordPhoneCommand(string Phone, string Channel) : IRequest<ForgotPasswordResponseDto>;

    public class ForgotPasswordPhoneCommandHandler : IRequestHandler<ForgotPasswordPhoneCommand, ForgotPasswordResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetRepository _resetRepository;
        private readonly IPhoneService _phoneService;

        public ForgotPasswordPhoneCommandHandler(
            IUserRepository userRepository,
            IPasswordResetRepository resetRepository,
            IPhoneService phoneService)
        {
            _userRepository = userRepository;
            _resetRepository = resetRepository;
            _phoneService = phoneService;
        }

        public async Task<ForgotPasswordResponseDto> Handle(ForgotPasswordPhoneCommand request, CancellationToken cancellationToken)
        {
            var phone = request.Phone?.Trim() ?? string.Empty;
            
            // 1. Find User by Phone
            var user = await _userRepository.GetByPhoneAsync(phone);
            if (user == null)
            {
                throw new KeyNotFoundException("USER_NOT_FOUND");
            }

            // 2. Rate Limiting (Simple check)
            int recentRequests = await _resetRepository.GetRecentRequestCountAsync(phone, DateTime.UtcNow.AddHours(-1));
            if (recentRequests >= 3) // Thắt chặt hơn cho Phone: 3 lần/giờ
            {
                throw new TooManyRequestsException();
            }

            // 3. Generate OTP (6 digits) and Reset Token
            var otp = new Random().Next(100000, 999999).ToString();
            string resetToken = Guid.NewGuid().ToString("N");

            // 4. Send OTP via selected channel
            string customerGuid = $"Customer/{user._key}";
            if (request.Channel.ToLower() == "zalo")
            {
                await _phoneService.SendZaloOtpAsync(phone, otp);
            }
            else
            {
                await _phoneService.SendSmsOtpAsync(phone, otp, customerGuid);
            }

            // 5. Save Request with OtpHash
            var resetRequest = new PasswordResetRequest
            {
                UserGuid = $"Customer/{user._key}",
                Phone = phone,
                Method = "phone",
                Channel = request.Channel.ToLower(),
                OtpHash = HashString(otp),
                ResetToken = resetToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(120) // OTP phone hiệu lực 120 giây
            };

            await _resetRepository.CreateAsync(resetRequest);

            return new ForgotPasswordResponseDto
            {
                Success = true,
                Message = "Mã xác thực đã được gửi đến điện thoại của bạn",
                ResetToken = resetToken,
                ExpiresIn = 120 // 120 seconds
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
