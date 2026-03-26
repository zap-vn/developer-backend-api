using MediatR;
using CRM.Customer.Application.Features.Customers.DTOs;
using CRM.Customer.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Customer.Application.Features.Customers.Queries
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
    {
        private readonly ICustomerRepository _repository;

        public GetCustomerByIdQueryHandler(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return null;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return null;

            return new CustomerDto
            {
                Id = entity.Id ?? string.Empty,
                Code = entity.CustomerCode,
                Name = entity.Name ?? string.Empty,
                Email = entity.Email ?? string.Empty,
                PhoneNumber = entity.PhoneNumber ?? string.Empty,
                Address = entity.Address ?? string.Empty,
                IsActive = entity.IsActive
            };
        }
    }
}
