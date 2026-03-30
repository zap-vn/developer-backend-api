using MediatR;
using CRM.Customer.Application.Features.Customers.DTOs;
using CRM.Customer.Domain.Interfaces;
using CRM.BuildingBlocks.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Customer.Application.Features.Customers.Queries
{
    public class GetCustomerListQueryHandler : IRequestHandler<GetCustomerListQuery, PagedResult<CustomerDto>>
    {
        private readonly ICustomerRepository _repository;

        public GetCustomerListQueryHandler(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<CustomerDto>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
        {
            var filter = request.Filter ?? new FilterDTOs();
            var pagedResult = await _repository.GetPagedAsync(filter.PageIndex, filter.PageSize);
            
            var dtos = pagedResult.Items.Select(x => new CustomerDto
            {
                Id = x.Id ?? string.Empty,
                Code = x.CustomerCode,
                Name = x.Name ?? string.Empty,
                Email = x.Email ?? string.Empty,
                PhoneNumber = x.PhoneNumber ?? string.Empty,
                Address = x.Address ?? string.Empty,
                IsActive = x.IsActive
            }).ToList();

            return new PagedResult<CustomerDto>(dtos, pagedResult.TotalCount, pagedResult.CurrentPage, pagedResult.PageSize);
        }
    }
}
