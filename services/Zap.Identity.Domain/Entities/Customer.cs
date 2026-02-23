using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Zap.Identity.Domain.Persistence;

namespace Zap.Identity.Domain.Entities;

[BsonIgnoreExtraElements]
public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("_key")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int Key { get; set; }

    [BsonElement("CustomerId")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int CustomerId { get; set; }

    [BsonElement("CustomerCode")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string CustomerCode { get; set; } = string.Empty;

    [BsonElement("Email")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Email { get; set; } = string.Empty;

    [BsonElement("Password")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Password { get; set; } = string.Empty;

    [BsonElement("FirstName")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("LastName")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("BusinessName")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string BusinessName { get; set; } = string.Empty;

    [BsonElement("MerchantName")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string MerchantName { get; set; } = string.Empty;

    [BsonElement("Phone")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("CustomerStatusId")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int CustomerStatusId { get; set; }

    [BsonElement("Visible")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int Visible { get; set; }

    [BsonElement("CreateDate")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string CreateDate { get; set; } = string.Empty;

    [BsonElement("StartedDate")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string StartedDate { get; set; } = string.Empty;

    [BsonElement("TimeZoneId")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string TimeZoneId { get; set; } = string.Empty;

    [BsonElement("TimeZoneDisplayName")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string TimeZoneDisplayName { get; set; } = string.Empty;

    [BsonElement("Country")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int Country { get; set; }

    [BsonElement("ProjectName")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string ProjectName { get; set; } = string.Empty;

    [BsonElement("BusinessType")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string BusinessType { get; set; } = string.Empty;

    [BsonElement("UseAiContentGeneration")]
    public bool UseAiContentGeneration { get; set; }

    [BsonElement("Language")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Language { get; set; } = string.Empty;

    [BsonElement("DateFormat")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string DateFormat { get; set; } = string.Empty;

    [BsonElement("TimeFormat")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string TimeFormat { get; set; } = string.Empty;

    [BsonElement("ReferenceAssets")]
    public List<string> ReferenceAssets { get; set; } = new();

    [BsonElement("PublicKey")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string PublicKey { get; set; } = string.Empty;

    [BsonElement("Url")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Url { get; set; } = string.Empty;

    [BsonElement("_rev")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Revision { get; set; } = string.Empty;
}
