using MediatR;
using ZAP.Authentication.Application.Users.DTOs;
using ZAP.Authentication.Application.Common.Interfaces;
using ZAP.Authentication.Domain.Entities;
using ZAP.Authentication.Domain.Interfaces;

namespace ZAP.Authentication.Application.Users.Commands.CreateUser
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
            var user = new User
            {
                _id = Guid.NewGuid().ToString(), // Generate string _id for new user
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FullName, // Temporary mapping FullName from register to FirstName
                MerchantName = request.MerchantName,
                Password = request.Password,
                Roles = new List<string> { "User" }
            };

            await _userRepository.CreateAsync(user);

            return new UserDto
            {
                Id = user._id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Roles = user.Roles
            };
        }
    }
}
