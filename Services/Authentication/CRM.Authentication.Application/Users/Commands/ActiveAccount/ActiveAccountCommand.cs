using MediatR;

namespace CRM.Authentication.Application.Users.Commands.ActiveAccount
{
    public record ActiveAccountCommand(string Email, string Otp) : IRequest<bool>;
}
