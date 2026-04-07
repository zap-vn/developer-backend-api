using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("product_location_pricing", Schema = "catalog")]
    public class ProductLocationPricing
    {
        [Column("product_variant_id")]
        public Guid product_variant_id { get; set; }
        
        [Column("location_id")]
        public Guid location_id { get; set; }
        
        [Column("sale_price_override")]
        public decimal? sale_price_override { get; set; }
        public bool is_active { get; set; } = true;

        // Navigation
        public ProductVariant? variant { get; set; }
    }
}
