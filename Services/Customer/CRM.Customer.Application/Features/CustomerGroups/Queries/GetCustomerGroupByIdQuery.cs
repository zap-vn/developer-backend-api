using MediatR;
using CRM.Customer.Application.Features.CustomerGroups.DTOs;

namespace CRM.Customer.Application.Features.CustomerGroups.Queries
{
    public class GetCustomerGroupByIdQuery : IRequest<CustomerGroupDto>
    {
        public string Id { get; set; }

        public GetCustomerGroupByIdQuery(string id)
        {
            Id = id;
        }
    }
}
