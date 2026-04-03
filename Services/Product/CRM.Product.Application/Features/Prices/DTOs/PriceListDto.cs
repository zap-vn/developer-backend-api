using System;

namespace CRM.Product.Application.Features.Prices.DTOs
{
    public class PriceListDto
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public string sku_code { get; set; } = string.Empty;
        public string? barcode { get; set; }
        public decimal base_price { get; set; }
        public decimal? price_override { get; set; }
        public decimal final_price => price_override ?? base_price;
        public string? category_name { get; set; }
    }
}
