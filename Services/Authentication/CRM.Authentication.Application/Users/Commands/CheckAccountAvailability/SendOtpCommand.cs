using MediatR;
using System.Text.Json.Serialization;

namespace CRM.Authentication.Application.Users.Commands.CheckAccountAvailability
{
    public class SendOtpCommand : IRequest<bool>
    {
        [JsonPropertyName("account")]
        public string Account { get; set; } = string.Empty;

        [JsonIgnore]
        public string? DialingCode { get; set; } = "+84";
    }
}
