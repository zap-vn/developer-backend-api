using System.Text.Json.Serialization;

namespace CRM.Order.Application.Orders.Queries.GetOrderList
{
    public class OrderListDto
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("_key")]
        public long Key { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        [System.Text.Json.Serialization.JsonPropertyName("Cart_id")]
        public string CartId { get; set; } = string.Empty;
        public int OrderStatusId { get; set; }
        public int PaymentStatusId { get; set; }
        public int DiningOptionId { get; set; }
        public string LocationGuid { get; set; } = string.Empty;
        public string DeviceGuid { get; set; } = string.Empty;
        public string UserGuid { get; set; } = string.Empty;
        public string CustomerGuestGuid { get; set; } = string.Empty;
        [JsonPropertyName("AssignToLocationGuid")]
        public string AssignToLocationGuid { get; set; } = string.Empty;

        [JsonPropertyName("AssignToLocationName")]
        public string AssignToLocationName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        
        // From OrderDetail
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Grandtotal { get; set; }
        public object? Shipping { get; set; }
        public List<object>? PaymentList { get; set; }

        public string Status { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
    }
}
