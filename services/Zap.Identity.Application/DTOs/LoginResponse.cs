namespace Zap.Identity.Application.DTOs;

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public CustomerInfo? Customer { get; set; }
}

public class CustomerInfo
{
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
    public string TimeZoneId { get; set; } = string.Empty;
}
