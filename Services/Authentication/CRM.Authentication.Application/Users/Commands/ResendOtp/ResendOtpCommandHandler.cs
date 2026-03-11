using MediatR;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Authentication.Application.Users.Commands.ResendOtp
{
    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpRepository _otpRepository;
        private readonly IEmailService _emailService;
        private readonly IPhoneService _phoneService;
        private readonly IMemoryCache _cache;

        public ResendOtpCommandHandler(
            IUserRepository userRepository, 
            IOtpRepository otpRepository, 
            IEmailService emailService,
            IPhoneService phoneService,
            IMemoryCache cache)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _emailService = emailService;
            _phoneService = phoneService;
            _cache = cache;
        }

        public async Task<bool> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            User? user = null;

            // 1. Find User
            if (!string.IsNullOrEmpty(request.Phone))
            {
                user = await _userRepository.GetByPhoneAsync(request.Phone);
            }
            
            if (user == null && !string.IsNullOrEmpty(request.Email))
            {
                user = await _userRepository.GetByEmailAsync(request.Email);
            }

            if (user == null)
            {
                throw new Exception("Account not found.");
            }

            // If purpose is register and already verified, no need to resend
            if (request.Purpose == "register" && user.IsVerify)
            {
                throw new Exception("Account already verified.");
            }

            // 2. Generate new 6-digit OTP
            var otp = new Random().Next(111111, 999999).ToString();

            // 3. Store in database (CustomerOtps)
            var customerOtp = new CustomerOtp
            {
                CustomerId = user._id,
                Email = user.Email,
                Phone = user.Phone,
                OtpCode = otp,
                Purpose = request.Purpose,
                ExpiredAt = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow
            };
            await _otpRepository.CreateAsync(customerOtp);

            // Backwards compatibility Cache
            string cacheKey = !string.IsNullOrEmpty(request.Phone) ? request.Phone : request.Email;
            _cache.Set($"OTP_ID_{cacheKey}", otp, TimeSpan.FromMinutes(15));

            // 4. Send OTP based on Provider or availability
            // Case: User specifically requested Phone or provider is Phone
            if (user.Provider == "Phone" || !string.IsNullOrEmpty(request.Phone))
            {
                string targetPhone = !string.IsNullOrEmpty(request.Phone) ? request.Phone : user.Phone;
                
                if (request.Channel?.ToLower() == "zalo")
                {
                    await _phoneService.SendZaloOtpAsync(targetPhone, otp);
                }
                else
                {
                    await _phoneService.SendSmsOtpAsync(targetPhone, otp);
                }
            }
            // Case: Default to Email
            else if (!string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendOtpEmailAsync(user.Email, otp, user.MerchantName);
            }
            else
            {
                throw new Exception("No valid contact method found to send OTP.");
            }

            return true;
        }
    }
}
