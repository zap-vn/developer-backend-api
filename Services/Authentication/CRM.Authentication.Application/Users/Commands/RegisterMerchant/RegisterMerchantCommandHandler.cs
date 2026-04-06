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

            string finalPhone = request.Phone?.Trim() ?? "";
            if (!string.IsNullOrEmpty(finalPhone))
            {
                if (!string.IsNullOrEmpty(request.DialingCode))
                {
                    if (finalPhone.StartsWith("0")) finalPhone = finalPhone.Substring(1);
                    finalPhone = request.DialingCode + finalPhone;
                }

                if (await _userRepository.PhoneExistsAsync(finalPhone))
                {
                    throw new System.Exception("error_duplicate_phone|error_duplicate_phone_detail");
                }

                if (!IsValidPhoneNumber(finalPhone))
                {
                    throw new System.Exception("error_invalid_phone|error_invalid_phone_detail");
                }
            }

            var nextId = await _userRepository.GetNextSequenceAsync("identity.customer_id");
            var customerIdStr = $"Customer/{nextId}";

            var detectedProvider = !string.IsNullOrWhiteSpace(request.Provider) ? request.Provider : DetermineProvider(request.Email ?? "", request.Phone ?? "");
            var langId = ExtractLanguageId(request.LanguageId);
            var langCode = string.IsNullOrEmpty(request.Language) ? (langId > 0 ? "" : "en") : request.Language;

            // New PostgreSQL Guid
            var userId = System.Guid.NewGuid();

            try 
            {
                // START TRANSACTION
                await _userRepository.BeginTransactionAsync();

                // 1. Check and Map Tenant Node (Slug-based lookup)
                string merchantSlug = (request.MerchantUrl ?? "").Trim().ToLower();
                TenantNode? tenantNode = null;
                
                if (!string.IsNullOrEmpty(merchantSlug))
                {
                    tenantNode = await _userRepository.GetTenantBySlugAsync(merchantSlug);
                    
                    // 2. If Tenant doesn't exist, create it automatically (Tier 2: Brand)
                    if (tenantNode == null)
                    {
                        tenantNode = new TenantNode
                        {
                            id = System.Guid.NewGuid(),
                            name = request.MerchantName ?? "New Merchant",
                            slug = merchantSlug,
                            legacy_id = customerIdStr, // Unified legacy_id
                            node_code = ("BR-" + merchantSlug.Replace("-", "_")).ToUpper(),
                            tier_level = 2, // 2: Brand
                            status_id = 9001, // 9001: ACTIVE (platform.status_item)
                            locale_id = langId > 0 ? (int?)langId : null,
                            timezone = "Asia/Ho_Chi_Minh",
                            created_at = System.DateTime.UtcNow,
                            updated_at = System.DateTime.UtcNow
                        };
                        
                        await _userRepository.CreateTenantNodeAsync(tenantNode);
                    }
                }

                var user = new User
                {
                    id = userId,
                    tenant_id = tenantNode?.id, 
                    legacy_id = customerIdStr, 
                    username = !string.IsNullOrEmpty(request.Email) ? request.Email.Trim().ToLower() : finalPhone, 
                    email = (request.Email?.Trim() ?? "").ToLower(),
                    full_name = string.IsNullOrWhiteSpace($"{request.FirstName} {request.LastName}") ? "SYSTEM_USER" : $"{request.FirstName} {request.LastName}".Trim(),
                    password_hash = HashLegacyPassword(request.Password),
                    status_id = 9001, // 9001: ACTIVE_USER
                    created_at = System.DateTime.UtcNow,
                    updated_at = System.DateTime.UtcNow
                };

                await _userRepository.CreateAsync(user);

                // COMMIT TRANSACTION
                await _userRepository.CommitTransactionAsync();
                
                // Managed Background Queue for Sync operations
                await _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var httpClient = _httpClientFactory.CreateClient();
                    httpClient.Timeout = System.TimeSpan.FromSeconds(15);

                    try
                    {
                        var customerPayload = new 
                        {
                            id = user.id,
                            tenant_id = user.tenant_id,
                            legacy_id = user.legacy_id,
                            username = user.username,
                            email = user.email,
                            full_name = user.full_name,
                            status_id = user.status_id,
                            created_at = user.created_at.GetValueOrDefault()
                        };
                        
                        var syncUrl = $"{_customerApiUrl.TrimEnd('/')}/api/customers";
                        await httpClient.PostAsJsonAsync(syncUrl, customerPayload, token);
                    }
                    catch (System.Exception syncEx)
                    {
                        System.Console.WriteLine($"[Error] Sync failed: {syncEx.Message}");
                    }
                });

                return new UserDto
                {
                    id = user.id,
                    tenant_id = user.tenant_id,
                    legacy_id = user.legacy_id,
                    username = user.username,
                    email = user.email,
                    full_name = user.full_name,
                    status_id = user.status_id.GetValueOrDefault(),
                    created_at = user.created_at.GetValueOrDefault(),
                    updated_at = user.updated_at.GetValueOrDefault()
                };
            }
            catch (System.Exception ex)
            {
                await _userRepository.RollbackTransactionAsync();
                var innerMsg = ex.InnerException != null ? ex.InnerException.Message : "N/A";
                System.Console.WriteLine($"[CRITICAL] DATABASE_ERROR during Merchant Registration:");
                System.Console.WriteLine($"Message: {ex.Message}");
                System.Console.WriteLine($"Inner Message: {innerMsg}");
                System.Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                throw new System.Exception($"DATABASE_ERROR: {ex.Message} | INNER: {innerMsg}");
            }
        }

        private string DetermineProvider(string email, string phone)
        {
            if (string.IsNullOrEmpty(email)) return "Phone";
            if (System.Text.RegularExpressions.Regex.IsMatch(email, @"^\d+$")) return "Phone";
            if (email.ToLower().Contains("appleid.com") || email.ToLower().Contains("@apple.")) return "Apple";
            return "Email";
        }

        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            var cleaned = System.Text.RegularExpressions.Regex.Replace(phone, @"\D", "");
            return cleaned.Length >= 9 && cleaned.Length <= 15;
        }

        private long ExtractLanguageId(object? languageIdObj)
        {
            if (languageIdObj == null) return 0;
            string languageIdStr = languageIdObj.ToString() ?? "";
            if (string.IsNullOrEmpty(languageIdStr)) return 0;
            var match = System.Text.RegularExpressions.Regex.Match(languageIdStr, @"\d+");
            return match.Success ? long.Parse(match.Value) : 0;
        }

        private string HashLegacyPassword(string? password)
        {
            if (string.IsNullOrEmpty(password)) return "";
            using var md5 = System.Security.Cryptography.MD5.Create();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] hash = md5.ComputeHash(bytes);
            var sb = new System.Text.StringBuilder();
            foreach (byte b in hash) sb.Append(b.ToString("x2").ToLower());
            string md5Hash = sb.ToString();
            
            string salt = "admin@backend.api.vn";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] saltedBytes = System.Text.Encoding.UTF8.GetBytes(md5Hash + salt);
            byte[] saltedHash = sha256.ComputeHash(saltedBytes);
            return System.Convert.ToBase64String(saltedHash);
        }
    }
}
