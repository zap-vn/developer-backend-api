using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CRM.Authentication.Application.Users.DTOs;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace CRM.Authentication.Application.Users.Commands.RegisterMerchant
{
    public class RegisterMerchantCommandHandler : IRequestHandler<RegisterMerchantCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpRepository _otpRepository;
        private readonly IMemoryCache _cache;
        private readonly CRM.BuildingBlocks.Interfaces.IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private static readonly string _customerApiUrl = System.Environment.GetEnvironmentVariable("CUSTOMER_API_URL") ?? "http://localhost:5003";

        public RegisterMerchantCommandHandler(
            IUserRepository userRepository,
            IOtpRepository otpRepository,
            IMemoryCache cache,
            CRM.BuildingBlocks.Interfaces.IBackgroundTaskQueue backgroundTaskQueue,
            IHttpClientFactory httpClientFactory,
            IServiceScopeFactory serviceScopeFactory)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _cache = cache;
            _backgroundTaskQueue = backgroundTaskQueue;
            _httpClientFactory = httpClientFactory;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<UserDto> Handle(RegisterMerchantCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.Phone))
            {
                throw new System.Exception("error_missing_contact|error_missing_contact_detail");
            }

            if (await _userRepository.MerchantNameExistsAsync(request.MerchantName))
            {
                throw new System.Exception("error_duplicate_merchant_name|error_duplicate_merchant_name_detail");
            }
            
            if (!string.IsNullOrWhiteSpace(request.Email) && await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new System.Exception("error_duplicate_email|error_duplicate_email_detail");
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                string phone = request.Phone.Trim();
                if (!string.IsNullOrEmpty(request.DialingCode))
                {
                    if (phone.StartsWith("0")) phone = phone.Substring(1);
                    phone = request.DialingCode + phone;
                }

                if (await _userRepository.PhoneExistsAsync(phone))
                {
                    throw new System.Exception("error_duplicate_phone|error_duplicate_phone_detail");
                }

                if (!IsValidPhoneNumber(phone))
                {
                    throw new System.Exception("error_invalid_phone|error_invalid_phone_detail");
                }
            }

            var nextId = await _userRepository.GetNextSequenceAsync("Customer_id");
            var customerIdStr = $"Customer/{nextId}";

            // Generate 6-digit OTP
            var otp = new System.Random().Next(100000, 999999).ToString();
            var detectedProvider = !string.IsNullOrWhiteSpace(request.Provider) ? request.Provider : DetermineProvider(request.Email ?? "", request.Phone ?? "");
            var langId = ExtractLanguageId(request.LanguageId);
            var langCode = string.IsNullOrEmpty(request.Language) ? (langId > 0 ? "" : "en") : request.Language;

            string finalPhone = request.Phone?.Trim() ?? "";
            if (!string.IsNullOrEmpty(finalPhone) && !string.IsNullOrEmpty(request.DialingCode))
            {
                if (finalPhone.StartsWith("0")) finalPhone = finalPhone.Substring(1);
                finalPhone = request.DialingCode + finalPhone;
            }

            // Generate a New Guid for PostgreSQL
            var userId = System.Guid.NewGuid();
            // Using null for TenantId to avoid Foreign Key constraint issues if the default tenant node is missing.
            // Guidance: Ensure the tenant exists in the platform.tenant_node table before enabling this.
            System.Guid? tenantId = null; 

            var user = new User
            {
                id = userId,
                TenantId = tenantId,
                Username = request.Email?.Trim() ?? "", 
                FullName = $"{request.FirstName} {request.LastName}".Trim(),
                PasswordHash = string.IsNullOrWhiteSpace(request.Password) ? "" : HashLegacyPassword(request.Password),
                StatusId = 1, // Set active status as requested
                
                // Legacy fields for backward compatibility
                _id = customerIdStr,
                _key = nextId,
                FirstName = request.FirstName?.Trim() ?? "",
                LastName = request.LastName?.Trim() ?? "",
                Email = request.Email?.Trim() ?? "",
                Phone = finalPhone,
                MerchantName = request.MerchantName,
                BusinessName = request.MerchantName,
                Language = langCode, 
                LanguageId = langId, 
                Password = string.IsNullOrWhiteSpace(request.Password) ? "" : HashLegacyPassword(request.Password),
                Provider = detectedProvider,
                Roles = new System.Collections.Generic.List<string> { "MerchantAdmin" },
                Visible = 1,
                MerchantUrl = request.MerchantUrl ?? "",
                IsVerify = !string.IsNullOrWhiteSpace(request.Email),
                IsVerifyGoogle = detectedProvider == "Google",
                IsVerifyApple = detectedProvider == "Apple",
                IsVerifyPhone = false,
                IsVerifyEmail = !string.IsNullOrWhiteSpace(request.Email),
                CreatedAt = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                CreatedAtDate = System.DateTime.UtcNow,
                UpdatedAtDate = System.DateTime.UtcNow
            };

            // Store OTP in database (CustomerOtps) - MongoDB is currently being used, wrapping in try-catch to avoid crash if flaky.
            try 
            {
                var customerOtp = new CustomerOtp
                {
                    CustomerId = user._id,
                    Email = user.Email,
                    Phone = user.Phone,
                    OtpCode = otp,
                    Purpose = "register",
                    ExpiredAt = DateTime.UtcNow.AddMinutes(15),
                    CreatedAt = DateTime.UtcNow
                };
                await _otpRepository.CreateAsync(customerOtp);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"[Warning] Failed to store OTP in MongoDB: {ex.Message}. Registration will continue using MemoryCache fallback.");
            }

            // Keep cache for backward compatibility during transition if needed, or remove
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                _cache.Set($"OTP_ID_{request.Email}", otp, System.TimeSpan.FromMinutes(15));
            }
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                _cache.Set($"OTP_ID_{request.Phone}", otp, System.TimeSpan.FromMinutes(15));
            }
            
            try 
            {
                await _userRepository.CreateAsync(user);
            }
            catch (System.Exception ex)
            {
                // Detailed database errors are handled by global middleware
                throw new System.Exception($"DATABASE_ERROR: {ex.Message}");
            }

            // Managed Background Queue for Sync operations
            _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = System.TimeSpan.FromSeconds(15);

                // Connect to Customer service via HTTP API call
                try
                {
                    var customerPayload = new 
                    {
                        _id = customerIdStr, 
                        _key = nextId,
                        CustomerCode = "MERCHANT-" + nextId,
                        MerchantName = request.MerchantName,
                        BusinessName = request.MerchantName,
                        FirstName = request.FirstName?.Trim() ?? "",
                        LastName = request.LastName?.Trim() ?? "",
                        Email = request.Email?.Trim() ?? "",
                        Phone = request.Phone?.Trim() ?? "",
                        Password = string.IsNullOrWhiteSpace(request.Password) ? "" : HashLegacyPassword(request.Password),
                        Visible = 1,
                        IsActive = !string.IsNullOrWhiteSpace(request.Email), // Set to active if email is verified
                        IsVerify = user.IsVerify,
                        IsVerifyEmail = user.IsVerifyEmail,
                        IsVerifyPhone = user.IsVerifyPhone,
                        IsVerifyGoogle = user.IsVerifyGoogle,
                        IsVerifyApple = user.IsVerifyApple,
                        LanguageId = langId,
                        Language = langCode, 
                        RegistrationSource = detectedProvider,
                        MerchantUrl = request.MerchantUrl ?? ""
                    };
                    
                    var syncUrl = $"{_customerApiUrl.TrimEnd('/')}/api/customers";
                    var response = await httpClient.PostAsJsonAsync(syncUrl, customerPayload, token);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorDetails = await response.Content.ReadAsStringAsync(token);
                        System.Console.WriteLine($"[Warning] Customer API failed: {response.StatusCode} - {errorDetails} (URL: {syncUrl})");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"[Error] Failed to connect to Customer API during background sync: {ex.Message}");
                }
            });

            return new UserDto
            {
                _id = user._id,
                Email = user.Email,
                Phone = user.Phone,
                FullName = string.IsNullOrWhiteSpace(user.FullName) ? user.MerchantName : user.FullName.Trim(), 
                LanguageId = user.LanguageId,
                Provider = user.Provider,
                Roles = user.Roles,
                CreatedAt = user.CreatedAt,
                IsVerifyPhone = user.IsVerifyPhone,
                IsVerifyEmail = user.IsVerifyEmail,
                IsVerifyGoogle = user.IsVerifyGoogle,
                IsVerifyApple = user.IsVerifyApple,
                MerchantUrl = user.MerchantUrl
            };
        }

        private string DetermineProvider(string email, string phone)
        {
            if (string.IsNullOrEmpty(email)) return "Phone";
            
            // If email is just numbers, it's likely a phone number used as identifier
            if (System.Text.RegularExpressions.Regex.IsMatch(email, @"^\d+$")) return "Phone";
            
            // Check for Apple domains
            if (email.ToLower().Contains("appleid.com") || email.ToLower().Contains("@apple.")) return "Apple";
            
            return "Email";
        }

        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            // Remove non-digit characters and check length (supporting 9-15 digits for international)
            var cleaned = System.Text.RegularExpressions.Regex.Replace(phone, @"\D", "");
            return cleaned.Length >= 9 && cleaned.Length <= 15;
        }

        private long ExtractLanguageId(object? languageIdObj)
        {
            if (languageIdObj == null) return 0;
            string languageIdStr = languageIdObj.ToString() ?? "";
            if (string.IsNullOrEmpty(languageIdStr)) return 0;
            // Example input: ["136 - English (United States)"] or "136"
            var match = System.Text.RegularExpressions.Regex.Match(languageIdStr, @"\d+");
            return match.Success ? long.Parse(match.Value) : 0;
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
