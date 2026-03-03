using MediatR;
using ZAP.Authentication.Application.Users.DTOs;

namespace ZAP.Authentication.Application.Users.Commands.CreateUser
{
    public record CreateUserCommand(
        string Username,
        string Password,
        string Email,
        string FullName,
        string MerchantName
    ) : IRequest<UserDto>;
}
