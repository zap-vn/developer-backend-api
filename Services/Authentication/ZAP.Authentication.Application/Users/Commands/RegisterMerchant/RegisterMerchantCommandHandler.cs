using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Authentication.Application.Users.DTOs;
using ZAP.Authentication.Domain.Entities;
using ZAP.Authentication.Domain.Interfaces;

namespace ZAP.Authentication.Application.Users.Commands.RegisterMerchant
{
    public class RegisterMerchantCommandHandler : IRequestHandler<RegisterMerchantCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private static readonly string _customerApiUrl = System.Environment.GetEnvironmentVariable("CUSTOMER_API_URL") ?? "http://localhost:5003";
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = System.TimeSpan.FromSeconds(10) };

        public RegisterMerchantCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(RegisterMerchantCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.MerchantNameExistsAsync(request.MerchantName))
            {
                throw new System.Exception($"Duplicate data: Merchant Name '{request.MerchantName}' already exists.");
            }
            
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new System.Exception($"Duplicate data: Email '{request.Email}' already exists.");
            }

            if (await _userRepository.UsernameExistsAsync(request.Username))
            {
                throw new System.Exception($"Duplicate data: Username '{request.Username}' already exists.");
            }

            var nextId = await _userRepository.GetNextSequenceAsync("Customer_id");
            var customerIdStr = $"Customer/{nextId}";

            var user = new User
            {
                _id = customerIdStr,
                CustomerId = (int)nextId,
                Email = request.Email,
                Username = request.Username,
                MerchantName = request.MerchantName,
                BusinessName = request.MerchantName,
                AccountName = request.MerchantName,
                Language = request.LanguageId.Contains("Vietnamese") ? "vi" : "en",
                Password = HashLegacyPassword(request.Password),
                Roles = new System.Collections.Generic.List<string> { "MerchantAdmin" },
                Visible = 1,
                Avatar = request.Url,
                CreatedAt = System.DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss")
            };
            
            await _userRepository.CreateAsync(user);

            // Connect to Customer service via HTTP API call
            try
            {
                var customerPayload = new 
                {
                    _id = customerIdStr, // Pass the same ID
                    CustomerId = (int)nextId,
                    _key = (int)nextId,
                    CustomerCode = "MERCHANT-" + nextId,
                    MerchantName = request.MerchantName,
                    BusinessName = request.MerchantName,
                    Email = request.Email,
                    Password = HashLegacyPassword(request.Password),
                    Visible = 1,
                    IsActive = true,
                    LanguageId = request.LanguageId,
                    RegistrationSource = request.Provider,
                    Url = request.Url
                };
                
                var syncUrl = $"{_customerApiUrl.TrimEnd('/')}/api/customers";
                var response = await _httpClient.PostAsJsonAsync(syncUrl, customerPayload, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    System.Console.WriteLine($"[Warning] Customer API failed: {response.StatusCode} - {errorDetails} (URL: {syncUrl})");
                }
            }
            catch (System.Exception ex)
            {
                // Log but don't fail the whole registration if just the sync fails
                System.Console.WriteLine($"[Error] Failed to connect to Customer API: {ex.Message}");
            }

            return new UserDto
            {
                _id = user._id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.MerchantName, // Fallback FullName to MerchantName initially
                Roles = user.Roles
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
            return System.Convert.ToBase64String(saltedHash);
        }
    }
}
