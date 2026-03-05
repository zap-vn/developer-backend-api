using System;
using ZAP.Product.Domain.Entities;
using ZAP.BuildingBlocks.Extensions;
using System.Linq;

namespace ZAP.Product.Api
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }

        public static ProductResponseDto FromEntity(ProductEntity entity, string? lang = null)
        {
            var translation = entity.GetTranslation(lang);
            
            return new ProductResponseDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = translation?.Name ?? entity.Name,
                Description = translation?.Description ?? entity.Description,
                Price = entity.Price,
                Category = entity.Category,
                ImageUrl = entity.ImageUrl,
                Stock = entity.Stock
            };
        }
    }
}
