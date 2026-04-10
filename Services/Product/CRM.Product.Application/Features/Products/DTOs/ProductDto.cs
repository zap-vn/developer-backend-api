using System;
using System.Collections.Generic;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class ProductVariantDto
    {
        public Guid id { get; set; }
        public int? serial_id { get; set; }
        public string? sku_code { get; set; }
        public string? barcode { get; set; }
        public string? variant_name { get; set; }
        public decimal? base_price { get; set; }
        public decimal? sale_price { get; set; }
        public decimal? cost_price { get; set; }
        public bool is_active { get; set; }
        public decimal? weight_grams { get; set; }
        public int location_count { get; set; }
        public string? media_url { get; set; }
        public decimal? qty_on_hand { get; set; }
    }


    public class ProductDto
    {
        public Guid id { get; set; }
        public int? serial_id { get; set; }
        public Guid? tenant_id { get; set; }
        public Guid? brand_id { get; set; }
        public string? legacy_id { get; set; }
        public int product_type_id { get; set; }
        public string? product_type_text { get; set; }
        public string name { get; set; } = string.Empty;
        public string? short_description { get; set; }
        public string? long_description_html { get; set; }
        public int? status_id { get; set; }
        public bool is_featured { get; set; }
        public List<ProductVariantDto> variants { get; set; } = new List<ProductVariantDto>();

        // Extra fields for List
        public string? media_url { get; set; }
        public string? variant_name { get; set; }
        public Guid? category_id { get; set; }
        public string? category_name { get; set; }
        public string? sku_code { get; set; }
        public string? barcode { get; set; }
        public decimal? sale_price { get; set; }
        public decimal? qty_on_hand { get; set; }
        public string? location_name { get; set; }
        public Guid? location_id { get; set; }
        public string? status_name { get; set; }
        public string? status_code { get; set; }
        public Guid? product_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

}
