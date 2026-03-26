using MediatR;
using System.Text.Json.Serialization;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.LoginUser
{
    public record LoginUserCommand(
        [property: JsonPropertyName("account")] string Account,
        [property: JsonPropertyName("password")] string? Password = null,
        [property: JsonPropertyName("otp")] string? Otp = null,
        [property: JsonPropertyName("dialing_code")] string? DialingCode = "+84"
    ) : IRequest<LoginResponseDto>;
}
