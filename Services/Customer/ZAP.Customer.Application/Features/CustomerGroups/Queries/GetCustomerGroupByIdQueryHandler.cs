using MediatR;
using ZAP.Customer.Application.Features.CustomerGroups.DTOs;
using System.Threading;
using System.Threading.Tasks;
using ZAP.Customer.Domain.Interfaces;
using System;

namespace ZAP.Customer.Application.Features.CustomerGroups.Queries
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
            if (!Guid.TryParse(request.Id, out Guid parsedId)) return null;
            var entity = await _repository.GetByIdAsync(parsedId);
            if (entity == null) return null;

            return new CustomerGroupDto 
            { 
#pragma warning disable CS8602
                Id = entity.Id.ToString(),
                Name = entity.Name,
                Description = entity.Description,
                DiscountPercentage = entity.DiscountPercentage
#pragma warning restore CS8602
            };
        }
    }
}
