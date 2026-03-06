using System;
using ZAP.BuildingBlocks.Models;

namespace ZAP.Promotion.Application.Features.Promotions.DTOs
{
    public class PromotionFilterDto : PaginationDto
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
