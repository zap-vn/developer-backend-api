using MediatR;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.CreateUser
{
    public record CreateUserCommand(
        string Username,
        string Password,
        string Email,
        string FullName,
        string MerchantName,
        object LanguageId, // Supports string ["136 - ..."] or numeric 136
        string? Language = null,
        string Provider = "Email"
    ) : IRequest<UserDto>;
}
