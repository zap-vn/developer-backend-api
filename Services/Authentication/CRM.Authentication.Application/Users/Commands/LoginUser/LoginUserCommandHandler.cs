using MediatR;
using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Application.Users.DTOs;
using CRM.Authentication.Domain.Interfaces;
using Microsoft.Extensions.Localization;
using CRM.BuildingBlocks.Localization;

namespace CRM.Authentication.Application.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IOtpRepository _otpRepository;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator,
            IOtpRepository otpRepository,
            IStringLocalizer<SharedResource> localizer)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _otpRepository = otpRepository;
            _localizer = localizer;
        }

        public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"[Legacy Login] START for Email: {request.Email}");
            
            var user = await _userRepository.GetByEmailAsync(request.Email);
            Console.WriteLine($"[Perf] DB Lookup took: {sw.ElapsedMilliseconds}ms");

            if (user == null)
            {
                Console.WriteLine($"[Login] User not found: {request.Email}");
                throw new UnauthorizedAccessException("AUTH_002|AUTH_002_detail");
            }

            // --- Case 1: Login via OTP ---
            if (!string.IsNullOrEmpty(request.Otp))
            {
                Console.WriteLine($"[Login] Attempting OTP Validation for {request.Email}");
                
                // Search for latest OTP with any common verification purpose
                var purposes = new[] { "register", "login", "forgot", "verify", "social" };
                
                // Try finding by Email
                var latestOtp = await _otpRepository.GetLatestOtpByEmailAsync(request.Email, "login")
                                ?? await _otpRepository.GetLatestOtpByEmailAsync(request.Email, "register")
                                ?? await _otpRepository.GetLatestOtpByEmailAsync(request.Email, "forgot")
                                ?? await _otpRepository.GetLatestOtpByEmailAsync(request.Email, "verify")
                                ?? await _otpRepository.GetLatestOtpByEmailAsync(request.Email, "social");

                // If not found by email, try by Phone (from the user record we just loaded)
                if (latestOtp == null && !string.IsNullOrEmpty(user.Phone))
                {
                    latestOtp = await _otpRepository.GetLatestOtpByPhoneAsync(user.Phone, "login")
                                ?? await _otpRepository.GetLatestOtpByPhoneAsync(user.Phone, "register")
                                ?? await _otpRepository.GetLatestOtpByPhoneAsync(user.Phone, "forgot")
                                ?? await _otpRepository.GetLatestOtpByPhoneAsync(user.Phone, "verify")
                                ?? await _otpRepository.GetLatestOtpByPhoneAsync(user.Phone, "social");
                }

                if (latestOtp == null)
                {
                    Console.WriteLine($"[Login] No OTP record found in CustomerOtps for {request.Email} or {user.Phone}");
                    throw new UnauthorizedAccessException("error_invalid_otp|Mã xác thực không hợp lệ hoặc đã hết hạn.");
                }

                if (latestOtp.ExpiredAt < DateTime.UtcNow)
                {
                    Console.WriteLine($"[Login] OTP Expired for {request.Email}");
                    throw new UnauthorizedAccessException("error_otp_expired|Mã xác thực đã hết hạn.");
                }

                if (latestOtp.OtpCode != request.Otp)
                {
                    Console.WriteLine($"[Login] OTP Mismatch for {request.Email}. Expected: {latestOtp.OtpCode}, Input: {request.Otp}");
                    throw new UnauthorizedAccessException("error_invalid_otp|Mã xác thực không chính xác.");
                }

                // Mark OTP as used if needed, or just proceed since login is success
                Console.WriteLine($"[Login] OTP Success for {request.Email}");
            }
            // --- Case 2: Login via Password ---
            else
            {
                var hashingSw = System.Diagnostics.Stopwatch.StartNew();
                var hashedInput = HashLegacyPassword(request.Password ?? "");
                bool isPasswordValid = user.Password == hashedInput || user.Password == request.Password;
                Console.WriteLine($"[Perf] Hashing & Validation took: {hashingSw.ElapsedMilliseconds}ms");

                if (!isPasswordValid)
                {
                    Console.WriteLine($"[Login] Password mismatch.");
                    throw new UnauthorizedAccessException("AUTH_002|AUTH_002_detail");
                }
            }

            // Account activation check
            if (!user.IsVerify)
            {
                Console.WriteLine($"[Login] Account not verified for user: {user.Email} (Identifier used: {request.Email})");
                throw new UnauthorizedAccessException("AUTH_001|AUTH_001_detail");
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
                Avatar = user.Avatar,
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
                    IsVerifyPhone = user.IsVerifyPhone,
                    IsVerifyEmail = user.IsVerifyEmail,
                    IsVerifyGoogle = user.IsVerifyGoogle
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
