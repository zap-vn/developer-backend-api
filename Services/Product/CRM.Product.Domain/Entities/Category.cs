using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("category", Schema = "catalog")]
    public class Category
    {
        public Guid id { get; set; }
        public Guid? parent_id { get; set; }
        public string name { get; set; } = string.Empty;
        public bool is_active { get; set; } = true;
        public string? icon_url { get; set; }
        public string? materialized_path { get; set; }
        public string? seo_title { get; set; }
        public string? seo_description { get; set; }
        public string[]? channels { get; set; }

        // Navigation properties
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();
    }
}
