using System;

namespace CRM.Product.Domain.Entities
{
    public class ProductVariant
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public Guid product_id { get; set; }
        public Guid? tenant_id { get; set; }
        public string? sku_code { get; set; } // Mã SKU
        public string? barcode { get; set; }
        public string? variant_name { get; set; }
        public decimal? base_price { get; set; } // Giá gốc 
        public decimal? sale_price { get; set; } // Giá bán 
        public decimal? cost_price { get; set; } // Giá vốn 
        public int stock_quantity { get; set; } = 0;
        public bool is_active { get; set; } = true; // Cho phép bán hay không
        public string? unit_of_measure { get; set; } // Đơn vị tính (Kg, Cái...)
        public decimal? weight_grams { get; set; }
        public decimal? length_mm { get; set; }
        public decimal? width_mm { get; set; }
        public decimal? height_mm { get; set; }
        public string? attributes { get; set; } // JSON chứa các thuộc tính như Màu sắc, Kích thước

        // Navigation
        public Product? product { get; set; }
    }
}

