using MediatR;
using System;
using System.Collections.Generic;

namespace CRM.Product.Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescriptionHtml { get; set; }
        public int StatusId { get; set; }
        public bool IsFeatured { get; set; }
        public int ProductTypeId { get; set; } = 1; // 1: PHYSICAL

        public List<UpdateProductVariantCommand> Variants { get; set; } = new List<UpdateProductVariantCommand>();
    }

    public class UpdateProductVariantCommand
    {
        public Guid? Id { get; set; }
        public string? VariantName { get; set; }
        public string? SkuCode { get; set; }
        public string? Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? Uom { get; set; }
        public bool IsActive { get; set; }
    }
}
