using MediatR;

namespace CRM.Authentication.Application.Users.Commands.CheckAccountAvailability
{
    public class CheckAccountAvailabilityCommand : IRequest<bool>
    {
        public string Email { get; set; } = string.Empty;
        public string Provider { get; set; } = "Email"; // Email, Google, Facebook, Apple
        public bool IsLogin { get; set; }
    }
}
