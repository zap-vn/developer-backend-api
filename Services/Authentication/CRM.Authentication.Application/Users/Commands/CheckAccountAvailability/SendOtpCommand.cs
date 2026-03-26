using MediatR;
using System.Text.Json.Serialization;

namespace CRM.Authentication.Application.Users.Commands.CheckAccountAvailability
{
    public class SendOtpCommand : IRequest<bool>
    {
        [JsonPropertyName("account")]
        public string Account { get; set; } = string.Empty;

        [JsonPropertyName("dialing_code")]
        public string? DialingCode { get; set; } = "+84";
    }
}
