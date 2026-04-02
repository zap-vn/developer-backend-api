using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("product_media", Schema = "catalog")]
    public class ProductMedia
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();
        
        [Column("product_variant_id")]
        public Guid product_variant_id { get; set; }
        
        [Column("media_url")]
        public string media_url { get; set; } = string.Empty;
        
        [Column("is_primary")]
        public bool is_primary { get; set; } = false;
        
        [Column("sort_order")]
        public int sort_order { get; set; } = 0;

        // Navigation
        public ProductVariant? variant { get; set; }
    }
}
