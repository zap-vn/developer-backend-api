using System;

namespace CRM.Promotion.Application.Features.Promotions.DTOs
{
    public class PromotionListDto
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? short_name { get; set; }

        public int promotion_class_id { get; set; }  // 3001 DISCOUNT | 3002 COMBO | 3003 BUY_X_GET_Y
        public int? discount_type_id { get; set; }   // 3101 PERCENTAGE | 3102 FIXED_AMOUNT | 3103 SPECIFIC_PRICE | 3104 FREE_ITEM
        public int? campaign_type_id { get; set; }   // 3601 STANDARD | 3602 LOYALTY_REWARD | 3603 GIFT_VOUCHER | 3604 FLASH_SALE

        public decimal discount_value { get; set; }
        public bool is_automatic { get; set; }
        public bool is_visible_pos { get; set; }

        public int status_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
