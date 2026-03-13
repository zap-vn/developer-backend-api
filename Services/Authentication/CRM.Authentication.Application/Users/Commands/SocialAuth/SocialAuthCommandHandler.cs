using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.Authentication.Application.Users.DTOs;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;
using CRM.BuildingBlocks.Localization;
using System.Net.Http;
using System.Net.Http.Json;

namespace CRM.Authentication.Application.Users.Commands.SocialAuth
{
    public class SocialAuthCommandHandler : IRequestHandler<SocialAuthCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private static readonly string _customerApiUrl = Environment.GetEnvironmentVariable("CUSTOMER_API_URL") ?? "http://localhost:5003";
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        public SocialAuthCommandHandler(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator,
            IStringLocalizer<SharedResource> localizer)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _localizer = localizer;
        }

        public async Task<LoginResponseDto> Handle(SocialAuthCommand request, CancellationToken cancellationToken)
        {
            // 1. Find user by Email (Social providers use Email as primary key)
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                // 2. Register new user for Social Provider
                var nextId = await _userRepository.GetNextSequenceAsync("Customer_id");
                var customerIdStr = $"Customer/{nextId}";

                user = new User
                {
                    _id = customerIdStr,
                    _key = nextId,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    MerchantName = $"{request.FirstName} {request.LastName}".Trim(),
                    BusinessName = $"{request.FirstName} {request.LastName}".Trim(),
                    Provider = request.Provider,
                    Avatar = request.Avatar,
                    IsVerify = true, // Social accounts are pre-verified
                    IsVerifyGoogle = request.Provider == "Google",
                    IsVerifyApple = request.Provider == "Apple",
                    IsVerifyEmail = true,
                    Visible = 1,
                    Roles = new System.Collections.Generic.List<string> { "MerchantAdmin" },
                    CreatedAt = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss")
                };

                await _userRepository.CreateAsync(user);

                // 3. Sync to Customer service
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var customerPayload = new
                        {
                            _id = user._id,
                            _key = user._key,
                            CustomerCode = "SOCIAL-" + user._key,
                            MerchantName = user.MerchantName,
                            BusinessName = user.BusinessName,
                            Email = user.Email,
                            IsActive = true,
                            IsVerify = user.IsVerify,
                            IsVerifyEmail = user.IsVerifyEmail,
                            IsVerifyPhone = user.IsVerifyPhone,
                            IsVerifyGoogle = user.IsVerifyGoogle,
                            IsVerifyApple = user.IsVerifyApple,
                            RegistrationSource = user.Provider,
                            MerchantUrl = user.Avatar
                        };
                        var syncUrl = $"{_customerApiUrl.TrimEnd('/')}/api/customers";
                        await _httpClient.PostAsJsonAsync(syncUrl, customerPayload);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Error] Social Sync failed: {ex.Message}");
                    }
                });
            }
            else
            {
                // Update existing user info if needed
                bool changed = false;
                if (string.IsNullOrEmpty(user.Avatar) && !string.IsNullOrEmpty(request.Avatar)) { user.Avatar = request.Avatar; changed = true; }
                if (user.Provider == "Email" && !string.IsNullOrEmpty(request.Provider)) { user.Provider = request.Provider; changed = true; }
                
                if (changed) await _userRepository.UpdateAsync(user);
            }

            // 4. Generate Token and return login response
            var token = _tokenGenerator.GenerateToken(user);

            return new LoginResponseDto
            {
                Success = true,
                Message = _localizer["auth_login_success"] ?? "Login successful",
                MerchantName = user.MerchantName,
                AccessToken = token,
                FullName = user.FullName,
                Avatar = user.Avatar,
                RefreshToken = Guid.NewGuid().ToString(),
                UserGuid = $"Customer/{user._key}",
                Role = user.Roles.FirstOrDefault() ?? "Admin",
                User = new UserDto
                {
                    _id = user._id,
                    Email = user.Email,
                    FullName = user.FullName,
                    LanguageId = user.LanguageId,
                    Roles = user.Roles,
                    CreatedAt = user.CreatedAt,
                    Phone = user.Phone,
                    IsVerifyPhone = user.IsVerifyPhone,
                    IsVerifyEmail = user.IsVerifyEmail,
                    IsVerifyGoogle = user.IsVerifyGoogle,
                    IsVerifyApple = user.IsVerifyApple,
                    MerchantUrl = user.Avatar
                }
            };
        }
    }
}
