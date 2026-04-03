using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Authentication.Domain.Entities
{
    [Table("tenant_node", Schema = "identity")]
    public class TenantNode
    {
        public Guid id { get; set; }
        public Guid? parent_id { get; set; }
        public string node_code { get; set; } = string.Empty;
        public string? legacy_id { get; set; }
        public int tier_level { get; set; }
        public string name { get; set; } = string.Empty;
        public string slug { get; set; } = string.Empty;
        public int? locale_id { get; set; }
        public int? status_id { get; set; }
        public Guid? avatar_id { get; set; }
        public Guid? banner_id { get; set; }
        public string? address_line_1 { get; set; }
        public int? country_id { get; set; }
        public int? province_id { get; set; }
        public int? district_id { get; set; }
        public int? ward_id { get; set; }
        public string? zipcode { get; set; }
        public string? timezone { get; set; } = "Asia/Ho_Chi_Minh";
        public DateTime? created_at { get; set; } = DateTime.UtcNow;
        public DateTime? updated_at { get; set; } = DateTime.UtcNow;
    }
}
