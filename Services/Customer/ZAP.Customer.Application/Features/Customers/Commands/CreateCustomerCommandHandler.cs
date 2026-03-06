using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Customer.Domain.Entities;
using ZAP.Customer.Domain.Interfaces;
using System;

namespace ZAP.Customer.Application.Features.Customers.Commands
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, string>
    {
        private readonly ICustomerRepository _repository;

        public CreateCustomerCommandHandler(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var entity = new CustomerEntity
            {
                _id = request._id,
                CustomerId = request.CustomerId,
                _key = request._key,
                BusinessName = request.BusinessName,
                MerchantName = request.MerchantName,
                Email = request.Email ?? string.Empty,
                Password = request.Password,
                CustomerCode = request.CustomerCode,
                Visible = request.Visible,
                CreateDate = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"),
                LanguageId = request.LanguageId,
                RegistrationSource = request.RegistrationSource,
                Url = request.Url,
                
                // Keep backward compatible fields if needed
                Name = string.IsNullOrEmpty(request.Name) ? request.MerchantName : request.Name,
                PhoneNumber = request.PhoneNumber ?? string.Empty,
                Address = request.Address ?? string.Empty,
                IsActive = request.IsActive
            };

            await _repository.CreateAsync(entity);
            return entity._id;
        }
    }
}
