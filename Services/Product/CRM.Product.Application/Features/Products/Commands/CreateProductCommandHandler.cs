using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
    {
        private readonly IProductRepository _repository;

        public CreateProductCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var entity = new CRM.Product.Domain.Entities.Product
            {
                id = Guid.NewGuid(),
                tenant_id = request.TenantId,
                brand_id = request.BrandId,
                name = request.Name ?? string.Empty,
                short_description = request.ShortDescription,
                long_description_html = request.LongDescriptionHtml,
                status_id = request.StatusId,
                product_type = request.ProductType,
                is_featured = request.IsFeatured,
                variants = request.Variants.Select(v => new ProductVariant
                {
                    id = Guid.NewGuid(),
                    tenant_id = request.TenantId,
                    variant_name = v.VariantName ?? string.Empty,
                    sku_code = v.SkuCode ?? string.Empty,
                    barcode = v.Barcode,
                    sale_price = v.Price,
                    base_price = v.OriginalPrice,
                    stock_quantity = v.StockQuantity,
                    unit_of_measure = v.Uom,
                    is_active = v.IsActive
                }).ToList()
            };

            await _repository.CreateAsync(entity);
            return entity.id.ToString();
        }
    }
}
