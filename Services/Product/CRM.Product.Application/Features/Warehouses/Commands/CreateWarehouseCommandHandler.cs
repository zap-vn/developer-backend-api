using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;

namespace CRM.Product.Application.Features.Warehouses.Commands
{
    public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Guid>
    {
        private readonly IWarehouseRepository _repository;

        public CreateWarehouseCommandHandler(IWarehouseRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var location = new Warehouse
            {
                id = Guid.NewGuid(),
                tenant_id = request.tenant_id,
                node_id = request.node_id,
                legacy_id = request.legacy_id,
                name = request.name,
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
