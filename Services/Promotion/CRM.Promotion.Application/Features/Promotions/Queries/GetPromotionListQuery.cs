using MediatR;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.BuildingBlocks.Models;

namespace CRM.Promotion.Application.Features.Promotions.Queries
{
    public class GetPromotionListQuery : IRequest<PagedResult<PromotionListDto>>
    {
        public FilterDTOs Filter { get; set; } = new();

        // Registry-based filters (INT IDs from platform.status_item)
        public int? PromotionClassId { get; set; }  // 3001 DISCOUNT | 3002 COMBO | 3003 BUY_X_GET_Y
        public int? DiscountTypeId { get; set; }    // 3101 PERCENTAGE | 3102 FIXED_AMOUNT | 3103 SPECIFIC_PRICE | 3104 FREE_ITEM
        public int? CampaignTypeId { get; set; }    // 3601 STANDARD | 3602 LOYALTY_REWARD | 3603 GIFT_VOUCHER | 3604 FLASH_SALE
        public int? StatusId { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
