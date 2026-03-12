#nullable enable
using MediatR;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.RegisterMerchant
{
    public record RegisterMerchantCommand(
        string MerchantName,
        string Email,
        string Phone,
        string Password,
        string? Language = null,
        object? LanguageId = null,
        string? Provider = null
    ) : IRequest<UserDto>;
}
