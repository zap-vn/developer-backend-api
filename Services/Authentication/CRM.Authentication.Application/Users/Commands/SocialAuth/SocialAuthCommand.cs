using MediatR;
using CRM.Authentication.Application.Users.DTOs;

namespace CRM.Authentication.Application.Users.Commands.SocialAuth
{
    public class SocialAuthCommand : IRequest<LoginResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty; // Google, Facebook, Apple
        public string ProviderId { get; set; } = string.Empty; // Unique ID from provider
        public string Avatar { get; set; } = string.Empty;
    }
}
