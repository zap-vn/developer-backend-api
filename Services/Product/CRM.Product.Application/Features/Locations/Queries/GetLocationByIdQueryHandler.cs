using MediatR;
using CRM.Product.Application.Features.Locations.DTOs;
using CRM.Product.Domain.Interfaces;
using CRM.BuildingBlocks.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Locations.Queries
{
    public class GetLocationByIdQueryHandler : IRequestHandler<GetLocationByIdQuery, LocationDto?>
    {
        private readonly ILocationRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        
        public GetLocationByIdQueryHandler(ILocationRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<LocationDto?> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
        {
            var x = await _repository.GetByIdAsync(request.Id);
            if (x == null) return null;

            return new LocationDto
            {
                id = x.id,
                tenant_id = x.tenant_id,
                serial_id = x.serial_id,
                serial_number = x.serial_number,
                legacy_id = x.legacy_id,
                name = x.name,
                location_code = x.location_code,
                status_id = x.status_id,
                status_code = x.status?.code,
                status_name = x.status != null
                    ? (x.status.translations?.FirstOrDefault(t => t.locale_id == _currentUserService.LocaleId)?.name ?? 
                       $"{x.status.translations?.FirstOrDefault(t => t.locale_id == 2)?.name} ({x.status.translations?.FirstOrDefault(t => t.locale_id == 1)?.name})")
                    : null,
                is_active = x.is_active,
                created_at = x.created_at,
                updated_at = x.updated_at,
                slug = x.slug,
                business_name = x.business_name,
                description = x.description,
                location_type_id = x.location_type_id,
                location_type_text = x.location_type != null 
                    ? (x.location_type.translations?.FirstOrDefault(t => t.locale_id == _currentUserService.LocaleId)?.name ?? 
                       $"{x.location_type.translations?.FirstOrDefault(t => t.locale_id == 2)?.name} ({x.location_type.translations?.FirstOrDefault(t => t.locale_id == 1)?.name})")
                    : null,

                address_line_1 = x.address_line_1,
                address_line_2 = x.address_line_2,
                city = x.city,
                state = x.state,
                country_id = x.country_id,
                province_id = x.province_id,
                district_id = x.district_id,
                ward_id = x.ward_id,
                zipcode = x.zipcode,
                phone_number = x.phone_number,
                email = x.email,
                website = x.website,
                twitter = x.twitter,
                instagram = x.instagram,
                facebook = x.facebook,
                logo_url = x.logo_url,
                cover_image_url = x.cover_image_url,
                brand_color = x.brand_color,
                timezone = x.timezone,
                latitude = x.latitude,
                longitude = x.longitude,
                operating_hours = x.operating_hours,
                transfer_account = x.transfer_account,
                transfer_tag = x.transfer_tag,
                parent_location_id = x.parent_location_id
            };
        }
    }
}
