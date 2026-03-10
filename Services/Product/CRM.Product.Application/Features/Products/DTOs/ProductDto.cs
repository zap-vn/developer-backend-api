using System;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class ProductDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("_id")]
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
