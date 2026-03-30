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
            var account = (request.Account ?? "").Trim();
            
            // Normalize phone number if dialing_code is present
            if (!string.IsNullOrEmpty(request.DialingCode) && !account.Contains("@") && account.All(char.IsDigit))
            {
                if (account.StartsWith("0")) account = account.Substring(1);
                account = request.DialingCode + account;
            }

            _logger.LogInformation("[Login] START for Identifier: {Identifier}", account);
            
            // Parallelize User and OTP lookup if possible
            var purposes = new[] { "login", "register", "forgot", "verify", "social", "resend-otp" };
            
            var userTask = _userRepository.GetByEmailAsync(account);
            var otpTask = !string.IsNullOrEmpty(request.Otp) 
                ? _otpRepository.GetLatestOtpByEmailForPurposesAsync(account, purposes)
                : Task.FromResult<CustomerOtp?>(null);

            await Task.WhenAll(userTask, otpTask);
            
            var user = userTask.Result;
            var latestOtp = otpTask.Result;

            if (user == null)
            {
                // Fallback to phone lookup if the identifier might be a phone number not found by the combined GetByEmailAsync
                user = await _userRepository.GetByPhoneAsync(account);
                if (user == null)
                {
                    _logger.LogWarning("[Login] User not found: {Identifier}", account);
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
                    _logger.LogWarning("[Login] No OTP found for {Identifier}", account);
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
                    user.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    updateTasks.Add(_userRepository.UpdateAsync(user));
                    _logger.LogInformation("[Login] Auto-verified user {Email} via OTP", user.Email);
                }
                
                await Task.WhenAll(updateTasks);
            }
            // --- Case 2: Login via Password ---
            else
            {
                var hashedInput = HashLegacyPassword(request.Password ?? "");
                bool isPasswordValid = user.Password == hashedInput || 
                                     user.Password == request.Password || 
                                     user.PasswordHash == hashedInput || 
                                     user.PasswordHash == request.Password;

                if (!isPasswordValid)
                {
                    throw new UnauthorizedAccessException("AUTH_002|AUTH_002_detail");
                }

                // Temporary bypass for verification if using PG user (optional, depending on business rule)
                if (!user.IsVerify && string.IsNullOrEmpty(user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("AUTH_001|AUTH_001_detail");
                }
            }

            if (user.Visible != 1 && user.StatusId != 1) // StatusId 1 is Active
            {
                _logger.LogWarning("[Login] Account not active for user: {Email}, Visible: {Visible}, StatusId: {StatusId}", user.Email, user.Visible, user.StatusId);
                throw new UnauthorizedAccessException("AUTH_003|AUTH_003_detail");
            }

            var tokenSw = System.Diagnostics.Stopwatch.StartNew();
            var token = await _tokenGenerator.GenerateTokenAsync(user);
            _logger.LogInformation("[Perf] Token generation took: {Elapsed}ms", tokenSw.ElapsedMilliseconds);

            _logger.LogInformation("[Login] TOTAL SUCCESS in {Elapsed}ms", sw.ElapsedMilliseconds);
            
            // Map merchant ID: prioritize Postgres Guid if available, fallback to Mongo _key
            var merchantId = user.id != Guid.Empty ? user.id.ToString() : $"merchant_{user._key}";

            return new LoginResponseDto
            {
                Success = true,
                Message = _localizer["auth_login_success"] ?? "Đăng nhập thành công",
                Data = new LoginDataDto
                {
                    Token = token,
                    MerchantId = merchantId,
                    Email = user.Email,
                    Name = !string.IsNullOrEmpty(user.MerchantName) ? user.MerchantName : user.FullName,
                    LogoUrl = string.IsNullOrEmpty(user.MerchantUrl) ? "https://api.pendogo.vn/logo.png" : user.MerchantUrl
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

