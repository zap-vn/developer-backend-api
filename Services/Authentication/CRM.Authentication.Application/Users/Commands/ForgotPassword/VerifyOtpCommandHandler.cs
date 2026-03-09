using MediatR;
using CRM.Authentication.Application.Users.DTOs;
using CRM.Authentication.Domain.Interfaces;
using Microsoft.Extensions.Localization;
using CRM.BuildingBlocks.Localization;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Authentication.Application.Users.Commands.ForgotPassword
{
    public record VerifyOtpCommand(string ResetToken, string Otp) : IRequest<VerifyOtpResponseDto>;

    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, VerifyOtpResponseDto>
    {
        private readonly IPasswordResetRepository _resetRepository;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public VerifyOtpCommandHandler(
            IPasswordResetRepository resetRepository,
            IStringLocalizer<SharedResource> localizer)
        {
            _resetRepository = resetRepository;
            _localizer = localizer;
        }

        public async Task<VerifyOtpResponseDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var resetRequest = await _resetRepository.GetByResetTokenAsync(request.ResetToken);

            if (resetRequest == null || resetRequest.IsUsed)
            {
                throw new Exception("TOKEN_INVALID");
            }

            if (resetRequest.ExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("OTP_EXPIRED");
            }

            if (resetRequest.Attempts >= 3)
            {
                throw new Exception("OTP_MAX_ATTEMPTS");
            }

            string inputOtpHash = HashString(request.Otp);
            if (resetRequest.OtpHash != inputOtpHash)
            {
                resetRequest.Attempts++;
                await _resetRepository.UpdateAsync(resetRequest);
                throw new Exception("INVALID_OTP");
            }

            // OTP is correct - Generate Confirm Token
            string confirmToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            resetRequest.ConfirmToken = confirmToken;
            resetRequest.IsUsed = true; // Mark OTP as used to get the confirm token
            await _resetRepository.UpdateAsync(resetRequest);

            return new VerifyOtpResponseDto
            {
                Success = true,
                Message = _localizer["auth_otp_verified"] ?? "Xác thực thành công",
                ConfirmToken = confirmToken
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
