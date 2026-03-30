#nullable enable
using MediatR;
using System.Text.Json.Serialization;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.RegisterMerchant
{
    public class RegisterMerchantCommand : IRequest<UserDto>
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("merchant_name")]
        public string MerchantName { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        [JsonIgnore]
        public string? DialingCode { get; set; } = "+84";

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonIgnore]
        public string? Language { get; set; }

        [JsonIgnore]
        public object? LanguageId { get; set; }

        [JsonIgnore]
        public string? Provider { get; set; }

        [JsonPropertyName("merchant_url")]
        public string? MerchantUrl { get; set; }
    }
}
