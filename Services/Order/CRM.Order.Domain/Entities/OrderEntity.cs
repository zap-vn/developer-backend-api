using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using CRM.BuildingBlocks;

namespace CRM.Order.Domain.Entities
{
    public class OrderEntity : BaseEntity
    {
        [BsonElement("_key")]
        public long Key { get; set; } 

        [BsonElement("Code")]
        public string OrderCode { get; set; } = string.Empty;

        [BsonElement("Cart_id")]
        public string? CartId { get; set; }

        [BsonElement("DiningOptionId")]
        public int DiningOptionId { get; set; }

        [BsonElement("CustomerGuestGuid")]
        public string? CustomerGuestGuid { get; set; }

        [BsonElement("LocationGuid")]
        public string? LocationGuid { get; set; }

        [BsonElement("DeviceGuid")]
        public string? DeviceGuid { get; set; }

        [BsonElement("AssignToLocationGuid")]
        public string? AssignToLocationGuid { get; set; }

        [BsonElement("TotalAmount")]
        public decimal TotalAmount { get; set; }

        [BsonElement("OrderStatusId")]
        public int OrderStatusId { get; set; }

        [BsonElement("PaymentStatusId")]
        public int PaymentStatusId { get; set; }

        public string Status => OrderStatusId.ToString();
        
        public List<OrderItemSnapshot> Items { get; set; } = new List<OrderItemSnapshot>();
    }

    public class OrderItemSnapshot
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
