using MediatR;
using CRM.Product.Application.Features.Products.DTOs;
using CRM.BuildingBlocks.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using CRM.Product.Domain.Interfaces;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, PagedResult<ProductDto>>
    {
        private readonly IProductRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetProductListQueryHandler(IProductRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<ProductDto>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            var tenantIdString = _currentUserService.UserGuid;
            Guid? tenantId = null;
            if (Guid.TryParse(tenantIdString, out var guid)) tenantId = guid;

            Guid? categoryId = null;
            if (Guid.TryParse(req.Filters?.CategoryId, out var catGuid)) categoryId = catGuid;

            Guid? warehouseId = null;
            if (Guid.TryParse(req.Filters?.WarehouseId, out var whGuid)) warehouseId = whGuid;

            int localeId = req.Filters?.LocaleId ?? 2;

            var (items, total) = await _repository.GetPagedAsync(
                req.Page,
                req.PageSize,
                tenantId,
                req.Search,
                req.Filters?.StatusId,
                categoryId,
                warehouseId,
                localeId);

            var dtos = items.Select(v =>
            {
                var p = v.product;
                var primaryMedia = v.media.FirstOrDefault(m => m.is_primary) ?? v.media.FirstOrDefault();
                var primaryCategory = p?.category_mappings.FirstOrDefault(cm => cm.is_primary) ?? p?.category_mappings.FirstOrDefault();
                
                var statusItem = p?.status;
                var translation = statusItem?.translations.FirstOrDefault(t => t.locale_id == localeId);
                var statusText = translation?.name ?? statusItem?.status_code;

                // Inventory summing for the specific variant
                var inventoryItems = v.inventory_items;
                if (warehouseId.HasValue)
                    inventoryItems = inventoryItems.Where(i => i.warehouse_id == warehouseId).ToList();
                
                var stockQty = inventoryItems.Sum(i => i.qty_on_hand);
                var firstInv = inventoryItems.FirstOrDefault();

                // Pricing: Check for override first
                var locationPrice = v.location_pricing.FirstOrDefault(lp => !warehouseId.HasValue || lp.warehouse_id == warehouseId);
                var price = locationPrice?.sale_price_override ?? v.sale_price ?? v.base_price;

                return new ProductDto
                {
                    id = v.id,
                    tenant_id = p?.tenant_id,
                    product_type = p?.product_type ?? 1,
                    status_id = p?.status_id,
                    status_text = statusText,
                    media_url = primaryMedia?.media_url,
                    variant_name = v.variant_name ?? p?.name ?? "",
                    category_id = primaryCategory?.category_id,
                    category_name = primaryCategory?.category?.name,
                    sku_code = v.sku_code,
                    barcode = v.barcode,
                    sale_price = price,
                    qty_on_hand = stockQty,
                    uom_id = v.bom_headers.FirstOrDefault()?.uom_id,
                    warehouse_id = firstInv?.warehouse_id,
                    location_name = firstInv?.Warehouse?.name,
                    created_at = p?.created_at ?? DateTime.UtcNow,
                    updated_at = p?.updated_at
                };
            }).ToList();

            return new PagedResult<ProductDto>(dtos, total, req.Page, req.PageSize);
        }
    }
}
