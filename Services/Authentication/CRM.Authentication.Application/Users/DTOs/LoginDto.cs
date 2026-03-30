using System.Collections.Generic;

namespace CRM.Authentication.Application.Users.DTOs
{
    public class LoginResponseDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("success")]
        public bool Success { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("data")]
        public LoginDataDto? Data { get; set; }
    }

    public class LoginDataDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("merchant_id")]
        public string MerchantId { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;


        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("logo_url")]
        public string LogoUrl { get; set; } = string.Empty;
    }

    public class LoginV1RequestDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("account")]
        public string Account { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("password")]
        public string? Password { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("dialing_code")]
        public string DialingCode { get; set; } = "+84";
    }
}
