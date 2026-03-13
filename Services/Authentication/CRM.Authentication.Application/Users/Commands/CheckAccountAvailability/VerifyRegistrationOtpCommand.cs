using MediatR;

namespace CRM.Authentication.Application.Users.Commands.CheckAccountAvailability
{
    public class VerifyRegistrationOtpCommand : IRequest<bool>
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}
