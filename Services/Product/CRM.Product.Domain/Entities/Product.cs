using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    public class Product
    {
        public Guid id { get; set; } = Guid.NewGuid();
        [Column("serial_id")]
        public int? serial_id { get; set; }
        public Guid? tenant_id { get; set; }
        
        public Guid? brand_id { get; set; }
        public string? legacy_id { get; set; } // Dùng để đối chiếu với hệ thống cũ
        public int product_type_id { get; set; } = 1; // 1: PHYSICAL, 2: SERVICE, 3: DIGITAL, 4: BUNDLE
        public string name { get; set; } = string.Empty;
        public string? short_description { get; set; }
        public string? long_description_html { get; set; }
        public string? search_vector { get; set; }
        public int priority_score { get; set; } = 0;
        public int? status_id { get; set; } // 2201 - Active, Draft, Archived
        public bool is_featured { get; set; } = false;
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime? updated_at { get; set; }

        // Navigation
        public StatusItem? status { get; set; }
        public ProductTypeItem? product_type { get; set; }
        public ICollection<ProductVariant> variants { get; set; } = new List<ProductVariant>();
        public ICollection<ProductCategoryMap> category_mappings { get; set; } = new List<ProductCategoryMap>();
    }
}
