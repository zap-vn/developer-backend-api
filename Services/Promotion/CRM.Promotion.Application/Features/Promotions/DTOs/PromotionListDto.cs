using System;

namespace CRM.Promotion.Application.Features.Promotions.DTOs
{
    public class PromotionListDto
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public string discount_type { get; set; } = string.Empty;
        public decimal discount_value { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string status { get; set; } = "Active";
    }
}
