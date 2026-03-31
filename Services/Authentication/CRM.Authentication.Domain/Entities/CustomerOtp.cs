using System;

namespace CRM.Authentication.Domain.Entities
{
    public class CustomerOtp
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string customer_id { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string otp_code { get; set; } = string.Empty;
        public string purpose { get; set; } = string.Empty; 
        public DateTime expired_at { get; set; }
        public DateTime? verified_at { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}
