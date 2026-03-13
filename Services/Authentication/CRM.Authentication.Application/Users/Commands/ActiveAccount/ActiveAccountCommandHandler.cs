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
        private readonly IOtpRepository _otpRepository;
        private readonly IMemoryCache _cache;
        private static readonly string _customerApiUrl = System.Environment.GetEnvironmentVariable("CUSTOMER_API_URL") ?? "http://localhost:5003";
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        public ActiveAccountCommandHandler(IUserRepository userRepository, IOtpRepository otpRepository, IMemoryCache cache)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _cache = cache;
        }

        public async Task<bool> Handle(ActiveAccountCommand request, CancellationToken cancellationToken)
        {
            // Try finding user by Email first, then by Phone
            var user = await _userRepository.GetByEmailAsync(request.Email) 
                       ?? await _userRepository.GetByPhoneAsync(request.Email);

            if (user == null)
            {
                throw new Exception("Account not found.");
            }

            if (user.IsVerify)
            {
                return true; // Already verified
            }

            // Verify OTP from database (CustomerOtps)
            // Look for both 'register' and 'resend-otp' purposes
            var customerOtp = await _otpRepository.GetLatestOtpForPurposesAsync(user._id, new[] { "register", "resend-otp" });
            
            if (customerOtp == null)
            {
                throw new Exception("OTP not found.");
            }

            if (customerOtp.VerifiedAt != null)
            {
                throw new Exception("OTP already verified.");
            }

            if (customerOtp.ExpiredAt < DateTime.UtcNow)
            {
                throw new Exception("AUTH_005|AUTH_005_detail");
            }

            if (customerOtp.OtpCode != request.Otp)
            {
                customerOtp.AttemptCount++;
                await _otpRepository.UpdateAsync(customerOtp);
                throw new Exception("AUTH_004|AUTH_004_detail");
            }

            // Mark OTP as verified
            customerOtp.VerifiedAt = DateTime.UtcNow;
            await _otpRepository.UpdateAsync(customerOtp);

            // Mark as verified
            user.IsVerify = true;

            // Set specific verification flags based on what was used
            if (!string.IsNullOrWhiteSpace(customerOtp.Phone))
            {
                user.IsVerifyPhone = true;
            }
            if (!string.IsNullOrWhiteSpace(customerOtp.Email))
            {
                user.IsVerifyEmail = true;
            }

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
