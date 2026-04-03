using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.BuildingBlocks.Models;
using CRM.Payment.Application.Features.Payments.DTOs;
using CRM.Payment.Application.Features.Payments.Queries;
using CRM.Payment.Infrastructure.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Payment.Infrastructure.Persistence.Queries
{
    public class GetPaymentListQueryHandler : IRequestHandler<GetPaymentListQuery, PagedResult<PaymentListDto>>
    {
        private readonly PostgresDbContext _context;

        public GetPaymentListQueryHandler(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<PaymentListDto>> Handle(GetPaymentListQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Transactions.AsNoTracking();

            if (request.ProviderId.HasValue)
            {
                query = query.Where(t => t.provider_id == request.ProviderId.Value);
            }

            if (request.StatusId.HasValue)
            {
                query = query.Where(t => t.status_id == request.StatusId.Value);
            }

            if (!string.IsNullOrEmpty(request.TransactionRef))
            {
                query = query.Where(t => t.provider_tx_id.Contains(request.TransactionRef) || t.order_number.Contains(request.TransactionRef));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var results = await query
                .OrderByDescending(t => t.processed_at)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new PaymentListDto
                {
                    id = t.id,
                    order_number = t.order_number,
                    amount_captured = t.amount_captured,
                    payment_method = t.payment_method,
                    provider_tx_id = t.provider_tx_id,
                    status = t.status_id == 1 ? "Completed" : "Failed",
                    processed_at = t.processed_at
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<PaymentListDto>(results, totalCount, request.Page, request.PageSize);
        }
    }
}
