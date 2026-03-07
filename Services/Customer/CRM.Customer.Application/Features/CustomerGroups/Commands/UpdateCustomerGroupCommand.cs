using MediatR;
using System;

namespace CRM.Customer.Application.Features.CustomerGroups.Commands
{
    public class UpdateCustomerGroupCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty; // Injected from route
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
    }
}
