using MediatR;
using CRM.Customer.Application.Features.Customers.DTOs;
using CRM.BuildingBlocks.Models;

namespace CRM.Customer.Application.Features.Customers.Queries
{
    public class GetCustomerListQuery : IRequest<PagedResult<CustomerDto>>
    {
        public FilterDTOs? Filter { get; set; }
    }
}
