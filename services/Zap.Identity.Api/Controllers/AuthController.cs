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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    /// <param name="request">Register request</param>
    /// <returns>Tự động login và trả về JWT token</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Registration attempt for user: {Email}, merchant: {MerchantName}", 
                request.Email, request.MerchantName);

            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing registration for user: {Email}", request.Email);
            return StatusCode(500, new { 
                Message = "Internal Server Error", 
                Error = ex.Message
            });
        }
    }

}
