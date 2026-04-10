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

            Guid? LocationId = null;
            if (Guid.TryParse(req.Filters?.LocationId, out var whGuid)) LocationId = whGuid;

            int localeId = req.Filters?.LocaleId ?? _currentUserService.LocaleId;

            // USE THE NEW PRODUCT-CENTRIC REPOSITORY METHOD
            var (products, total) = await _repository.GetPagedProductsAsync(
                req.Page,
                req.PageSize,
                tenantId,
                req.Search,
                req.Filters?.StatusId,
                categoryId,
                LocationId,
                localeId,
                req.Filters?.ProductTypeId,
                req.Sort?.Field ?? "created_at",
                req.Sort?.Descending ?? true);

            var dtos = products.Select(p => 
            {
                var primaryCategory = p.category_mappings.FirstOrDefault(cm => cm.is_primary) ?? p.category_mappings.FirstOrDefault();
                
                var statusItem = p.status;
                var translation = statusItem?.translations?.FirstOrDefault(t => t.locale_id == localeId);
                var statusText = translation?.name ?? statusItem?.code ?? "";

                // Map variants hierarchical
                var productVariants = p.variants.Select(v => {
                    var primaryMedia = v.media.FirstOrDefault(m => m.is_primary) ?? v.media.FirstOrDefault();
                    
                    var inventoryItems = v.inventory_items;
                    if (LocationId.HasValue)
                        inventoryItems = inventoryItems.Where(i => i.location_id == LocationId).ToList();

                    var locationPrice = v.location_pricing.FirstOrDefault(lp => !LocationId.HasValue || lp.location_id == LocationId);
                    var price = locationPrice?.sale_price_override ?? v.sale_price ?? v.base_price;

                    return new ProductVariantDto {
                        id = v.id,
                        serial_id = v.serial_id,
                        sku_code = v.sku_code,
                        barcode = v.barcode,
                        variant_name = v.variant_name ?? "",
                        base_price = v.base_price,
                        sale_price = price,
                        cost_price = v.cost_price,
                        is_active = true,
                        media_url = primaryMedia?.media_url,
                        qty_on_hand = inventoryItems.Sum(i => i.qty_on_hand),
                        location_count = inventoryItems.Select(i => i.location_id).Distinct().Count()
                    };
                }).ToList();

                // Lead variant info for the Product root row display
                var leadVariant = productVariants.FirstOrDefault();

                return new ProductDto
                {
                    id = p.id,
                    serial_id = p.serial_id,
                    product_id = p.id,
                    name = p.name,
                    tenant_id = p.tenant_id,
                    product_type_id = p.product_type_id,
                    product_type_text = p.product_type?.translations?.FirstOrDefault(t => t.locale_id == localeId)?.name ?? p.product_type?.code ?? "",
                    status_id = p.status_id,
                    status_name = statusText,
                    status_code = statusItem?.code ?? "",
                    category_id = primaryCategory?.category_id,
                    category_name = primaryCategory?.category?.name,
                    media_url = leadVariant?.media_url,
                    variant_name = leadVariant?.variant_name,
                    sku_code = leadVariant?.sku_code,
                    barcode = leadVariant?.barcode,
                    sale_price = leadVariant?.sale_price,
                    qty_on_hand = productVariants.Sum(v => v.qty_on_hand),
                    created_at = p.created_at,
                    updated_at = p.updated_at,
                    variants = productVariants
                };
            }).ToList();

            return new PagedResult<ProductDto>(dtos, total, req.Page, req.PageSize);
        }
    }
}
