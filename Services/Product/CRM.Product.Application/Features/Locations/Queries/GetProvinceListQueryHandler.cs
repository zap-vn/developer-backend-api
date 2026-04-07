using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CRM.Product.Application.Features.Locations.DTOs;
using CRM.Product.Domain.Interfaces;

namespace CRM.Product.Application.Features.Locations.Queries
{
    public class GetProvinceListQueryHandler : IRequestHandler<GetProvinceListQuery, List<ProvinceDto>>
    {
        private readonly ILocationRepository _repository;

        public GetProvinceListQueryHandler(ILocationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProvinceDto>> Handle(GetProvinceListQuery request, CancellationToken cancellationToken)
        {
            var items = await _repository.GetProvincesAsync(request.LocaleId);

            return items.Select(p =>
            {
                var translation = p.translations?.FirstOrDefault(t => t.locale_id == request.LocaleId);
                return new ProvinceDto
                {
                    province_code = p.code,
                    city_name = translation?.name ?? string.Empty
                };
            }).ToList();
        }
    }
}
