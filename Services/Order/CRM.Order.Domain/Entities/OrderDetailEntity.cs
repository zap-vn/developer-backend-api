using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace CRM.Order.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class OrderDetailEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Order_id")]
        public string OrderId { get; set; } = string.Empty;

        [BsonElement("Order")]
        public OrderSummaryInfo OrderSummary { get; set; } = new OrderSummaryInfo();

        [BsonElement("Shipping")]
        public object? Shipping { get; set; }

        [BsonElement("PaymentList")]
        public List<object>? PaymentList { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class OrderSummaryInfo
    {
        [BsonElement("Subtotal")]
        public decimal Subtotal { get; set; }

        [BsonElement("Discount")]
        public decimal Discount { get; set; }

        [BsonElement("Grandtotal")]
        public decimal Grandtotal { get; set; }

        [BsonElement("Shipping")]
        public decimal ShippingValue { get; set; }
    }
}
