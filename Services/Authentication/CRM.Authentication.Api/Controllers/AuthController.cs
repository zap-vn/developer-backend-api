using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.Authentication.Application.Users.Commands.CreateUser;
using CRM.Authentication.Application.Users.Commands.LoginUser;
using CRM.Authentication.Application.Users.Commands.RegisterMerchant;
using CRM.Authentication.Application.Users.Commands.ForgotPassword;
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

        [HttpOptions("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // Support aliases: UserName -> Email, MerchantName -> AccountName
            var email = request.Email ?? request.UserName;
            var accountName = request.AccountName ?? request.MerchantName;

            if (string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(request.Password) || 
                string.IsNullOrEmpty(accountName))
            {
                return BadRequest(new { Message = "AccountName (MerchantName), Email (UserName) and Password are required." });
            }

            var command = new LoginUserCommand(accountName, email, request.Password);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
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
