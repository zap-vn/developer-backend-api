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
            var identifier = request.Email;
            
            // Find user by Email or Phone
            var user = await _userRepository.GetByEmailAsync(identifier) 
                       ?? await _userRepository.GetByPhoneAsync(identifier);

            if (user == null)
            {
                throw new Exception("Account not found.");
            }

            // If purpose is register and already verified, no need to resend
            if (request.Purpose == "register" && user.IsVerify)
            {
                throw new Exception("Account already verified.");
            }

            // Generate new 6-digit OTP
            var otp = new Random().Next(111111, 999999).ToString();

            // Store in database (CustomerOtps)
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

            // Backwards compatibility Cache (optional)
            _cache.Set($"OTP_ID_{identifier}", otp, TimeSpan.FromMinutes(15));

            // Send OTP based on Provider or availability
            if (user.Provider == "Phone" || (!string.IsNullOrEmpty(user.Phone) && string.IsNullOrEmpty(user.Email)))
            {
                await _phoneService.SendSmsOtpAsync(user.Phone, otp);
            }
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
