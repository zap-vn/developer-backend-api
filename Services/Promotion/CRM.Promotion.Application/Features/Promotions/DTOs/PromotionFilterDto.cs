using System;
using CRM.BuildingBlocks.Models;

namespace CRM.Promotion.Application.Features.Promotions.DTOs
{
    public class PromotionFilterDto : PaginationDto
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
