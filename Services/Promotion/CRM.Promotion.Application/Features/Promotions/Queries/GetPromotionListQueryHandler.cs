using MediatR;
using CRM.Promotion.Application.Features.Promotions.DTOs;
using CRM.BuildingBlocks.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CRM.Promotion.Domain.Interfaces;

namespace CRM.Promotion.Application.Features.Promotions.Queries
{
    public class GetPromotionListQueryHandler : IRequestHandler<GetPromotionListQuery, PagedResult<PromotionListDto>>
    {
        private readonly IPromotionRepository _repository;

        public GetPromotionListQueryHandler(IPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<PromotionListDto>> Handle(GetPromotionListQuery request, CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllAsync();
            var dtos = list.Select(x => new PromotionListDto
            {
                id = Guid.TryParse(x.Id, out var gid) ? gid : Guid.Empty,
                name = x.Title ?? x.Code,
                discount_type = x.DiscountType,
                discount_value = x.DiscountValue,
                start_date = x.StartDate,
                end_date = x.EndDate,
                status = "Active"
            }).ToList();

            return new PagedResult<PromotionListDto>(dtos, dtos.Count, request.Filter.PageIndex, request.Filter.PageSize);
        }
    }
}
