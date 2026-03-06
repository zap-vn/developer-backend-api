using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZAP.Authentication.Application.Users.Commands.CreateUser;
using ZAP.Authentication.Application.Users.Commands.LoginUser;
using ZAP.Authentication.Application.Users.DTOs;

namespace ZAP.Authentication.Api.Controllers
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
        public async Task<IActionResult> RegisterMerchant([FromBody] ZAP.Authentication.Application.Users.Commands.RegisterMerchant.RegisterMerchantCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpOptions("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || 
                string.IsNullOrEmpty(request.Password) || 
                string.IsNullOrEmpty(request.AccountName))
            {
                return BadRequest(new { Message = "AccountName, Email and Password are required." });
            }

            var command = new LoginUserCommand(request.AccountName, request.Email, request.Password);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "Auth API is running", Time = DateTime.UtcNow });
        }
    }
}
