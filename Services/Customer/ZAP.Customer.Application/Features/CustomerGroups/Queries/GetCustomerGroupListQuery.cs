using MediatR;
using ZAP.Customer.Application.Features.CustomerGroups.DTOs;
using ZAP.BuildingBlocks.Models;

namespace ZAP.Customer.Application.Features.CustomerGroups.Queries
{
    public class GetCustomerGroupListQuery : IRequest<PagedResult<CustomerGroupDto>>
    {
        public CustomerGroupFilterDto Filter { get; set; } = new();
    }
}
