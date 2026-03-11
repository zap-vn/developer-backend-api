using MediatR;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.CreateUser
{
    public record CreateUserCommand(
        string Password,
        string Email,
        string FullName,
        string MerchantName,
        object? LanguageId = null, // Supports string ["136 - ..."] or numeric 136
        string? Language = null
    ) : IRequest<UserDto>;
}
