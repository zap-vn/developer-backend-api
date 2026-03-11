using MediatR;

namespace CRM.Authentication.Application.Users.Commands.ActiveAccount
{
    public record ActiveAccountCommand(string Identifier, string Otp) : IRequest<bool>;
}
