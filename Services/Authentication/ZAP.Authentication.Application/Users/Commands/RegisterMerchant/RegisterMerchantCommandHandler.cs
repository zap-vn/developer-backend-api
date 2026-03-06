using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Authentication.Application.Users.DTOs;
using ZAP.Authentication.Domain.Entities;
using ZAP.Authentication.Domain.Interfaces;

namespace ZAP.Authentication.Application.Users.Commands.RegisterMerchant
{
    public class RegisterMerchantCommandHandler : IRequestHandler<RegisterMerchantCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;

        public RegisterMerchantCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(RegisterMerchantCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.MerchantNameExistsAsync(request.MerchantName))
            {
                throw new System.Exception($"Duplicate data: Merchant Name '{request.MerchantName}' already exists.");
            }
            
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new System.Exception($"Duplicate data: Email '{request.Email}' already exists.");
            }

            if (await _userRepository.UsernameExistsAsync(request.Username))
            {
                throw new System.Exception($"Duplicate data: Username '{request.Username}' already exists.");
            }

            var user = new User
            {
                _id = System.Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = request.Email,
                MerchantName = request.MerchantName,
                Password = request.Password,
                Roles = new System.Collections.Generic.List<string> { "MerchantAdmin" }
            };

            await _userRepository.CreateAsync(user);

            // Connect to Customer table via HTTP API call
            try
            {
                using var client = new HttpClient();
                var customerPayload = new 
                {
                    CustomerCode = "MERCHANT-" + user._id.Substring(0, 6).ToUpper(),
                    MerchantName = request.MerchantName,
                    BusinessName = request.MerchantName,
                    Email = request.Email,
                    Password = request.Password,
                    Visible = 1,
                    IsActive = true,
                    LanguageId = request.LanguageId,
                    RegistrationSource = request.Provider
                };
                await client.PostAsJsonAsync("http://localhost:5003/api/customers", customerPayload, cancellationToken);
            }
            catch (System.Exception)
            {
                // Silently handle if Customer API is unreachable
            }

            return new UserDto
            {
                Id = user._id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.MerchantName, // Fallback FullName to MerchantName initially
                Roles = user.Roles
            };
        }
    }
}
