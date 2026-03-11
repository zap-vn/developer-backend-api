using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CRM.Authentication.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class CustomerOtp
    {
        [BsonId]
        public string _id { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("Customer_id")]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("Email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("Phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("OtpCode")]
        public string OtpCode { get; set; } = string.Empty;

        [BsonElement("Purpose")]
        public string Purpose { get; set; } = string.Empty; // login, register, reset_password

        [BsonElement("ExpiredAt")]
        public DateTime ExpiredAt { get; set; }

        [BsonElement("VerifiedAt")]
        public DateTime? VerifiedAt { get; set; }

        [BsonElement("AttemptCount")]
        public int AttemptCount { get; set; } = 0;

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
