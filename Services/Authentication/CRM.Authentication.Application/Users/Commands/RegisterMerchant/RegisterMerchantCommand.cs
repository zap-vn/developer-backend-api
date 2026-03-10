using MediatR;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.RegisterMerchant
{
    public record RegisterMerchantCommand(
        string MerchantName,
        string Email,
        string Username,
        string Password,
        object LanguageId, // Supports string ["136 - ..."] or numeric 136
        string? Language = null,
        string Provider = "Email",
        string Url = ""
    ) : IRequest<UserDto>;
}
