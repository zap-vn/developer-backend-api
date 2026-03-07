using MediatR;
using CRM.Promotion.Application.Features.Promotions.DTOs;

namespace CRM.Promotion.Application.Features.Promotions.Queries
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
