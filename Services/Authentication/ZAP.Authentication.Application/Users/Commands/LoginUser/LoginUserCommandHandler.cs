using MediatR;
using ZAP.Authentication.Application.Common.Interfaces;
using ZAP.Authentication.Application.Users.DTOs;
using ZAP.Authentication.Domain.Interfaces;
using Microsoft.Extensions.Localization;
using ZAP.BuildingBlocks.Localization;

namespace ZAP.Authentication.Application.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator,
            IStringLocalizer<SharedResource> localizer)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _localizer = localizer;
        }

        public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"[Legacy Login] START for Email: {request.Email}");
            
            var user = await _userRepository.GetByUsernameAsync(request.Email, request.AccountName);
            Console.WriteLine($"[Perf] DB Lookup took: {sw.ElapsedMilliseconds}ms");

            if (user == null)
            {
                Console.WriteLine($"[Login] User not found: {request.Email}");
                throw new UnauthorizedAccessException(_localizer["auth_invalid_credentials"] ?? "Invalid username or password.");
            }

            // Legacy Hashing Logic
            var hashingSw = System.Diagnostics.Stopwatch.StartNew();
            var hashedInput = HashLegacyPassword(request.Password);
            bool isPasswordValid = user.Password == hashedInput || user.Password == request.Password;
            Console.WriteLine($"[Perf] Hashing & Validation took: {hashingSw.ElapsedMilliseconds}ms");

            if (!isPasswordValid)
            {
                Console.WriteLine($"[Login] Password mismatch. Input hashed: {hashedInput}");
                throw new UnauthorizedAccessException(_localizer["auth_invalid_credentials"] ?? "Invalid username or password.");
            }

            // Account activation check
            if (user.Visible != 1)
            {
                Console.WriteLine($"[Login] Account not active for user: {user.Username}");
                throw new UnauthorizedAccessException(_localizer["auth_account_inactive"] ?? "Account is not active.");
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
                UserGuid = $"Customer/{user.CustomerId}",
                Permissions = new List<string>(),
                Screens = new List<string>(),
                User = new UserDto
                {
                    _id = user._id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = user.Roles
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
