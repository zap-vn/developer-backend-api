using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.Product.Application.Features.Locations.Commands
{
    public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, Guid>
    {
        private readonly ILocationRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public CreateLocationCommandHandler(ILocationRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            // tenant_id from JWT token, fallback to request if not present (for testing/internal calls)
            Guid? tenantId = Guid.TryParse(_currentUserService.UserGuid, out var tGuid) ? tGuid : request.tenant_id;

            var location = new Location
            {
                id = Guid.NewGuid(),
                tenant_id = tenantId,
                // node_id = request.node_id,
                legacy_id = request.legacy_id,
                name = request.name,
                location_code = request.location_code,
                status_id = request.status_id ?? 30001,
                is_active = request.is_active ?? true,
                slug = request.slug,
                business_name = request.business_name,
                description = request.description,
                location_type_id = request.location_type_id,
                address_line_1 = request.address_line_1,
                city = request.city,
                state = request.state,
                country_id = request.country_id,
                province_id = request.province_id,
                district_id = request.district_id,
                ward_id = request.ward_id,
                zipcode = request.zipcode,
                phone_number = request.phone_number,
                email = request.email,
                website = request.website,
                twitter = request.twitter,
                instagram = request.instagram,
                facebook = request.facebook,
                logo_url = request.logo_url,
                cover_image_url = request.cover_image_url,
                brand_color = request.brand_color,
                timezone = request.timezone,
                operating_hours = request.operating_hours,
                transfer_account = request.transfer_account,
                transfer_tag = request.transfer_tag,
                parent_location_id = request.parent_location_id,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            await _repository.CreateAsync(location);

            // Create child Store record linked to this location
            var store = new Store
            {
                id = Guid.NewGuid(),
                location_id = location.id,
                legacy_id = request.legacy_id,
                store_code = ("STR-" + (request.slug ?? request.name.ToLower().Replace(" ", "-"))).ToUpper(),
                store_name = request.business_name ?? request.name,
                address_line_1 = request.address_line_1,
                phone_number = request.phone_number,
                email = request.email,
                country_id = request.country_id,
                province_id = request.province_id,
                district_id = request.district_id,
                ward_id = request.ward_id,
                timezone = request.timezone,
                status_id = request.status_id ?? 30001,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            await _repository.CreateStoreAsync(store);

            return location.id;
        }
    }
}
