using MediatR;
using System.Text.Json.Serialization;

namespace CRM.Authentication.Application.Users.Commands.ResendOtp
{
    public class ResendOtpCommand : IRequest<bool>
    {
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("Identifier")]
        public string Identifier 
        { 
            get => Email; 
            set => Email = value; 
        }

        public string Purpose { get; set; } = "register"; // register, login, reset_password
    }
}
