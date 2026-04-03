#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class CategoryDto
    {
        public Guid id { get; set; }
        public Guid? parent_id { get; set; }
        public string? legacy_id { get; set; }
        public string? materialized_path { get; set; }
        public string name { get; set; } = string.Empty;
        public string? slug { get; set; }
        public string? icon_url { get; set; }
        public string? banner_url { get; set; }
        public int sort_order { get; set; }
        public string? meta_title { get; set; }
        public string? meta_description { get; set; }
        public int? status_id { get; set; }
        public bool is_active { get; set; }
        public string? seo_title { get; set; }
        public string? seo_description { get; set; }
        public string[]? channels { get; set; }
        public int item_count { get; set; }
        public List<CategoryDto> children { get; set; } = new();
    }
}
