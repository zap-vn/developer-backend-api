using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("product_category_map", Schema = "catalog")]
    public class ProductCategoryMap
    {
        [Column("product_id")]
        public Guid product_id { get; set; }
        
        [Column("category_id")]
        public Guid category_id { get; set; }
        
        [Column("is_primary")]
        public bool is_primary { get; set; } = false;

        // Navigation
        public Product? product { get; set; }
        public Category? category { get; set; }
    }
}
