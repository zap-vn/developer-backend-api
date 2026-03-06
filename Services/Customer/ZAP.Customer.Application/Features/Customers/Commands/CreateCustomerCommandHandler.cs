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
                BusinessName = request.BusinessName,
                MerchantName = request.MerchantName,
                Email = request.Email,
                Password = request.Password,
                CustomerCode = request.CustomerCode,
                Visible = request.Visible,
                CreateDate = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"),
                LanguageId = request.LanguageId,
                RegistrationSource = request.RegistrationSource,
                
                // Keep backward compatible fields
                Name = string.IsNullOrEmpty(request.Name) ? request.MerchantName : request.Name,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                IsActive = request.IsActive
            };

            await _repository.CreateAsync(entity);
            return entity.Id.ToString();
        }
    }
}
