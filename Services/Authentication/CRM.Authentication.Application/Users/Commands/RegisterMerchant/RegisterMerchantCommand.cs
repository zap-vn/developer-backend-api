#nullable enable
using MediatR;
using System.Text.Json.Serialization;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.RegisterMerchant
{
    public record RegisterMerchantCommand(
        [property: JsonPropertyName("first_name")] string FirstName,
        [property: JsonPropertyName("last_name")] string LastName,
        [property: JsonPropertyName("merchant_name")] string MerchantName,
        [property: JsonPropertyName("phone")] string Phone,
        [property: JsonIgnore] string? DialingCode = "+84",
        [property: JsonPropertyName("email")] string? Email = null,
        [property: JsonPropertyName("password")] string? Password = null,
        [property: JsonIgnore] string? Language = null,
        [property: JsonIgnore] object? LanguageId = null,
        [property: JsonIgnore] string? Provider = null,
        [property: JsonPropertyName("merchant_url")] string? MerchantUrl = null
    ) : IRequest<UserDto>;
}
