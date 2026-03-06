using MediatR;
using ZAP.Promotion.Application.Features.Promotions.DTOs;

namespace ZAP.Promotion.Application.Features.Promotions.Queries
{
    public class GetPromotionByIdQuery : IRequest<PromotionDto>
    {
        public string Id { get; set; }

        public GetPromotionByIdQuery(string id)
        {
            Id = id;
        }
    }
}
