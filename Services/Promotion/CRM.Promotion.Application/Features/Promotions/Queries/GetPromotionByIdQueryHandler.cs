using MediatR;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using System.Threading;
using System.Threading.Tasks;
using CRM.Promotion.Domain.Interfaces;

namespace CRM.Promotion.Application.Features.Promotions.Queries
{
    public class GetPromotionByIdQueryHandler : IRequestHandler<GetPromotionByIdQuery, PromotionDto>
    {
        private readonly IPromotionRepository _repository;

        public GetPromotionByIdQueryHandler(IPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<PromotionDto> Handle(GetPromotionByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return null;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return null;

            return new PromotionDto 
            { 
                Id = entity.Id,
                Code = entity.Code,
                Title = entity.Title,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                DiscountValue = entity.DiscountValue,
                DiscountType = entity.DiscountType
            };
        }
    }
}
