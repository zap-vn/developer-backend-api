using MediatR;
using CRM.Customer.Application.Features.Customers.DTOs;
using CRM.Customer.Domain.Interfaces;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Customer.Application.Features.Customers.Queries
{
    public class GetCustomerListQueryHandler : IRequestHandler<GetCustomerListQuery, PagedResult<CustomerDto>>
    {
        private readonly ICustomerRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetCustomerListQueryHandler(ICustomerRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<CustomerDto>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
        {
            var filter = request.Filter ?? new FilterDTOs();
            
            Guid? tenantId = null;
            if (Guid.TryParse(_currentUserService.UserGuid, out var tenantGuid))
            {
                tenantId = tenantGuid;
            }

            var pagedResult = await _repository.GetPagedAsync(filter.PageIndex, filter.PageSize, tenantId, filter.Keyword);
            
            var dtos = pagedResult.Items.Select(x => new CustomerDto
            {
                Id = x.id.ToString(),
                Code = x.CustomerCode,
                Name = x.full_name ?? string.Empty,
                Email = x.email ?? string.Empty,
                PhoneNumber = x.phone_number ?? string.Empty,
                Address = x.Address ?? string.Empty,
                IsActive = x.status_id == 1 // or other logic based on status_id
            }).ToList();

            return new PagedResult<CustomerDto>(dtos, pagedResult.TotalCount, pagedResult.CurrentPage, pagedResult.PageSize);
        }
    }
}
