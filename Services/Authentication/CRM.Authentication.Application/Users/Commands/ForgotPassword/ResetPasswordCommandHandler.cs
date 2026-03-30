using MediatR;
using CRM.Authentication.Domain.Interfaces;
using Microsoft.Extensions.Localization;
using CRM.BuildingBlocks.Localization;
using System.Security.Cryptography;
using System.Text;
using CRM.BuildingBlocks.Exceptions;

namespace CRM.Authentication.Application.Users.Commands.ForgotPassword
{
    public class ResetPasswordCommand : IRequest<bool>
    {
        public string ConfirmToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetRepository _resetRepository;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ResetPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordResetRepository resetRepository,
            IStringLocalizer<SharedResource> localizer)
        {
            _userRepository = userRepository;
            _resetRepository = resetRepository;
            _localizer = localizer;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new ValidationException("PASSWORD_MISMATCH");
            }

            var resetRequest = await _resetRepository.GetByConfirmTokenAsync(request.ConfirmToken)
                               ?? await _resetRepository.GetByResetTokenAsync(request.ConfirmToken);
 
            if (resetRequest == null)
            {
                throw new ValidationException("TOKEN_INVALID");
            }
            
            // Nếu là ResetToken (từ link), đảm bảo chưa bị dùng để verify OTP (nếu còn dùng OTP)
            // Hoặc đơn giản là chưa bị đánh dấu IsUsed.
            if (resetRequest.IsUsed)
            {
                throw new ValidationException("TOKEN_ALREADY_USED");
            }

            // Confirm token should expire shortly (e.g., 15 mins after creation)
            if (resetRequest.CreatedAt.AddMinutes(15) < DateTime.UtcNow)
            {
                throw new ValidationException("TOKEN_EXPIRED");
            }

            // Get user by its Guid (Customer/Id)
            var user = await _userRepository.GetByIdAsync(resetRequest.UserGuid);

            if (user == null)
            {
                throw new ValidationException("USER_NOT_FOUND");
            }

            // Hash new password using LEGACY logic to match existing system
            user.Password = HashLegacyPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            await _userRepository.UpdateAsync(user);
            
            // Mark token as used
            resetRequest.IsUsed = true;
            await _resetRepository.UpdateAsync(resetRequest);

            return true;
        }

        private string HashLegacyPassword(string password)
        {
            using var md5 = MD5.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = md5.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (byte b in hash) sb.Append(b.ToString("x2").ToLower());
            string md5Hash = sb.ToString();
            
            string salt = "admin@backend.api.vn";
            using var sha256 = SHA256.Create();
            byte[] saltedBytes = Encoding.UTF8.GetBytes(md5Hash + salt);
            byte[] saltedHash = sha256.ComputeHash(saltedBytes);
            return Convert.ToBase64String(saltedHash);
        }
    }
}
