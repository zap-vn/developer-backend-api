using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Application.Common.Interfaces;

using CRM.Authentication.Application.Users.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Authentication.Application.Users.Commands.CheckAccountAvailability
{
    public class CheckAccountAvailabilityCommandHandler : IRequestHandler<CheckAccountAvailabilityCommand, CheckAccountResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IPhoneService _phoneService;
        private readonly IOtpRepository _otpRepository;

        public CheckAccountAvailabilityCommandHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            IPhoneService phoneService,
            IOtpRepository otpRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _phoneService = phoneService;
            _otpRepository = otpRepository;
        }

        public async Task<CheckAccountResponseDto> Handle(CheckAccountAvailabilityCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Account))
            {
                return new CheckAccountResponseDto { Success = false, Message = "Vui lòng nhập Email hoặc Số điện thoại." };
            }

            string identifier = request.Account.Trim();
            bool isEmail = identifier.Contains("@");

            // Normalize phone number if dialing_code is present
            if (!isEmail && !string.IsNullOrEmpty(request.DialingCode))
            {
                if (identifier.StartsWith("0")) identifier = identifier.Substring(1);
                identifier = request.DialingCode + identifier;
            }

            var user = await _userRepository.GetByEmailAsync(identifier);
            if (user == null && !isEmail)
            {
                user = await _userRepository.GetByPhoneAsync(identifier);
            }

            bool accountExists = user != null;

            if (!accountExists)
            {
                if (request.IsLogin)
                {
                    return new CheckAccountResponseDto 
                    { 
                        Success = false, 
                        Message = "Email hoặc số điện thoại chưa được đăng ký.",
                        Data = new CheckAccountDataDto { Exists = false, Methods = new List<string>() }
                    };
                }
                else
                {
                    // Registration case: available
                    return new CheckAccountResponseDto 
                    { 
                        Success = true, 
                        Message = "Tài khoản khả dụng.",
                        Data = new CheckAccountDataDto { Exists = false, Methods = new List<string> { "otp" } }
                    };
                }
            }

            // If account exists
            var methods = new List<string> { "otp" };
            if (user != null && !string.IsNullOrEmpty(user.password_hash))
            {
                methods.Add("password");
            }

            return new CheckAccountResponseDto
            {
                Success = true,
                Message = "Kiểm tra tài khoản thành công.",
                Data = new CheckAccountDataDto
                {
                    Exists = true,
                    Methods = methods
                }
            };
        }
    }
}
