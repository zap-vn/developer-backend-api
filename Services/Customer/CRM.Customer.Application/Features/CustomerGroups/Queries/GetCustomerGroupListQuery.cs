using MediatR;
using CRM.Customer.Application.Features.CustomerGroups.DTOs;
using CRM.BuildingBlocks.Models;

namespace CRM.Customer.Application.Features.CustomerGroups.Queries
{
    public class GetCustomerGroupListQuery : IRequest<PagedResult<CustomerGroupDto>>
    {
        public FilterDTOs Filter { get; set; } = new();
    }
}
