using MediatR;
using ZAP.Customer.Application.Features.CustomerGroups.DTOs;
using ZAP.BuildingBlocks.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ZAP.Customer.Domain.Interfaces;

namespace ZAP.Customer.Application.Features.CustomerGroups.Queries
{
    public class GetCustomerGroupListQueryHandler : IRequestHandler<GetCustomerGroupListQuery, PagedResult<CustomerGroupDto>>
    {
        private readonly ICustomerGroupRepository _repository;

        public GetCustomerGroupListQueryHandler(ICustomerGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<CustomerGroupDto>> Handle(GetCustomerGroupListQuery request, CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllAsync();
            var dtos = list.Select(x => new CustomerGroupDto 
            { 
#pragma warning disable CS8602
                Id = x.Id.ToString(),
                Name = x.Name,
                Description = x.Description,
                DiscountPercentage = x.DiscountPercentage
#pragma warning restore CS8602
            }).ToList();

            return new PagedResult<CustomerGroupDto>(dtos, dtos.Count, request.Filter.Page, request.Filter.PageSize);
        }
    }
}
