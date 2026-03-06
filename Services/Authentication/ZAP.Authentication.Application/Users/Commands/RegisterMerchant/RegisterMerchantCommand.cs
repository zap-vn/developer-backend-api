using MediatR;
using ZAP.Authentication.Application.Users.DTOs;

namespace ZAP.Authentication.Application.Users.Commands.RegisterMerchant
{
    public record RegisterMerchantCommand(
        string MerchantName,
        string Email,
        string Username,
        string Password,
        string LanguageId = "[\"136 - English (United States)\"]",
        string Provider = "Email"
    ) : IRequest<UserDto>;
}
