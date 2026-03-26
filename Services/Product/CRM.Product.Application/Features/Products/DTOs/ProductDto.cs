using System;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class ProductDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("cate_name")]
        public string CateName { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("merchant_id")]
        public string MerchantId { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("price")]
        public decimal Price { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
