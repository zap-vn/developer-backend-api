using MediatR;
using CRM.BuildingBlocks.Models;
using CRM.Order.Application.Features.Orders.DTOs;
using System;

namespace CRM.Order.Application.Features.Orders.Queries
{
    public class GetTransactionManagementListQuery : IRequest<PagedResult<TransactionListDto>>
    {
        public Guid? TenantId { get; set; }
        public string? OrderNumber { get; set; }
        public int? StatusId { get; set; }
        public string? Channel { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
