using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.Authentication.Domain.Interfaces;

namespace CRM.Authentication.Application.Users.Commands.CheckAccountAvailability
{
    public class VerifyRegistrationOtpCommandHandler : IRequestHandler<VerifyRegistrationOtpCommand, bool>
    {
        private readonly IOtpRepository _otpRepository;

        public VerifyRegistrationOtpCommandHandler(IOtpRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }

        public async Task<bool> Handle(VerifyRegistrationOtpCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
            {
                throw new Exception("error_missing_data|Vui lòng nhập đầy đủ thông tin xác thực.");
            }

            // Get latest OTP for the given identifier (Email or Phone)
            var customerOtp = await _otpRepository.GetLatestOtpByEmailAsync(request.Email.Trim(), "register");

            if (customerOtp == null)
            {
                throw new Exception("error_otp_not_found|Mã xác thực không hợp lệ hoặc đã hết hạn.");
            }

            if (customerOtp.VerifiedAt != null)
            {
                throw new Exception("error_otp_already_verified|Mã xác thực này đã được sử dụng.");
            }

            if (customerOtp.ExpiredAt < DateTime.UtcNow)
            {
                throw new Exception("error_otp_expired|Mã xác thực đã hết hạn.");
            }

            if (customerOtp.OtpCode != request.Otp)
            {
                customerOtp.AttemptCount++;
                await _otpRepository.UpdateAsync(customerOtp);
                throw new Exception("error_invalid_otp|Mã xác thực không chính xác.");
            }

            // OTP is correct - mark as verified
            customerOtp.VerifiedAt = DateTime.UtcNow;
            await _otpRepository.UpdateAsync(customerOtp);

            return true;
        }
    }
}
