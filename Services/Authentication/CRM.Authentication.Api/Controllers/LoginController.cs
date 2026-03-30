using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.Authentication.Application.Users.Commands.LoginUser;
using CRM.Authentication.Application.Users.Commands.CheckAccountAvailability;
using CRM.Authentication.Application.Users.DTOs;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace CRM.Authentication.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoginController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 1. Check account (Login)
        /// </summary>
        [HttpPost("check-account")]
        public async Task<IActionResult> CheckAccount([FromBody] CheckAccountAvailabilityCommand command)
        {
            command.IsLogin = true; 
            var result = await _mediator.Send(command);
            return Ok(new 
            {
                success = result.Success,
                code = result.Success ? 200 : 400,
                message = result.Message,
                data = result.Data
            });
        }

        /// <summary>
        /// 2. Send OTP (Login)
        /// </summary>
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

        /// <summary>
        /// 3. Verify OTP & Login
        /// </summary>
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtpAndLogin([FromBody] LoginUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// 4. Login with password
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> LoginWithPassword([FromBody] LoginUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "Login API is running", Time = DateTime.UtcNow });
        }
    }
}
