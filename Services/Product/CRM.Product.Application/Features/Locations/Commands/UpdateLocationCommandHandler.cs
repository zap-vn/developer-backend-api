using CRM.Product.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Locations.Commands
{
    public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, bool>
    {
        private readonly ILocationRepository _repository;

        public UpdateLocationCommandHandler(ILocationRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            if (request.name != null) entity.name = request.name;
            if (request.legacy_id != null) entity.legacy_id = request.legacy_id;
            if (request.location_code.HasValue) entity.location_code = request.location_code;
            if (request.status_id.HasValue) entity.status_id = request.status_id;
            if (request.is_active.HasValue) entity.is_active = request.is_active;
            if (request.slug != null) entity.slug = request.slug;
            if (request.business_name != null) entity.business_name = request.business_name;
            if (request.description != null) entity.description = request.description;
            if (request.location_type_id.HasValue) entity.location_type_id = request.location_type_id;
            if (request.address_line_1 != null) entity.address_line_1 = request.address_line_1;
            if (request.city != null) entity.city = request.city;
            if (request.state != null) entity.state = request.state;
            if (request.country_id.HasValue) entity.country_id = request.country_id;
            if (request.province_id.HasValue) entity.province_id = request.province_id;
            if (request.district_id.HasValue) entity.district_id = request.district_id;
            if (request.ward_id.HasValue) entity.ward_id = request.ward_id;
            if (request.zipcode != null) entity.zipcode = request.zipcode;
            if (request.phone_number != null) entity.phone_number = request.phone_number;
            if (request.email != null) entity.email = request.email;
            if (request.website != null) entity.website = request.website;
            if (request.twitter != null) entity.twitter = request.twitter;
            if (request.instagram != null) entity.instagram = request.instagram;
            if (request.facebook != null) entity.facebook = request.facebook;
            if (request.logo_url != null) entity.logo_url = request.logo_url;
            if (request.cover_image_url != null) entity.cover_image_url = request.cover_image_url;
            if (request.brand_color != null) entity.brand_color = request.brand_color;
            if (request.timezone != null) entity.timezone = request.timezone;
            if (request.operating_hours.HasValue) entity.operating_hours = request.operating_hours;
            if (request.transfer_account != null) entity.transfer_account = request.transfer_account;
            if (request.transfer_tag != null) entity.transfer_tag = request.transfer_tag;
            if (request.parent_location_id.HasValue) entity.parent_location_id = request.parent_location_id;

            entity.updated_at = System.DateTime.UtcNow;
            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
