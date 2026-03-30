using MediatR;
using System.Text.Json.Serialization;

namespace CRM.Authentication.Application.Users.Commands.ActiveAccount
{
    public class ActiveAccountCommand : IRequest<bool>
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;

        [JsonPropertyName("identifier")]
        public string Identifier 
        { 
            get => Email; 
            set => Email = value; 
        }

        public ActiveAccountCommand() { }

        public ActiveAccountCommand(string email, string otp)
        {
            Email = email;
            Otp = otp;
        }
    }
}
