using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zap.Identity.Domain.Entities;

[BsonIgnoreExtraElements]
public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("_key")]
    public int Key { get; set; }

    [BsonElement("CustomerId")]
    public int CustomerId { get; set; }

    [BsonElement("CustomerCode")]
    public string CustomerCode { get; set; } = string.Empty;

    [BsonElement("Email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("Password")]
    public string Password { get; set; } = string.Empty;

    [BsonElement("FirstName")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("LastName")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("BusinessName")]
    public string BusinessName { get; set; } = string.Empty;

    [BsonElement("MerchantName")]
    public string MerchantName { get; set; } = string.Empty;

    [BsonElement("Phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("CustomerStatusId")]
    public int CustomerStatusId { get; set; }

    [BsonElement("Visible")]
    public int Visible { get; set; }

    [BsonElement("CreateDate")]
    public string CreateDate { get; set; } = string.Empty;

    [BsonElement("StartedDate")]
    public string StartedDate { get; set; } = string.Empty;

    [BsonElement("TimeZoneId")]
    public string TimeZoneId { get; set; } = string.Empty;

    [BsonElement("TimeZoneDisplayName")]
    public string TimeZoneDisplayName { get; set; } = string.Empty;

    [BsonElement("Country")]
    public int Country { get; set; }

    [BsonElement("PublicKey")]
    public string PublicKey { get; set; } = string.Empty;
}
