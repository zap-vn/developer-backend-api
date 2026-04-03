using System;

namespace CRM.Order.Application.Features.Orders.DTOs
{
    public class TransactionListDto
    {
        public Guid id { get; set; }
        public string order_number { get; set; } = string.Empty;
        public decimal total_amount { get; set; }
        public string status_text { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public string customer_name { get; set; } = "Guest";
    }
}
