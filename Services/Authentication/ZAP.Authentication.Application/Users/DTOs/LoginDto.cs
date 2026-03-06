namespace ZAP.Authentication.Application.Users.DTOs
{
    public class LoginResponseDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("Success")]
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
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
        
        [System.Text.Json.Serialization.JsonIgnore]
        public UserDto User { get; set; } = new(); // Keep for backward compatibility if needed internally
    }

    public class LoginRequestDto
    {
        public string AccountName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsRemember { get; set; }
    }
}
