using System;

namespace CRM.Authentication.Domain.Entities
{
    public class SystemError
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string? message { get; set; }
        public string? detail { get; set; }
        public string? source { get; set; }
        public string? user_id { get; set; }
        public string? merchant_name { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}
