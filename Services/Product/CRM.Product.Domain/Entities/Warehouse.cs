using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Product.Domain.Entities
{
    [Table("warehouse", Schema = "logistics")]
    public class Warehouse
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Column("tenant_id")]
        public Guid? tenant_id { get; set; }
        
        [Column("legacy_id")]
        public string? legacy_id { get; set; }
        
        [Column("name")]
        public string name { get; set; } = string.Empty;
        
        [Column("warehouse_type")]
        public string? warehouse_type { get; set; }
        
        [Column("status_id")]
        public int? status_id { get; set; }
        
        [Column("address_json")]
        public string? address_json { get; set; }
        
        [Column("manager_id")]
        public Guid? manager_id { get; set; }
        
        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }
    }
}
