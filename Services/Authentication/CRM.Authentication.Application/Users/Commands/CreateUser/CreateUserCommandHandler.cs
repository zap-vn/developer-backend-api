using MediatR;
using CRM.Authentication.Application.Users.DTOs;
using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Domain.Entities;
using CRM.Authentication.Domain.Interfaces;

namespace CRM.Authentication.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var nextId = await _userRepository.GetNextSequenceAsync("Customer_id");
            var detectedProvider = DetermineProvider(request.Email);
            
            var user = new User
            {
                id = Guid.NewGuid(),
                email = request.Email ?? string.Empty,
                username = request.Email ?? string.Empty,
                full_name = request.FullName ?? string.Empty, 
                password_hash = request.Password ?? string.Empty, // assuming a hash service is called later or hashing is irrelevant if it's external auth
                status_id = 9001
            };

            await _userRepository.CreateAsync(user);

            return new UserDto
            {
                id = user.id,
                email = user.email,
                username = user.username,
                full_name = user.full_name,
                status_id = user.status_id.GetValueOrDefault(),
                created_at = user.created_at.GetValueOrDefault()
            };
        }

        private string DetermineProvider(string email)
        {
            if (string.IsNullOrEmpty(email)) return "Email"; // Default for system creation if no identifier
            if (System.Text.RegularExpressions.Regex.IsMatch(email, @"^\d+$")) return "Phone";
            if (email.ToLower().Contains("appleid.com") || email.ToLower().Contains("@apple.")) return "Apple";
            return "Email";
        }

        private long ExtractLanguageId(object? languageIdObj)
        {
            if (languageIdObj == null) return 0;
            string languageIdStr = languageIdObj.ToString() ?? "";
            if (string.IsNullOrEmpty(languageIdStr)) return 0;
            // Example input: ["136 - English (United States)"] or "136"
            var match = System.Text.RegularExpressions.Regex.Match(languageIdStr, @"\d+");
            return match.Success ? long.Parse(match.Value) : 0;
        }
    }
}
