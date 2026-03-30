using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CRM.Authentication.Infrastructure.Security;

namespace CRM.Authentication.Api.Controllers
{
    [ApiController]
    [Route("api/otp")]
    public class OtpController : ControllerBase
    {
        private readonly OtpService _otpService;

        public OtpController(OtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        {
            var status = await _otpService.SendOtpAsync(request.PhoneNumber);

            return Ok(new
            {
                success = true,
                status
            });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyRequest request)
        {
            var isValid = await _otpService.VerifyOtpAsync(
                request.PhoneNumber,
                request.Code
            );

            return Ok(new
            {
                success = isValid
            });
        }
    }

    public class OtpRequest
    {
        public string PhoneNumber { get; set; }
    }

    public class VerifyRequest
    {
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
    }
}
