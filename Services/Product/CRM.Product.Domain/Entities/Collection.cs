using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("collection", Schema = "catalog")]
    public class Collection
    {
        [Key]
        [Column("id")]
        public Guid id { get; set; }

        [Column("tenant_id")]
        public Guid? tenant_id { get; set; }

        [Column("name")]
        public string name { get; set; } = string.Empty;

        [Column("slug")]
        public string slug { get; set; } = string.Empty;

        [Column("description")]
        public string? description_html { get; set; }

        [Column("image_url")]
        public string? banner_url { get; set; }

        [Column("status_id")]
        public int status_id { get; set; } = 1;

        [Column("sort_order")]
        public int sort_order { get; set; } = 0;

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }

        public ICollection<CollectionItem> items { get; set; } = new List<CollectionItem>();
    }
}
