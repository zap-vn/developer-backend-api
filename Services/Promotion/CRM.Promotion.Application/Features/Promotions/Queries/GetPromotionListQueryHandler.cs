using MediatR;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.BuildingBlocks.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CRM.Promotion.Domain.Interfaces;

namespace CRM.Promotion.Application.Features.Promotions.Queries
{
    public class GetPromotionListQueryHandler : IRequestHandler<GetPromotionListQuery, PagedResult<PromotionDto>>
    {
        private readonly IPromotionRepository _repository;

        public GetPromotionListQueryHandler(IPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<PromotionDto>> Handle(GetPromotionListQuery request, CancellationToken cancellationToken)
        {
            // Simple mock implementation for template setup
            // In a real scenario, this would apply the filters map to DTO
            var list = await _repository.GetAllAsync();
            var dtos = list.Select(x => new PromotionDto 
            { 
                Id = x.Id.ToString(),
                Code = x.Code,
                Title = x.Title,
                Description = x.Description,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                DiscountValue = x.DiscountValue,
                DiscountType = x.DiscountType
            }).ToList();

            return new PagedResult<PromotionDto>(dtos, dtos.Count, request.Filter.Page, request.Filter.PageSize);
        }
    }
}
