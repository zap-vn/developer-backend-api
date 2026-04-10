using System;
using System.Collections.Generic;

namespace CRM.Customer.Domain.Entities
{
    public class LoyaltyTier
    {
        public Guid id { get; set; }
        public string tier_name { get; set; } = string.Empty;
        public string? tier_code { get; set; }
        public int? priority_level { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool is_active { get; set; } = true;
        public DateTime? created_at { get; set; } = DateTime.UtcNow;
        public DateTime? updated_at { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<CustomerEntity> customers { get; set; } = new List<CustomerEntity>();
    }
}
