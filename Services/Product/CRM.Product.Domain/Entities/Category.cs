using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("category", Schema = "catalog")]
    public class Category
    {
        [Column("id")]
        public Guid id { get; set; }

        [Column("tenant_id")]
        public Guid? tenant_id { get; set; }

        [Column("parent_id")]
        public Guid? parent_id { get; set; }

        [Column("serial_id")]
        public int? serial_id { get; set; }

        [Column("serial_number")]
        public string? serial_number { get; set; }

        [Column("category_code")]
        public string? category_code { get; set; }

        [Column("legacy_id")]
        public string? legacy_id { get; set; }

        [Column("materialized_path")]
        public string? materialized_path { get; set; }

        [Column("name")]
        public string name { get; set; } = string.Empty;

        [Column("slug")]
        public string? slug { get; set; }

        [Column("icon_url")]
        public string? icon_url { get; set; }

        [Column("banner_url")]
        public string? banner_url { get; set; }

        [Column("sort_order")]
        public int? sort_order { get; set; }

        [NotMapped]
        public string? meta_title { get; set; }

        [NotMapped]
        public string? meta_description { get; set; }

        [NotMapped]
        public string? meta_keywords { get; set; }

        [Column("canonical_url")]
        public string? canonical_url { get; set; }

        [Column("status_id")]
        public int? status_id { get; set; }

        [Column("is_active")]
        public bool is_active { get; set; } = true;

        [NotMapped]
        public string? display_initial { get; set; }

        [Column("applicable_channels")]
        public string[]? applicable_channels { get; set; }

        [NotMapped]
        public string? seo_title { get; set; }

        [NotMapped]
        public string? seo_description { get; set; }

        // Navigation properties (Renamed to avoid EF conventions)
        [ForeignKey("parent_id")]
        public Category? parent_category { get; set; }

        [ForeignKey("status_id")]
        public StatusItem? status { get; set; }
        
        public ICollection<Category> sub_categories { get; set; } = new List<Category>();
        
        public ICollection<ProductCategoryMap> product_mappings { get; set; } = new List<ProductCategoryMap>();
    }
}
