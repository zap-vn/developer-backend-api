using MediatR;
using System.Text.Json.Serialization;

namespace CRM.Authentication.Application.Users.Commands.ResendOtp
{
    public class ResendOtpCommand : IRequest<bool>
    {
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        [JsonPropertyName("Identifier")]
        public string Identifier 
        { 
            get => !string.IsNullOrEmpty(Email) ? Email : Phone; 
            set 
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(value ?? "", @"^\d+$"))
                {
                    Phone = value!;
                }
                else
                {
                    Email = value!;
                }
            } 
        }

        public string Purpose { get; set; } = "resend-otp";
        public string Channel { get; set; } = "sms"; // sms, zalo
    }
}
