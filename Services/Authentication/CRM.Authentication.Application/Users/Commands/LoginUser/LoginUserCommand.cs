using MediatR;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.LoginUser
{
    public record LoginUserCommand(
        string Email,
        string? Password = null,
        string? Otp = null
    ) : IRequest<LoginResponseDto>;
}
