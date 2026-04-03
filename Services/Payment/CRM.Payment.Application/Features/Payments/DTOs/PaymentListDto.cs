using System;

namespace CRM.Payment.Application.Features.Payments.DTOs
{
    public class PaymentListDto
    {
        public Guid id { get; set; }
        public string order_number { get; set; } = string.Empty;
        public decimal amount_captured { get; set; }
        public string payment_method { get; set; } = string.Empty;
        public string? provider_tx_id { get; set; }
        public string status { get; set; } = "Completed";
        public DateTime processed_at { get; set; }
    }
}
