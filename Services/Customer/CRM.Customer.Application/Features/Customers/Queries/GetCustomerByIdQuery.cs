using MediatR;
using CRM.Customer.Application.Features.Customers.DTOs;

namespace CRM.Customer.Application.Features.Customers.Queries
{
    public class GetCustomerByIdQuery : IRequest<CustomerDto>
    {
        public string Id { get; set; }

        public GetCustomerByIdQuery(string id)
        {
            Id = id;
        }
    }
}
