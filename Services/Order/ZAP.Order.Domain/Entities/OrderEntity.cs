using System;
using ZAP.BuildingBlocks;

namespace ZAP.Order.Domain.Entities
{
    public class OrderEntity : BaseEntity
    {
        public string OrderCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        
        // Snapshot dữ liệu sản phẩm để tối ưu Query hàng triệu dòng
        public List<OrderItemSnapshot> Items { get; set; } = new List<OrderItemSnapshot>();
    }

    public class OrderItemSnapshot
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty; // Lưu trực tiếp tên sản phẩm (đã dịch) tại thời điểm mua
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
