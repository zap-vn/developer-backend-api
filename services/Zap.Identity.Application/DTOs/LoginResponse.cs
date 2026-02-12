using System.Collections.Generic;

namespace Zap.Identity.Application.DTOs;

public class LoginResponse
{
    // Meta info for the internal API logic
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    // The flat properties requested by the user
    public string MerchantName { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string Acronym { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UpdateDate { get; set; } = string.Empty;
    public string UserGuid { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public List<string> Screens { get; set; } = new();
}
