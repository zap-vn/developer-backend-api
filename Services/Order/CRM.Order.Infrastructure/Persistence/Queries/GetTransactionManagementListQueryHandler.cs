using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.BuildingBlocks.Models;
using CRM.Order.Application.Features.Orders.DTOs;
using CRM.Order.Application.Features.Orders.Queries;
using CRM.Order.Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Order.Infrastructure.Persistence.Queries
{
    public class GetTransactionManagementListQueryHandler : IRequestHandler<GetTransactionManagementListQuery, PagedResult<TransactionListDto>>
    {
        private readonly PostgresDbContext _context;

        public GetTransactionManagementListQueryHandler(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TransactionListDto>> Handle(GetTransactionManagementListQuery request, CancellationToken cancellationToken)
        {
            // Join commerce.order_header with platform.status_item
            var query = from oh in _context.Orders
                        join si in _context.StatusItems on oh.status_id equals si.id into siGroup
                        from si in siGroup.DefaultIfEmpty()
                        where oh.tenant_id == request.TenantId
                        select new { oh, status_text = si != null ? si.name : "Unknown" };

            if (!string.IsNullOrEmpty(request.OrderNumber))
            {
                query = query.Where(x => x.oh.order_number.Contains(request.OrderNumber));
            }

            if (request.StatusId.HasValue)
            {
                query = query.Where(x => x.oh.status_id == request.StatusId.Value);
            }

            if (!string.IsNullOrEmpty(request.Channel))
            {
                query = query.Where(x => x.oh.channel == request.Channel);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(x => x.oh.created_at >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(x => x.oh.created_at <= request.ToDate.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var results = await query
                .OrderByDescending(x => x.oh.created_at)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new TransactionListDto
                {
                    id = x.oh.id,
                    order_number = x.oh.order_number,
                    total_amount = x.oh.total_amount,
                    status_text = x.status_text,
                    created_at = x.oh.created_at,
                    customer_name = x.oh.customer_name ?? "Guest"
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<TransactionListDto>(results, totalCount, request.Page, request.PageSize);
        }
    }
}
