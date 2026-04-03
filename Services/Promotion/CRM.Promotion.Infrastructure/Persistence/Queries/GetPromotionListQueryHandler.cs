using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.BuildingBlocks.Models;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.Promotion.Application.Features.Promotions.Queries;
using CRM.Promotion.Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Promotion.Infrastructure.Persistence.Queries
{
    public class GetPromotionListQueryHandler : IRequestHandler<GetPromotionListQuery, PagedResult<PromotionListDto>>
    {
        private readonly PostgresDbContext _context;

        public GetPromotionListQueryHandler(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<PromotionListDto>> Handle(GetPromotionListQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Promotions.AsNoTracking();

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.is_active == request.IsActive.Value);
            }

            if (!string.IsNullOrEmpty(request.PromotionType))
            {
                query = query.Where(p => p.discount_type == request.PromotionType);
            }

            if (request.ValidAt.HasValue)
            {
                query = query.Where(p => p.start_date <= request.ValidAt.Value && (!p.end_date.HasValue || p.end_date >= request.ValidAt.Value));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var results = await query
                .OrderByDescending(p => p.start_date)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PromotionListDto
                {
                    id = p.id,
                    name = p.name,
                    discount_type = p.discount_type,
                    discount_value = p.discount_value,
                    start_date = p.start_date,
                    end_date = p.end_date,
                    status = p.is_active ? "Active" : "Disabled"
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<PromotionListDto>(results, totalCount, request.Page, request.PageSize);
        }
    }
}
