using MediatR;
using System;

namespace CRM.Promotion.Application.Features.Promotions.Commands
{
    public class CreatePromotionCommand : IRequest<Guid>
    {
        public Guid? tenant_id { get; set; }
        public string? legacy_id { get; set; }

        public string name { get; set; } = string.Empty;
        public string? short_name { get; set; }
        public string? description { get; set; }
        public string? terms_conditions { get; set; }
        public string? color_hex { get; set; }
        public string? reference_id { get; set; }

        // Engine Controls — INT registry IDs (platform.status_item)
        public int promotion_class_id { get; set; }       // 3001 DISCOUNT | 3002 COMBO | 3003 BUY_X_GET_Y
        public int? discount_type_id { get; set; }        // 3101 PERCENTAGE | 3102 FIXED_AMOUNT | 3103 SPECIFIC_PRICE | 3104 FREE_ITEM
        public int? apply_to_id { get; set; }             // 3201 ORDER | 3202 ITEM | 3203 SHIPPING | 3204 MODIFIER
        public int? campaign_type_id { get; set; }        // 3601 STANDARD | 3602 LOYALTY_REWARD | 3603 GIFT_VOUCHER | 3604 FLASH_SALE
        public int? min_requirement_type_id { get; set; } // 3301 NONE | 3302 AMOUNT | 3303 QUANTITY

        public bool? is_automatic { get; set; } = true;
        public bool? is_scan_qr_table { get; set; } = false;
        public bool? is_visible_pos { get; set; } = true;
        public bool? is_banner_default { get; set; } = false;
        public bool? is_exclude_mode { get; set; } = false;

        public decimal discount_value { get; set; } = 0;
        public decimal? maximum_discount_amount { get; set; }
        public bool? is_discount_limit { get; set; } = false;
        public bool? only_apply_once_per_order { get; set; } = false;
        public decimal min_requirement_value { get; set; } = 0;

        public bool? is_all_locations { get; set; } = true;
        public bool? is_all_payment_methods { get; set; } = true;

        public int? status_id { get; set; } = 7001;
    }
}
