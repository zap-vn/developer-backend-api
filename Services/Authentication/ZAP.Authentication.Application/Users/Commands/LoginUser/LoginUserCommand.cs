using MediatR;
using ZAP.Authentication.Application.Users.DTOs;

namespace ZAP.Authentication.Application.Users.Commands.LoginUser
{
    public record LoginUserCommand(
        string Username,
        string Password,
        string MerchantName
    ) : IRequest<LoginResponseDto>;
}
