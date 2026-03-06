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

            var count = await _userRepository.GetCountAsync();
            var nextId = (int)count + 1;
            var customerIdStr = $"Customer/{nextId}";

            var user = new User
            {
                _id = customerIdStr,
                CustomerId = nextId,
                Username = request.Username,
                Email = request.Email,
                MerchantName = request.MerchantName,
                Password = request.Password,
                Roles = new System.Collections.Generic.List<string> { "MerchantAdmin" },
                Visible = 1,
                CreatedAt = System.DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss")
            };
            
            await _userRepository.CreateAsync(user);

            // Connect to Customer table via HTTP API call to ensure any Customer-specific logic triggers
            try
            {
                using var client = new HttpClient();
                var customerPayload = new 
                {
                    _id = customerIdStr, // Pass the same ID
                    CustomerId = nextId,
                    CustomerCode = "MERCHANT-" + nextId,
                    MerchantName = request.MerchantName,
                    BusinessName = request.MerchantName,
                    Email = request.Email,
                    Password = request.Password,
                    Visible = 1,
                    IsActive = true,
                    LanguageId = request.LanguageId,
                    RegistrationSource = request.Provider
                };
                
                var response = await client.PostAsJsonAsync("http://localhost:5003/api/customers", customerPayload, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    System.Console.WriteLine($"[Warning] Customer API failed: {response.StatusCode} - {errorDetails}");
                }
            }
            catch (System.Exception ex)
            {
                // Log but don't fail the whole registration if just the sync fails
                System.Console.WriteLine($"[Error] Failed to connect to Customer API: {ex.Message}");
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
