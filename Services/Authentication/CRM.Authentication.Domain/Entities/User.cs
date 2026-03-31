using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Authentication.Domain.Entities
{
    [Table("user", Schema = "identity")]
    public class User
    {
        // === PostgreSQL mapped columns (matching DB exactly) ===
        public Guid id { get; set; } = Guid.NewGuid();
        public Guid? tenant_id { get; set; }
        public string? legacy_id { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
        public string full_name { get; set; } = string.Empty;
        public int? status_id { get; set; }
        public DateTime? created_at { get; set; } = DateTime.UtcNow;
        public DateTime? updated_at { get; set; } = DateTime.UtcNow;
        
        // Removed all [NotMapped] legacy properties as requested
    }
}
