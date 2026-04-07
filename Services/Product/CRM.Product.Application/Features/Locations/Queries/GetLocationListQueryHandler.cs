using MediatR;
using CRM.Product.Application.Features.Locations.DTOs;
using CRM.BuildingBlocks.Models;
using CRM.Product.Domain.Interfaces;
using CRM.BuildingBlocks.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace CRM.Product.Application.Features.Locations.Queries
{
    public class GetLocationListQueryHandler : IRequestHandler<GetLocationListQuery, PagedResult<LocationDto>>
    {
        private readonly ILocationRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetLocationListQueryHandler(ILocationRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<LocationDto>> Handle(GetLocationListQuery request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            Guid? tenantId = Guid.TryParse(_currentUserService.UserGuid, out var guid) ? guid : null;

            var filter = new LocationListFilter
            {
                PageIndex      = req.page_index,
                PageSize       = req.page_size,
                Search         = req.search,
                StatusId       = req.filters?.status_id,
                ProvinceId     = req.filters?.province_id,
                SortField      = req.sort?.field,
                SortDescending = req.sort?.descending ?? false,
                LocationTypeIds = req.filters?.location_type_id,
            };

            var items = await _repository.GetPagedAsync(filter);
            var total = await _repository.GetTotalCountAsync(filter);

            var dtos = items.Select(x => new LocationDto
            {
                id                 = x.id,
                tenant_id          = x.tenant_id,
                legacy_id          = x.legacy_id,
                name               = x.name,
                serial_id          = x.serial_id,
                serial_number      = x.serial_number,
                location_code      = x.location_code,
                status_id          = x.status_id,
                status_code        = x.status?.code,
                is_active          = x.is_active,
                created_at         = x.created_at,
                updated_at         = x.updated_at,
                slug               = x.slug,
                business_name      = x.business_name,
                description        = x.description,
                location_type_id   = x.location_type_id,
                location_type_text = x.location_type != null
                    ? (req.locale_id == 2 
                        ? $"{x.location_type.label_vi} ({x.location_type.label_en})"
                        : $"{x.location_type.label_en} ({x.location_type.label_vi})")
                    : null,
                address_line_1     = x.address_line_1,
                city               = x.city,
                state              = x.state,
                country_id         = x.country_id,
                province_id        = x.province_id,
                district_id        = x.district_id,
                ward_id            = x.ward_id,
                zipcode            = x.zipcode,
                phone_number       = x.phone_number,
                email              = x.email,
                website            = x.website,
                twitter            = x.twitter,
                instagram          = x.instagram,
                facebook           = x.facebook,
                logo_url           = x.logo_url,
                cover_image_url    = x.cover_image_url,
                brand_color        = x.brand_color,
                timezone           = x.timezone,
                operating_hours    = x.operating_hours,
                transfer_account   = x.transfer_account,
                transfer_tag       = x.transfer_tag,
                parent_location_id = x.parent_location_id
            }).ToList();

            return new PagedResult<LocationDto>(dtos, total, req.page_index, req.page_size);
        }
    }
}
