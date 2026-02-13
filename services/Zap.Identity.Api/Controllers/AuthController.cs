using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;

namespace Zap.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login với email, password và merchant name
    /// </summary>
    /// <param name="request">Login request</param>
    /// <returns>JWT token và thông tin customer</returns>
    /// 
    [AllowAnonymous]
    [HttpOptions("login")]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try 
        {
            if (string.IsNullOrEmpty(request.MerchantName))
            {
                return Unauthorized(new LoginResponse { Success = false, Message = "MerchantName is required." });
            }

            _logger.LogInformation("Login attempt for user: {UserName}, merchant: {MerchantName}", 
                request.UserName, request.MerchantName);

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                _logger.LogWarning("Login failed for user: {UserName}, reason: {Reason}", 
                    request.UserName, result.Message);
                return Unauthorized(result);
            }

            _logger.LogInformation("Login successful for user: {UserName}", request.UserName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing login for user: {UserName}", request.UserName);
            return StatusCode(500, new { 
                Message = "Internal Server Error", 
                Error = ex.Message, 
                Stack = ex.StackTrace 
            });
        }
    }
}
