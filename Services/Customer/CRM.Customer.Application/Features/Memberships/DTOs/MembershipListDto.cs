using System;

namespace CRM.Customer.Application.Features.Memberships.DTOs
{
    public class MembershipListDto
    {
        public Guid id { get; set; }
        public string customer_name { get; set; } = string.Empty;
        public string plan_name { get; set; } = string.Empty;
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public bool auto_renew { get; set; }
        public int status_id { get; set; }
    }
}
