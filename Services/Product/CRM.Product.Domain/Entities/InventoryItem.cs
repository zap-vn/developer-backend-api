using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("inventory_item", Schema = "logistics")]
    public class InventoryItem
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        
        [Column("product_variant_id")]
        public Guid product_variant_id { get; set; }
        
        [Column("location_id")]
        public Guid location_id { get; set; }
        
        [Column("qty_on_hand")]
        public decimal qty_on_hand { get; set; }

        public ProductVariant? Variant { get; set; }
        public Location? Location { get; set; }
    }
}
