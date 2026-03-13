#nullable enable
using MediatR;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.RegisterMerchant
{
    public record RegisterMerchantCommand(
        string FirstName,
        string LastName,
        string MerchantName,
        string Phone,
        string? Email = null,
        string? Password = null,
        string? Language = null,
        object? LanguageId = null,
        string? Provider = null
    ) : IRequest<UserDto>;
}
