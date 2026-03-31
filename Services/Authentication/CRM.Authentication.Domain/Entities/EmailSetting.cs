using System;

namespace CRM.Authentication.Domain.Entities
{
    public class EmailSetting
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string? customer_guid { get; set; }
        public string? smtp_host { get; set; }
        public int? smtp_port { get; set; }
        public string? smtp_user { get; set; }
        public string? smtp_pass { get; set; }
        public string? from_email { get; set; }
        public string? from_name { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}
