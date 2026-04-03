using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("bom_header", Schema = "catalog")]
    public class BomHeader
    {
        public Guid id { get; set; }
        public Guid? tenant_id { get; set; }
        public Guid? product_variant_id { get; set; }
        public Guid? uom_id { get; set; }
        public bool is_active { get; set; } = true;

        // Navigation
        public ProductVariant? variant { get; set; }
    }
}
