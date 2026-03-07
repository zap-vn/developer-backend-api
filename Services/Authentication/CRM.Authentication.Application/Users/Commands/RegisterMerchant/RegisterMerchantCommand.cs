using MediatR;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.RegisterMerchant
{
    public record RegisterMerchantCommand(
        string MerchantName,
        string Email,
        string Username,
        string Password,
        string LanguageId = "[\"136 - English (United States)\"]",
        string Provider = "Email",
        string Url = ""
    ) : IRequest<UserDto>;
}
