using MediatR;
using System.Text.Json.Serialization;

namespace CRM.Authentication.Application.Users.Commands.CheckAccountAvailability
{
    public class VerifyRegistrationOtpCommand : IRequest<bool>
    {
        [JsonPropertyName("account")]
        public string Account { get; set; } = string.Empty;

        [JsonPropertyName("otp")]
        public string Otp { get; set; } = string.Empty;
    }
}
