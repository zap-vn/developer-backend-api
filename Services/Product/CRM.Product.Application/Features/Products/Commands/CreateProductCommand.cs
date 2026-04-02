using MediatR;
using System;
using System.Collections.Generic;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<string>
    {
        public Guid TenantId { get; set; }
        public Guid? BrandId { get; set; }
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescriptionHtml { get; set; }
        public int StatusId { get; set; } = 2201; // Active
        public int ProductType { get; set; } = 1; // 1: PHYSICAL
        public bool IsFeatured { get; set; }

        public List<CreateProductVariantCommand> Variants { get; set; } = new List<CreateProductVariantCommand>();
    }

    public class CreateProductVariantCommand
    {
        public string? VariantName { get; set; }
        public string? SkuCode { get; set; }
        public string? Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? Uom { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
