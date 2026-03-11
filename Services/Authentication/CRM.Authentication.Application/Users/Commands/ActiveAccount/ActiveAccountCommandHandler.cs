using MediatR;
using CRM.Authentication.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Net.Http.Json;

namespace CRM.Authentication.Application.Users.Commands.ActiveAccount
{
    public class ActiveAccountCommandHandler : IRequestHandler<ActiveAccountCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;
        private static readonly string _customerApiUrl = System.Environment.GetEnvironmentVariable("CUSTOMER_API_URL") ?? "http://localhost:5003";
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        public ActiveAccountCommandHandler(IUserRepository userRepository, IMemoryCache cache)
        {
            _userRepository = userRepository;
            _cache = cache;
        }

        public async Task<bool> Handle(ActiveAccountCommand request, CancellationToken cancellationToken)
        {
            // Try finding user by Email first, then by Phone
            var user = await _userRepository.GetByEmailAsync(request.Identifier) 
                       ?? await _userRepository.GetByPhoneAsync(request.Identifier);

            if (user == null)
            {
                throw new Exception("Account not found.");
            }

            if (user.IsVerify)
            {
                return true; // Already verified
            }

            // Get OTP from Cache using the provided Identifier
            if (!_cache.TryGetValue($"OTP_ID_{request.Identifier}", out string? cachedOtp))
            {
                throw new Exception("OTP has expired or not found.");
            }

            if (cachedOtp != request.Otp)
            {
                throw new Exception("Invalid OTP.");
            }

            // Mark as verified
            user.IsVerify = true;

            // Clear cache for both Email and Phone to be safe
            if (!string.IsNullOrWhiteSpace(user.Email)) _cache.Remove($"OTP_ID_{user.Email}");
            if (!string.IsNullOrWhiteSpace(user.Phone)) _cache.Remove($"OTP_ID_{user.Phone}");

            await _userRepository.UpdateAsync(user);

            // Optional: Notify Customer Service if needed
            // For now, since there's no update endpoint in Customer service, we just update Authentication
            
            return true;
        }
    }
}
