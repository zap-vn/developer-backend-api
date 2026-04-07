using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CRM.Promotion.Domain.Interfaces;

namespace CRM.Promotion.Application.Features.Promotions.Commands
{
    public class UpdatePromotionCommandHandler : IRequestHandler<UpdatePromotionCommand, bool>
    {
        private readonly IPromotionRepository _repository;

        public UpdatePromotionCommandHandler(IPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdatePromotionCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return false;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            if (request.name != null) entity.name = request.name;
            if (request.short_name != null) entity.short_name = request.short_name;
            if (request.description != null) entity.description = request.description;
            if (request.terms_conditions != null) entity.terms_conditions = request.terms_conditions;
            if (request.color_hex != null) entity.color_hex = request.color_hex;
            if (request.reference_id != null) entity.reference_id = request.reference_id;

            if (request.promotion_class_id.HasValue) entity.promotion_class_id = request.promotion_class_id.Value;
            if (request.discount_type_id.HasValue) entity.discount_type_id = request.discount_type_id.Value;
            if (request.apply_to_id.HasValue) entity.apply_to_id = request.apply_to_id.Value;
            if (request.campaign_type_id.HasValue) entity.campaign_type_id = request.campaign_type_id.Value;
            if (request.min_requirement_type_id.HasValue) entity.min_requirement_type_id = request.min_requirement_type_id.Value;

            if (request.is_automatic.HasValue) entity.is_automatic = request.is_automatic.Value;
            if (request.is_scan_qr_table.HasValue) entity.is_scan_qr_table = request.is_scan_qr_table.Value;
            if (request.is_visible_pos.HasValue) entity.is_visible_pos = request.is_visible_pos.Value;
            if (request.is_banner_default.HasValue) entity.is_banner_default = request.is_banner_default.Value;
            if (request.is_exclude_mode.HasValue) entity.is_exclude_mode = request.is_exclude_mode.Value;

            if (request.discount_value.HasValue) entity.discount_value = request.discount_value.Value;
            if (request.maximum_discount_amount.HasValue) entity.maximum_discount_amount = request.maximum_discount_amount.Value;
            if (request.is_discount_limit.HasValue) entity.is_discount_limit = request.is_discount_limit.Value;
            if (request.only_apply_once_per_order.HasValue) entity.only_apply_once_per_order = request.only_apply_once_per_order.Value;
            if (request.min_requirement_value.HasValue) entity.min_requirement_value = request.min_requirement_value.Value;

            if (request.is_all_locations.HasValue) entity.is_all_locations = request.is_all_locations.Value;
            if (request.is_all_payment_methods.HasValue) entity.is_all_payment_methods = request.is_all_payment_methods.Value;

            if (request.status_id.HasValue) entity.status_id = request.status_id.Value;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
