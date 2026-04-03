using MediatR;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.BuildingBlocks.Models;

namespace CRM.Promotion.Application.Features.Promotions.Queries
{
    public class GetPromotionListQuery : IRequest<PagedResult<PromotionListDto>>
    {
        public FilterDTOs Filter { get; set; } = new();
        public bool? IsActive { get; set; }
        public string? PromotionType { get; set; }
        public System.DateTime? ValidAt { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
