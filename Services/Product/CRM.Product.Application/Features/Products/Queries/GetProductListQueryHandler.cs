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

            var (items, total) = await _repository.GetPagedAsync(
                req.Page, 
                req.PageSize, 
                tenantId, 
                req.Search, 
                req.Filters?.Status);

            var dtos = items.Select(x => new ProductDto 
            { 
                id = x.id,
                tenant_id = x.tenant_id,
                brand_id = x.brand_id,
                legacy_id = x.legacy_id,
                product_type = x.product_type,
                name = x.name,
                short_description = x.short_description,
                long_description_html = x.long_description_html,
                status_id = x.status_id,
                is_featured = x.is_featured,
                variants = x.variants.Select(v => new ProductVariantDto
                {
                    id = v.id,
                    sku_code = v.sku_code,
                    barcode = v.barcode,
                    variant_name = v.variant_name,
                    base_price = v.base_price,
                    sale_price = v.sale_price,
                    cost_price = v.cost_price,
                    is_active = v.is_active,
                    unit_of_measure = v.unit_of_measure,
                    weight_grams = v.weight_grams
                }).ToList()
            }).ToList();

            return new PagedResult<ProductDto>(dtos, total, req.Page, req.PageSize);
        }
    }
}
