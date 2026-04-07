using MediatR;
using System;

namespace CRM.Promotion.Application.Features.Promotions.Commands
{
    public class UpdatePromotionCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty; // Injected from route

        public string? name { get; set; }
        public string? short_name { get; set; }
        public string? description { get; set; }
        public string? terms_conditions { get; set; }
        public string? color_hex { get; set; }
        public string? reference_id { get; set; }

        public int? promotion_class_id { get; set; }
        public int? discount_type_id { get; set; }
        public int? apply_to_id { get; set; }
        public int? campaign_type_id { get; set; }
        public int? min_requirement_type_id { get; set; }

        public bool? is_automatic { get; set; }
        public bool? is_scan_qr_table { get; set; }
        public bool? is_visible_pos { get; set; }
        public bool? is_banner_default { get; set; }
        public bool? is_exclude_mode { get; set; }

        public decimal? discount_value { get; set; }
        public decimal? maximum_discount_amount { get; set; }
        public bool? is_discount_limit { get; set; }
        public bool? only_apply_once_per_order { get; set; }
        public decimal? min_requirement_value { get; set; }

        public bool? is_all_locations { get; set; }
        public bool? is_all_payment_methods { get; set; }

        public int? status_id { get; set; }
    }
}
