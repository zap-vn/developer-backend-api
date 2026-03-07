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
            var user = new User
            {
                _id = Guid.NewGuid().ToString(), // Generate string _id for new user
                _key = nextId,
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FullName, // Temporary mapping FullName from register to FirstName
                MerchantName = request.MerchantName,
                Password = request.Password,
                LanguageId = request.LanguageId,
                Provider = request.Provider,
                Roles = new List<string> { "User" }
            };

            await _userRepository.CreateAsync(user);

            return new UserDto
            {
                _id = user._id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                LanguageId = user.LanguageId,
                Provider = user.Provider,
                Roles = user.Roles
            };
        }
    }
}
