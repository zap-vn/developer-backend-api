using MediatR;
using ZAP.Promotion.Application.Features.Promotions.DTOs;
using ZAP.BuildingBlocks.Models;

namespace ZAP.Promotion.Application.Features.Promotions.Queries
{
    public class GetPromotionListQuery : IRequest<PagedResult<PromotionDto>>
    {
        public PromotionFilterDto Filter { get; set; } = new();
    }
}
