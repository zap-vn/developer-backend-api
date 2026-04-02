using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("inventory_item", Schema = "logistics")]
    public class InventoryItem
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();
        
        [Column("product_variant_id")]
        public Guid product_variant_id { get; set; }
        
        [Column("warehouse_id")]
        public Guid warehouse_id { get; set; }
        
        [Column("qty_on_hand")]
        public decimal qty_on_hand { get; set; }

        public ProductVariant? Variant { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}
