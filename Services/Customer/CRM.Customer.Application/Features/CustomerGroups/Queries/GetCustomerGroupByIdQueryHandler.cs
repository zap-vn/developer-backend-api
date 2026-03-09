using MediatR;
using CRM.Customer.Application.Features.CustomerGroups.DTOs;
using System.Threading;
using System.Threading.Tasks;
using CRM.Customer.Domain.Interfaces;
using System;

namespace CRM.Customer.Application.Features.CustomerGroups.Queries
{
    public class GetCustomerGroupByIdQueryHandler : IRequestHandler<GetCustomerGroupByIdQuery, CustomerGroupDto>
    {
        private readonly ICustomerGroupRepository _repository;

        public GetCustomerGroupByIdQueryHandler(ICustomerGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerGroupDto> Handle(GetCustomerGroupByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return null;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return null;

            return new CustomerGroupDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                DiscountPercentage = entity.DiscountPercentage
            };
        }
    }
}
