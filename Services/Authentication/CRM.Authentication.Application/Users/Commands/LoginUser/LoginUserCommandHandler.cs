using MediatR;
using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Application.Users.DTOs;
using CRM.Authentication.Domain.Interfaces;
using Microsoft.Extensions.Localization;
using CRM.BuildingBlocks.Localization;
using Microsoft.Extensions.Logging;
using CRM.Authentication.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace CRM.Authentication.Application.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IOtpRepository _otpRepository;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly Microsoft.Extensions.Logging.ILogger<LoginUserCommandHandler> _logger;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator,
            IOtpRepository otpRepository,
            IStringLocalizer<SharedResource> localizer,
            Microsoft.Extensions.Logging.ILogger<LoginUserCommandHandler> _logger)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _otpRepository = otpRepository;
            _localizer = localizer;
            this._logger = _logger;
        }

        public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var email = (request.Email ?? "").Trim();
            _logger.LogInformation("[Login] START for Identifier: {Identifier}", email);
            
            // Parallelize User and OTP lookup if possible
            var purposes = new[] { "login", "register", "forgot", "verify", "social", "resend-otp" };
            
            var userTask = _userRepository.GetByEmailAsync(email);
            var otpTask = !string.IsNullOrEmpty(request.Otp) 
                ? _otpRepository.GetLatestOtpByEmailForPurposesAsync(email, purposes)
                : Task.FromResult<CustomerOtp?>(null);

            await Task.WhenAll(userTask, otpTask);
            
            var user = userTask.Result;
            var latestOtp = otpTask.Result;

            if (user == null)
            {
                // Fallback to phone lookup if the identifier might be a phone number not found by the combined GetByEmailAsync
                user = await _userRepository.GetByPhoneAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("[Login] User not found: {Identifier}", email);
                    throw new UnauthorizedAccessException("AUTH_002|AUTH_002_detail");
                }
            }

            // --- Case 1: Login via OTP ---
            if (!string.IsNullOrEmpty(request.Otp))
            {
                // If not found by email/identifier, try by User's verified phone
                if (latestOtp == null && !string.IsNullOrEmpty(user.Phone))
                {
                    latestOtp = await _otpRepository.GetLatestOtpByPhoneForPurposesAsync(user.Phone, purposes);
                }

                if (latestOtp == null)
                {
                    _logger.LogWarning("[Login] No OTP found for {Identifier}", email);
                    throw new UnauthorizedAccessException("error_invalid_otp|Mã xác thực không hợp lệ hoặc đã hết hạn.");
                }

                if (latestOtp.ExpiredAt < DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("error_otp_expired|Mã xác thực đã hết hạn.");
                }

                if (latestOtp.OtpCode != request.Otp)
                {
                    throw new UnauthorizedAccessException("error_invalid_otp|Mã xác thực không chính xác.");
                }

                // Parallelize persistence updates
                var updateTasks = new List<Task>();
                
                latestOtp.VerifiedAt = DateTime.UtcNow;
                updateTasks.Add(_otpRepository.UpdateAsync(latestOtp));

                if (!user.IsVerify)
                {
                    user.IsVerify = true;
                    if (!string.IsNullOrEmpty(latestOtp.Email)) user.IsVerifyEmail = true;
                    if (!string.IsNullOrEmpty(latestOtp.Phone)) user.IsVerifyPhone = true;
                    user.UpdatedAt = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
                    updateTasks.Add(_userRepository.UpdateAsync(user));
                    _logger.LogInformation("[Login] Auto-verified user {Email} via OTP", user.Email);
                }
                
                await Task.WhenAll(updateTasks);
            }
            // --- Case 2: Login via Password ---
            else
            {
                var hashedInput = HashLegacyPassword(request.Password ?? "");
                bool isPasswordValid = user.Password == hashedInput || user.Password == request.Password;

                if (!isPasswordValid)
                {
                    throw new UnauthorizedAccessException("AUTH_002|AUTH_002_detail");
                }

                if (!user.IsVerify)
                {
                    throw new UnauthorizedAccessException("AUTH_001|AUTH_001_detail");
                }
            }

            if (user.Visible != 1)
            {
                Console.WriteLine($"[Login] Account not active for user: {user.Email}");
                throw new UnauthorizedAccessException("AUTH_003|AUTH_003_detail");
            }

            var tokenSw = System.Diagnostics.Stopwatch.StartNew();
            var token = _tokenGenerator.GenerateToken(user);
            Console.WriteLine($"[Perf] Token generation took: {tokenSw.ElapsedMilliseconds}ms");

            Console.WriteLine($"[Legacy Login] TOTAL SUCCESS in {sw.ElapsedMilliseconds}ms");
            return new LoginResponseDto
            {
                Success = true,
                Message = _localizer["auth_login_success"] ?? "Login successful",
                MerchantName = user.MerchantName,
                AccessToken = token,
                Acronym = string.IsNullOrEmpty(user.Acronym) ? (user.FirstName.Length > 0 ? user.FirstName.Substring(0, 1) : "") + (user.LastName.Length > 0 ? user.LastName.Substring(0, 1) : "") : user.Acronym,
                Avatar = user.MerchantUrl,
                Color = "",
                ExpiresIn = 86400, // 24 hours in seconds
                FullName = user.FullName,
                RefreshToken = Guid.NewGuid().ToString(),
                Role = user.Roles.FirstOrDefault() ?? "Admin",
                UpdateDate = user.UpdatedAt,
                UserGuid = $"Customer/{user._key}",
                Permissions = new List<string>(),
                Screens = new List<string>(),
                User = new UserDto
                {
                    _id = user._id,
                    Email = user.Email,
                    Phone = user.Phone,
                    FullName = user.FullName,
                    LanguageId = user.LanguageId,
                    Roles = user.Roles,
                    CreatedAt = user.CreatedAt,
                    IsVerifyPhone = user.IsVerifyPhone,
                    IsVerifyEmail = user.IsVerifyEmail,
                    IsVerifyGoogle = user.IsVerifyGoogle,
                    IsVerifyApple = user.IsVerifyApple,
                    MerchantUrl = user.MerchantUrl
                }
            };
        }

        private string HashLegacyPassword(string password)
        {
            // 1. Get MD5 Hash (lowercase hex)
            using var md5 = System.Security.Cryptography.MD5.Create();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] hash = md5.ComputeHash(bytes);
            var sb = new System.Text.StringBuilder();
            foreach (byte b in hash) sb.Append(b.ToString("x2").ToLower());
            string md5Hash = sb.ToString();
            
            // 2. Generate Salted SHA256 Hash
            string salt = "admin@backend.api.vn";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] saltedBytes = System.Text.Encoding.UTF8.GetBytes(md5Hash + salt);
            byte[] saltedHash = sha256.ComputeHash(saltedBytes);
            return Convert.ToBase64String(saltedHash);
        }
    }
}
