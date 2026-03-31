using System;

namespace CRM.Authentication.Domain.Entities
{
    public class PasswordResetRequest
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string user_guid { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string method { get; set; } = string.Empty; 
        public string channel { get; set; } = string.Empty; 
        public string otp_hash { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string confirm_token { get; set; } = string.Empty;
        public int attempts { get; set; } = 0;
        public bool is_used { get; set; } = false;
        public DateTime expired_at { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}
