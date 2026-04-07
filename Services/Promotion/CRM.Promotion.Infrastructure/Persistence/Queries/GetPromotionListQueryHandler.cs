using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.BuildingBlocks.Models;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.Promotion.Application.Features.Promotions.Queries;
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

            if (!string.IsNullOrEmpty(request.Filter.Keyword))
                query = query.Where(p => p.name.Contains(request.Filter.Keyword) ||
                                         (p.short_name != null && p.short_name.Contains(request.Filter.Keyword)));

            if (request.PromotionClassId.HasValue)
                query = query.Where(p => p.promotion_class_id == request.PromotionClassId.Value);

            if (request.DiscountTypeId.HasValue)
                query = query.Where(p => p.discount_type_id == request.DiscountTypeId.Value);

            if (request.CampaignTypeId.HasValue)
                query = query.Where(p => p.campaign_type_id == request.CampaignTypeId.Value);

            if (request.StatusId.HasValue)
                query = query.Where(p => p.status_id == request.StatusId.Value);

            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 10;
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PromotionListDto
                {
                    id = p.id,
                    name = p.name,
                    short_name = p.short_name,
                    promotion_class_id = p.promotion_class_id,
                    discount_type_id = p.discount_type_id,
                    campaign_type_id = p.campaign_type_id,
                    discount_value = p.discount_value,
                    is_automatic = p.is_automatic,
                    is_visible_pos = p.is_visible_pos,
                    status_id = p.status_id,
                    created_at = p.CreatedAt,
                    updated_at = p.UpdatedAt ?? p.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<PromotionListDto>(items, totalCount, page, pageSize);
        }
    }
}
