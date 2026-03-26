using MediatR;
using System.Text.Json.Serialization;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.CheckAccountAvailability
{
    public class CheckAccountAvailabilityCommand : IRequest<CheckAccountResponseDto>
    {
        [JsonPropertyName("account")]
        public string Account { get; set; } = string.Empty;

        [JsonPropertyName("dialing_code")]
        public string? DialingCode { get; set; } = "+84";

        public string Provider { get; set; } = "Email"; 
        public bool IsLogin { get; set; }
    }
}
