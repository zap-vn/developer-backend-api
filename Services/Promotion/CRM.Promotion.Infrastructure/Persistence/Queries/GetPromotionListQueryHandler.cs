using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.BuildingBlocks.Models;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.Promotion.Application.Features.Promotions.Queries;
using CRM.Promotion.Domain.Entities;
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
            var req = request.Request;
            var query = _context.Promotions.AsNoTracking().AsQueryable();

            // Search by name
            if (!string.IsNullOrEmpty(req.Search))
                query = query.Where(p =>
                    p.name.Contains(req.Search) ||
                    (p.short_name != null && p.short_name.Contains(req.Search)));

            // Filters
            if (req.Filters.StatusId.HasValue)
                query = query.Where(p => p.status_id == req.Filters.StatusId.Value);

            if (req.Filters.DiscountTypeId.HasValue)
                query = query.Where(p => p.discount_type_id == req.Filters.DiscountTypeId.Value);

            if (req.Filters.IsAllLocations.HasValue)
                query = query.Where(p => p.is_all_locations == req.Filters.IsAllLocations.Value);

            if (req.Filters.IsAutomatic.HasValue)
                query = query.Where(p => p.is_automatic == req.Filters.IsAutomatic.Value);

            // member_level_id: no column yet — TODO when promotion_member_level table is added

            var page = req.PageIndex > 0 ? req.PageIndex : 1;
            var pageSize = req.PageSize > 0 ? req.PageSize : 10;
            var totalCount = await query.CountAsync(cancellationToken);

            // Dynamic sort
            IOrderedQueryable<PromotionEntity> ordered = req.Sort.Field switch
            {
                "discount_value" => req.Sort.Descending
                    ? query.OrderByDescending(p => p.discount_value)
                    : query.OrderBy(p => p.discount_value),
                "status_id" => req.Sort.Descending
                    ? query.OrderByDescending(p => p.status_id)
                    : query.OrderBy(p => p.status_id),
                _ => req.Sort.Descending
                    ? query.OrderByDescending(p => p.name)
                    : query.OrderBy(p => p.name),
            };

            var items = await ordered
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
