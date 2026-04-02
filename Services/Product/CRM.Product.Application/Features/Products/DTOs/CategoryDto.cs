using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class CategoryDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("parent_id")]
        public Guid? ParentId { get; set; }


        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("icon_url")]
        public string? IconUrl { get; set; }

        [JsonPropertyName("materialized_path")]
        public string? MaterializedPath { get; set; }

        [JsonPropertyName("seo_title")]
        public string? SeoTitle { get; set; }

        [JsonPropertyName("channels")]
        public string[]? Channels { get; set; }

        [JsonPropertyName("item_count")]
        public int ItemCount { get; set; }

        [JsonPropertyName("children")]
        public List<CategoryDto> Children { get; set; } = new();
    }
}
