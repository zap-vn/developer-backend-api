using MediatR;
using System.Text.Json.Serialization;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.LoginUser
{
    public class LoginUserCommand : IRequest<LoginResponseDto>
    {
        [JsonPropertyName("account")]
        public string Account { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("dialing_code")]
        public string? DialingCode { get; set; } = "+84";
    }
}
