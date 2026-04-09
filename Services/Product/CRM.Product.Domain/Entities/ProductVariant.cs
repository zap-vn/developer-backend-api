using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    public class ProductVariant
    {
        public Guid id { get; set; } = Guid.NewGuid();

        [Column("serial_id")]
        public int? serial_id { get; set; }

        public Guid? product_id { get; set; }
        public string? sku_code { get; set; } // Mã SKU
        public string? barcode { get; set; }
        public string? variant_name { get; set; }
        public decimal? base_price { get; set; } // Giá gốc 
        public decimal? sale_price { get; set; } // Giá bán 
        public decimal? cost_price { get; set; } // Giá vốn 
        public bool is_default { get; set; } = false; // SKU mặc định
        public decimal? weight_grams { get; set; }
        public decimal? length_mm { get; set; }
        public decimal? width_mm { get; set; }
        public decimal? height_mm { get; set; }
        public string? attributes { get; set; } // JSON chứa các thuộc tính như Màu sắc, Kích thước
        public int? uom_id { get; set; }
        
        // Navigation
        public UomItem? uom { get; set; }
        public Product? product { get; set; }
        public ICollection<ProductMedia> media { get; set; } = new List<ProductMedia>();
        public ICollection<ProductLocationPricing> location_pricing { get; set; } = new List<ProductLocationPricing>();
        public ICollection<InventoryItem> inventory_items { get; set; } = new List<InventoryItem>();
        public ICollection<BomHeader> bom_headers { get; set; } = new List<BomHeader>();
    }
}

