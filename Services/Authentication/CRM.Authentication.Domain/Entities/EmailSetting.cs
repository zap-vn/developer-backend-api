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
        
        // Vietguy SMS Settings
        public string? account_name { get; set; }
        public string? passcode { get; set; }
        public string? send_id { get; set; }
        public string? base_url_sms { get; set; }
        public string? api_endpoint { get; set; }
        public string? body_sms { get; set; }

        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}

