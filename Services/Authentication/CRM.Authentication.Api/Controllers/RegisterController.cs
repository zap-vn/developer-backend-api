using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM.Authentication.Application.Users.Commands.CheckAccountAvailability;
using CRM.Authentication.Application.Users.Commands.RegisterMerchant;
using CRM.Authentication.Application.Users.Queries.CheckMerchantUrl;
using CRM.Authentication.Application.Users.DTOs;
using System.Threading.Tasks;

namespace CRM.Authentication.Api.Controllers
{
    [ApiController]
    [Route("api/v1/register")]
    public class RegisterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegisterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private object CreateResponse(bool success, int code, string message, object? data = null)
        {
            return new 
            {
                success = success,
                code = code,
                message = message,
                data = data
            };
        }

        // We can't use anonymous object with JsonPropertyName easily.
        // I'll define a local record to ensure naming.
        private record ApiResponse(
            [property: System.Text.Json.Serialization.JsonPropertyName("success")] bool Success,
            [property: System.Text.Json.Serialization.JsonPropertyName("code")] int Code,
            [property: System.Text.Json.Serialization.JsonPropertyName("message")] string Message,
            [property: System.Text.Json.Serialization.JsonPropertyName("data")] object? Data = null
        );

        [HttpPost("check-account")]
        public async Task<IActionResult> CheckAccount([FromBody] CheckAccountAvailabilityCommand command)
        {
            command.IsLogin = false; // Registration context
            var result = await _mediator.Send(command);
            
            return Ok(new ApiResponse(result.Success, result.Success ? 200 : 400, result.Success ? "Account is available" : result.Message));
        }

        [HttpPost("check-email")]
        public async Task<IActionResult> CheckEmail([FromBody] CheckAccountAvailabilityCommand command)
        {
            command.IsLogin = false;
            command.Provider = "Email";
            var result = await _mediator.Send(command);
            
            return Ok(new ApiResponse(result.Success, result.Success ? 200 : 400, result.Success ? "Email is available" : result.Message));
        }

        [HttpPost("check-phone")]
        public async Task<IActionResult> CheckPhone([FromBody] CheckAccountAvailabilityCommand command)
        {
            command.IsLogin = false;
            command.Provider = "Phone";
            var result = await _mediator.Send(command);
            
            return Ok(new ApiResponse(result.Success, result.Success ? 200 : 400, result.Success ? "Phone number is available" : result.Message));
        }

        [HttpPost("check-merchant-url")]
        public async Task<IActionResult> CheckMerchantUrl([FromBody] CheckMerchantUrlQuery query)
        {
            var result = await _mediator.Send(query);
            
            return Ok(new ApiResponse(result, result ? 200 : 400, result ? "URL is available" : "URL is already taken"));
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpCommand command)
        {
            var result = await _mediator.Send(command);
            
            return Ok(new ApiResponse(result, result ? 200 : 400, result ? "OTP sent successfully" : "Failed to send OTP"));
        }

        [HttpPost("check-otp")]
        public async Task<IActionResult> CheckOtp([FromBody] VerifyRegistrationOtpCommand command)
        {
            var result = await _mediator.Send(command);
            
            return Ok(new ApiResponse(result, result ? 200 : 400, result ? "OTP verified" : "Invalid OTP"));
        }

        [HttpPost("insert-merchant")]
        public async Task<IActionResult> InsertMerchant([FromBody] RegisterMerchantCommand command)
        {
            var result = await _mediator.Send(command);
            
            return Ok(new ApiResponse(true, 200, "Registration successful"));
        }
    }
}
