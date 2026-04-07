using System;

namespace CRM.Customer.Application.Features.Customers.DTOs
{
    public class CustomerListDto
    {
        public Guid id { get; set; }
        public string? legacy_id { get; set; }
        public string full_name { get; set; } = string.Empty;
        public string? phone_number { get; set; }
        public string? email { get; set; }
        public decimal current_points_balance { get; set; }
        public decimal total_spent_amount { get; set; }
        public int? tier_id { get; set; }
        public string? tier_name { get; set; }
        public int status_id { get; set; }
    }
}
