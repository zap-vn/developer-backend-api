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
    [Route("api/v1/auth")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private record ApiResponse(
            [property: System.Text.Json.Serialization.JsonPropertyName("success")] bool Success,
            [property: System.Text.Json.Serialization.JsonPropertyName("code")] int Code,
            [property: System.Text.Json.Serialization.JsonPropertyName("message")] string Message,
            [property: System.Text.Json.Serialization.JsonPropertyName("data")] object? Data = null
        );

        [HttpPost("check-account")]
        public async Task<IActionResult> CheckAccount([FromBody] CheckAccountAvailabilityCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new 
            {
                success = result.Success,
                code = result.Success ? 200 : 400,
                message = result.Message,
                data = result.Data
            });
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new 
            {
                success = result,
                code = result ? 200 : 400,
                message = result ? "OTP sent successfully" : "Failed to send OTP",
                data = (object?)null
            });
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
            var identifier = !string.IsNullOrEmpty(request.Account) ? request.Account : request.Email;
            
            if (string.IsNullOrEmpty(identifier))
            {
                return BadRequest(new { Message = "Account (Email/Phone) is required." });
            }

            if (string.IsNullOrEmpty(request.Password) && string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest(new { Message = "Either Password or OTP is required." });
            }
            
            var command = new LoginUserCommand(identifier, request.Password, request.Otp);
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
            return Ok(new 
            {
                success = result.Success,
                code = result.Success ? 200 : 400,
                message = result.Success ? "OTP verified" : result.Message,
                data = result.ConfirmToken != null ? new { confirm_token = result.ConfirmToken } : null
            });
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
