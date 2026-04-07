using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.Promotion.Infrastructure.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Promotion.Application.Features.Promotions.Queries
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
                legacy_id = x.legacy_id,
                name = x.name,
                short_name = x.short_name,
                description = x.description,
                terms_conditions = x.terms_conditions,
                color_hex = x.color_hex,
                reference_id = x.reference_id,
                promotion_class_id = x.promotion_class_id,
                discount_type_id = x.discount_type_id,
                apply_to_id = x.apply_to_id,
                campaign_type_id = x.campaign_type_id,
                min_requirement_type_id = x.min_requirement_type_id,
                is_automatic = x.is_automatic,
                is_scan_qr_table = x.is_scan_qr_table,
                is_visible_pos = x.is_visible_pos,
                is_banner_default = x.is_banner_default,
                is_exclude_mode = x.is_exclude_mode,
                discount_value = x.discount_value,
                maximum_discount_amount = x.maximum_discount_amount,
                is_discount_limit = x.is_discount_limit,
                only_apply_once_per_order = x.only_apply_once_per_order,
                min_requirement_value = x.min_requirement_value,
                is_all_locations = x.is_all_locations,
                is_all_payment_methods = x.is_all_payment_methods,
                status_id = x.status_id,
                created_at = x.created_at,
                updated_at = x.updated_at
            };
        }
    }
}
