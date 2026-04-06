using MediatR;
using CRM.Product.Application.Features.Warehouses.DTOs;
using CRM.BuildingBlocks.Models;
using CRM.Product.Domain.Interfaces;
using CRM.BuildingBlocks.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace CRM.Product.Application.Features.Warehouses.Queries
{
    public class GetWarehouseListQueryHandler : IRequestHandler<GetWarehouseListQuery, PagedResult<WarehouseDto>>
    {
        private readonly IWarehouseRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetWarehouseListQueryHandler(IWarehouseRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<WarehouseDto>> Handle(GetWarehouseListQuery request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            var tenantIdString = _currentUserService.UserGuid;
            Guid? tenantId = null;
            if (Guid.TryParse(tenantIdString, out var guid)) tenantId = guid;

            var items = await _repository.GetPagedAsync(
                req.page_index, 
                req.page_size, 
                tenantId, 
                req.search);

            var total = await _repository.GetTotalCountAsync(tenantId, req.search);

            var dtos = items.Select(x => new WarehouseDto
            {
                id = x.id,
                tenant_id = x.tenant_id,
                legacy_id = x.legacy_id,
                name = x.name,
                // nickname = x.nickname,
                // description = x.description,
                // warehouse_type = x.warehouse_type,
                is_active = true,
                status_id = x.status_id,
                status_text = x.status?.code,
                // address_line1 = x.address_line1,
                // address_line2 = x.address_line2,
                // city = x.city,
                // province = x.province,
                // postal_code = x.postal_code,
                // email = x.email,
                // phone = x.phone,
                // website = x.website,
                // x_link = x.x_link,
                // instagram_link = x.instagram_link,
                // facebook_link = x.facebook_link,
                // logo_url = x.logo_url,
                // brand_color = x.brand_color,
                // timezone = x.timezone,
                // business_hours = x.business_hours,
                // preferred_language = x.preferred_language,
                // match_location_id = x.match_location_id,
                // address_json = x.address_json,
                // manager_id = x.manager_id,
                created_at = x.created_at,
                updated_at = x.updated_at
            }).ToList();

            return new PagedResult<WarehouseDto>(dtos, total, req.page_index, req.page_size);
        }
    }
}
