using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("location", Schema = "pos")]
    public class Warehouse
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Column("tenant_id")]
        public Guid? tenant_id { get; set; }

        [Column("node_id")]
        public Guid? node_id { get; set; }
        
        [Column("legacy_id")]
        public string? legacy_id { get; set; }
        
        [Column("name")]
        public string name { get; set; } = string.Empty;

        [Column("status_id")]
        public int? status_id { get; set; }

        [Column("is_active")]
        public bool? is_active { get; set; }
        
        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }

        [ForeignKey("status_id")]
        public StatusItem? status { get; set; }
    }
}
