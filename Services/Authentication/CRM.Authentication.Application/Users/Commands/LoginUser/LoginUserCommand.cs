using MediatR;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.LoginUser
{
    public record LoginUserCommand(
        string MerchantName,
        string Email,
        string Password
    ) : IRequest<LoginResponseDto>;
}
