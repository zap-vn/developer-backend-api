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
                name = request.Name,
                status_id = 1, // Default to Active
                is_active = true,
                created_at = DateTime.UtcNow
            };

            await _repository.CreateAsync(location);

            var store = new Store
            {
                id = Guid.NewGuid(),
                location_id = location.id,
                store_name = request.Name,
                address_line_1 = request.AddressLine1,
                phone_number = request.Phone,
                email = request.Email,
                timezone = request.Timezone,
                created_at = DateTime.UtcNow,
                
                // Mapped from additional fields in Command
                instagram_link = request.InstagramLink,
                facebook_link = request.FacebookLink,
                logo_url = request.LogoUrl,
                brand_color = request.BrandColor,
                business_hours = request.BusinessHours,
                preferred_language = request.PreferredLanguage
            };

            await _repository.CreateStoreAsync(store);

            return location.id;
        }
    }
}
