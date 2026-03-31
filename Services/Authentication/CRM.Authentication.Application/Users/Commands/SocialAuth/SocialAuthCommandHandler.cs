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
                    id = Guid.NewGuid(),
                    email = request.Email,
                    full_name = $"{request.FirstName} {request.LastName}".Trim(),
                    password_hash = "", 
                    status_id = 9001, // 9001: ACTIVE_USER
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };

                await _userRepository.CreateAsync(user);

                // 3. Sync to Customer service
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var customerPayload = new
                        {
                            id = user.id,
                            full_name = user.full_name,
                            email = user.email,
                            status_id = user.status_id,
                            created_at = user.created_at.GetValueOrDefault()
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

            // 4. Generate Token and return login response
            var token = await _tokenGenerator.GenerateTokenAsync(user);

            return new LoginResponseDto
            {
                Success = true,
                Message = _localizer["auth_login_success"] ?? "Đăng nhập thành công",
                Data = new LoginDataDto
                {
                    Token = token,
                    MerchantId = user.id.ToString(),
                    Email = user.email,
                    Name = user.full_name,
                    LogoUrl = "https://api.pendogo.vn/logo.png"
                }
            };
        }
    }
}

