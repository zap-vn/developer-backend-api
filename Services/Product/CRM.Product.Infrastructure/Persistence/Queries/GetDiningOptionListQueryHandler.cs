using MediatR;
using CRM.Product.Application.Features.DiningOptions.DTOs;
using CRM.Product.Application.Features.DiningOptions.Queries;
using CRM.Product.Domain.Interfaces;
using CRM.BuildingBlocks.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Infrastructure.Persistence.Queries
{
    public class GetDiningOptionListQueryHandler : IRequestHandler<GetDiningOptionListQuery, IEnumerable<DiningOptionDto>>
    {
        private readonly IDiningOptionRepository _repository;
        private readonly ILocalizationService _localizationService;

        public GetDiningOptionListQueryHandler(IDiningOptionRepository repository, ILocalizationService localizationService)
        {
            _repository = repository;
            _localizationService = localizationService;
        }

        public async Task<IEnumerable<DiningOptionDto>> Handle(GetDiningOptionListQuery request, CancellationToken cancellationToken)
        {
            var localeId = _localizationService.GetCurrentLocaleId();
            var entities = await _repository.GetAllAsync(localeId);

            return entities.Select(e => new DiningOptionDto
            {
                id = e.id,
                code = e.code,
                display_name = e.DisplayName ?? e.code,
                used_in_locations = e.UsedInLocations,
                sort_order = e.sort_order,
                is_active = e.is_active
            });
        }
    }
}
