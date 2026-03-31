using MediatR;
using CRM.Authentication.Application.Users.DTOs;
using CRM.Authentication.Domain.Interfaces;
using Microsoft.Extensions.Localization;
using CRM.BuildingBlocks.Localization;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Text;

using CRM.BuildingBlocks.Exceptions;

namespace CRM.Authentication.Application.Users.Commands.ForgotPassword
{
    public class VerifyOtpCommand : IRequest<VerifyOtpResponseDto>
    {
        [JsonPropertyName("account")]
        public string Account { get; set; } = string.Empty;

        [JsonPropertyName("otp")]
        public string Otp { get; set; } = string.Empty;
    }

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
            var resetRequest = await _resetRepository.GetLatestByIdentifierAsync(request.Account);

            if (resetRequest == null || resetRequest.is_used)
            {
                throw new ValidationException("TOKEN_INVALID");
            }

            if (resetRequest.expired_at < DateTime.UtcNow)
            {
                throw new ValidationException("OTP_EXPIRED");
            }

            if (resetRequest.attempts >= 3)
            {
                throw new ValidationException("OTP_MAX_ATTEMPTS");
            }

            string inputOtpHash = HashString(request.Otp?.Trim() ?? string.Empty);
            if (resetRequest.otp_hash != inputOtpHash)
            {
                resetRequest.attempts++;
                await _resetRepository.UpdateAsync(resetRequest);
                throw new ValidationException("INVALID_OTP");
            }

            // OTP is correct - Generate Confirm Token
            string confirmToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            resetRequest.confirm_token = confirmToken;
            // Note: Don't set is_used = true here, as ResetPasswordCommandHandler checks this flag.
            // is_used will be set to true only after password is successfully reset.
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
