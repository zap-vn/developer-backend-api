using System;
using System.Collections.Generic;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class ProductVariantDto
    {
        public Guid id { get; set; }
        public string? sku_code { get; set; }
        public string? barcode { get; set; }
        public string? variant_name { get; set; }
        public decimal? base_price { get; set; }
        public decimal? sale_price { get; set; }
        public decimal? cost_price { get; set; }
        public bool is_active { get; set; }
        public string? unit_of_measure { get; set; }
        public decimal? weight_grams { get; set; }
    }

    public class ProductDto
    {
        public Guid id { get; set; }
        public Guid? tenant_id { get; set; }
        public Guid? brand_id { get; set; }
        public string? legacy_id { get; set; }
        public string product_type { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string? short_description { get; set; }
        public string? long_description_html { get; set; }
        public int? status_id { get; set; }
        public bool is_featured { get; set; }
        public List<ProductVariantDto> variants { get; set; } = new List<ProductVariantDto>();
    }
}
