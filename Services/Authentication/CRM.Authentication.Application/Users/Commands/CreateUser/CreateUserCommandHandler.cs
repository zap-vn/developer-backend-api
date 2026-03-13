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
                _id = Guid.NewGuid().ToString(),
                _key = nextId,
                Email = request.Email,
                FirstName = request.FullName, 
                MerchantName = request.MerchantName,
                Password = request.Password,
                Language = string.IsNullOrEmpty(request.Language) ? request.LanguageId?.ToString() ?? "" : request.Language, 
                LanguageId = ExtractLanguageId(request.LanguageId), 
                Provider = detectedProvider,
                Roles = new List<string> { "User" },
                IsVerify = true // Direct creation might not require OTP activation by default, or you can set to false if needed
            };

            await _userRepository.CreateAsync(user);

            return new UserDto
            {
                _id = user._id,
                Email = user.Email,
                Phone = user.Phone,
                FullName = user.FullName,
                LanguageId = user.LanguageId,
                Provider = user.Provider,
                Roles = user.Roles,
                CreatedAt = user.CreatedAt,
                IsVerifyPhone = user.IsVerifyPhone,
                IsVerifyEmail = user.IsVerifyEmail,
                IsVerifyGoogle = user.IsVerifyGoogle,
                IsVerifyApple = user.IsVerifyApple,
                MerchantUrl = user.Avatar
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
