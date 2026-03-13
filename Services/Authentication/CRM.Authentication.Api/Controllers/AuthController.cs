using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.Authentication.Application.Users.Commands.CreateUser;
using CRM.Authentication.Application.Users.Commands.LoginUser;
using CRM.Authentication.Application.Users.Commands.RegisterMerchant;
using CRM.Authentication.Application.Users.Commands.ForgotPassword;
using CRM.Authentication.Application.Users.Commands.ActiveAccount;
using CRM.Authentication.Application.Users.Commands.ResendOtp;
using CRM.Authentication.Application.Users.Commands.CheckAccountAvailability;
using CRM.Authentication.Application.Users.Commands.SocialAuth;
using CRM.Authentication.Application.Users.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace CRM.Authentication.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("check-account")]
        public async Task<IActionResult> CheckAccount([FromBody] CheckAccountAvailabilityCommand command)
        {
            var result = await _mediator.Send(command);
            string message = command.IsLogin ? "Tài khoản hợp lệ." : "Mã OTP đã được gửi.";
            return Ok(new { Success = result, Message = message });
        }

        [HttpPost("verify-registration-otp")]
        public async Task<IActionResult> VerifyRegistrationOtp([FromBody] VerifyRegistrationOtpCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "Xác thực thành công." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("register-merchant")]
        public async Task<IActionResult> RegisterMerchant([FromBody] RegisterMerchantCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("active-account")]
        public async Task<IActionResult> ActiveAccount([FromBody] ActiveAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = result ? "Tài khoản đã được kích hoạt" : "Kích hoạt thất bại" });
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = result ? "Mã OTP đã được gửi lại" : "Gửi lại thất bại" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || 
                string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "Email and Password are required." });
            }
            
            var command = new LoginUserCommand(request.Email, request.Password);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("social-login")]
        public async Task<IActionResult> SocialLogin([FromBody] SocialAuthCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("forgot-password/phone")]
        public async Task<IActionResult> ForgotPasswordPhone([FromBody] ForgotPasswordPhoneRequestDto request)
        {
            var command = new ForgotPasswordPhoneCommand(request.Phone, request.Channel);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = result ? "Mật khẩu đã được cập nhật" : "Thất bại" });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "Auth API is running", Time = DateTime.UtcNow });
        }
    }
}
