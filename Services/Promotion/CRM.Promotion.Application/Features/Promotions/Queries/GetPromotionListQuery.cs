using MediatR;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.BuildingBlocks.Models;

namespace CRM.Promotion.Application.Features.Promotions.Queries
{
    public class GetPromotionListQuery : IRequest<PagedResult<PromotionDto>>
    {
        public PromotionFilterDto Filter { get; set; } = new();
    }
}
