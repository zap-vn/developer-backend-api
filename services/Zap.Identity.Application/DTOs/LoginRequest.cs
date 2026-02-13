using System.ComponentModel.DataAnnotations;

namespace Zap.Identity.Application.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "UserName is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    public string? MerchantName { get; set; }

    public bool IsRemember { get; set; }
}
