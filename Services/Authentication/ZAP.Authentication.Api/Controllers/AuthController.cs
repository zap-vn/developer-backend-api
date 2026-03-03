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

        [HttpOptions("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.UserName) || 
                string.IsNullOrEmpty(request.Password) || 
                string.IsNullOrEmpty(request.MerchantName))
            {
                return BadRequest(new { Message = "UserName, Password and MerchantName are required." });
            }

            var command = new LoginUserCommand(request.UserName, request.Password, request.MerchantName);
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
