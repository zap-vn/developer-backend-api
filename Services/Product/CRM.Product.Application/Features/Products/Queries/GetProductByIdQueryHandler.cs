using MediatR;
using CRM.Product.Application.Features.Products.DTOs;
using System.Threading;
using System.Threading.Tasks;
using CRM.Product.Domain.Interfaces;
using System;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _repository;

        public GetProductByIdQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id)) return null;
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return null;

            return new ProductDto 
            { 
                id = entity.id,
                tenant_id = entity.tenant_id,
                brand_id = entity.brand_id,
                legacy_id = entity.legacy_id,
                product_type = entity.product_type,
                name = entity.name,
                short_description = entity.short_description,
                long_description_html = entity.long_description_html,
                status_id = entity.status_id,
                is_featured = entity.is_featured,
                variants = entity.variants.Select(v => new ProductVariantDto
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
            };
        }
    }
}
