using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.Promotion.Application.Features.Promotions.Queries;
using CRM.Promotion.Infrastructure.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Promotion.Infrastructure.Persistence.Queries
{
    public class GetPromotionByIdQueryHandler : IRequestHandler<GetPromotionByIdQuery, PromotionDto?>
    {
        private readonly PostgresDbContext _context;

        public GetPromotionByIdQueryHandler(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<PromotionDto?> Handle(GetPromotionByIdQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Id, out var guid)) return null;

            var x = await _context.Promotions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.id == guid, cancellationToken);

            if (x == null) return null;

            return new PromotionDto
            {
                id = x.id,
                tenant_id = x.tenant_id,
                name = x.name,
                discount_value = x.discount_value,
                status_id = x.is_active ? 1 : 0
            };
        }
    }
}
