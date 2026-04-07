using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.Promotion.Domain.Entities;
using CRM.Promotion.Domain.Interfaces;

namespace CRM.Promotion.Application.Features.Promotions.Commands
{
    public class CreatePromotionCommandHandler : IRequestHandler<CreatePromotionCommand, Guid>
    {
        private readonly IPromotionRepository _repository;

        public CreatePromotionCommandHandler(IPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreatePromotionCommand request, CancellationToken cancellationToken)
        {
            var newId = Guid.NewGuid();
            var entity = new PromotionEntity
            {
                Id = newId.ToString(),
                tenant_id = request.tenant_id,
                legacy_id = request.legacy_id,
                name = request.name,
                short_name = request.short_name,
                description = request.description,
                terms_conditions = request.terms_conditions,
                color_hex = request.color_hex ?? "#cccccc",
                reference_id = request.reference_id,
                promotion_class_id = request.promotion_class_id,
                discount_type_id = request.discount_type_id,
                apply_to_id = request.apply_to_id,
                campaign_type_id = request.campaign_type_id,
                min_requirement_type_id = request.min_requirement_type_id,
                is_automatic = request.is_automatic ?? true,
                is_scan_qr_table = request.is_scan_qr_table ?? false,
                is_visible_pos = request.is_visible_pos ?? true,
                is_banner_default = request.is_banner_default ?? false,
                is_exclude_mode = request.is_exclude_mode ?? false,
                discount_value = request.discount_value,
                maximum_discount_amount = request.maximum_discount_amount,
                is_discount_limit = request.is_discount_limit ?? false,
                only_apply_once_per_order = request.only_apply_once_per_order ?? false,
                min_requirement_value = request.min_requirement_value,
                is_all_locations = request.is_all_locations ?? true,
                is_all_payment_methods = request.is_all_payment_methods ?? true,
                status_id = request.status_id ?? 7001
            };

            await _repository.CreateAsync(entity);
            return newId;
        }
    }
}
