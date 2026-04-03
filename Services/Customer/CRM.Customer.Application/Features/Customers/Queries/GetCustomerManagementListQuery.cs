using MediatR;
using CRM.BuildingBlocks.Models;
using CRM.Customer.Application.Features.Customers.DTOs;
using System;

namespace CRM.Customer.Application.Features.Customers.Queries
{
    public class GetCustomerManagementListQuery : IRequest<PagedResult<CustomerListDto>>
    {
        public Guid? TenantId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int? TierId { get; set; }
        public int? StatusId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
