using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CRM.Authentication.Application.Users.DTOs
{
    public class CheckAccountResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public CheckAccountDataDto? Data { get; set; }
    }

    public class CheckAccountDataDto
    {
        [JsonPropertyName("exists")]
        public bool Exists { get; set; }

        [JsonPropertyName("methods")]
        public List<string> Methods { get; set; } = new();
    }

    public class CheckAccountRequestDto
    {
        [JsonPropertyName("account")]
        public string Account { get; set; } = string.Empty;
    }
}
