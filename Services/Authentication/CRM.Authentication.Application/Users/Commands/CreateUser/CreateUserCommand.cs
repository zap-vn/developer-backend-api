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
        string LanguageId = "[\"136 - English (United States)\"]",
        string Provider = "Email"
    ) : IRequest<UserDto>;
}
