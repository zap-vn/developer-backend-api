using MediatR;
using CRM.Product.Application.Features.Warehouses.DTOs;
using CRM.Product.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Warehouses.Queries
{
    public class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, WarehouseDto?>
    {
        private readonly IWarehouseRepository _repository;

        public GetWarehouseByIdQueryHandler(IWarehouseRepository repository)
        {
            _repository = repository;
        }

        public async Task<WarehouseDto?> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
        {
            var x = await _repository.GetByIdAsync(request.Id);
            if (x == null) return null;

            return new WarehouseDto
            {
                id = x.id,
                tenant_id = x.tenant_id,
                node_id = x.node_id,
                legacy_id = x.legacy_id,
                name = x.name,
                status_id = x.status_id,
                is_active = x.is_active,
                created_at = x.created_at,
                updated_at = x.updated_at,
                slug = x.slug,
                business_name = x.business_name,
                description = x.description,
                location_type_id = x.location_type_id,
                location_type_text = x.location_type != null ? $"{x.location_type.label_en} ({x.location_type.label_vi})" : null,
                address_line_1 = x.address_line_1,
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
                operating_hours = x.operating_hours,
                transfer_account = x.transfer_account,
                transfer_tag = x.transfer_tag,
                parent_location_id = x.parent_location_id
            };
        }
    }
}
