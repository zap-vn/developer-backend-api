using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Zap.Identity.Domain.Persistence;

namespace Zap.Identity.Domain.Entities;

[BsonIgnoreExtraElements]
public class Category
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("Title")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Title { get; set; } = string.Empty;

    [BsonElement("ReferenceId")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string ReferenceId { get; set; } = string.Empty;

    [BsonElement("Description")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Description { get; set; } = string.Empty;

    [BsonElement("SubCategory")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int SubCategory { get; set; }

    [BsonElement("ParentCategoryId")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int ParentCategoryId { get; set; }

    [BsonElement("Level")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int Level { get; set; }

    [BsonElement("Color")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Color { get; set; } = "#ccc";

    [BsonElement("Acronymn")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Acronymn { get; set; } = string.Empty;

    [BsonElement("BusinessTypeId")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int BusinessTypeId { get; set; }

    [BsonElement("Handle")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Handle { get; set; } = string.Empty;

    [BsonElement("Ansi")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Ansi { get; set; } = string.Empty;

    [BsonElement("Version")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Version { get; set; } = string.Empty;

    [BsonElement("SeoTitle")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string SeoTitle { get; set; } = string.Empty;

    [BsonElement("SeoDescription")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string SeoDescription { get; set; } = string.Empty;

    [BsonElement("CreateDate")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string CreateDate { get; set; } = string.Empty;

    [BsonElement("UserGuid")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string UserGuid { get; set; } = string.Empty;

    [BsonElement("CategoryId")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int CategoryId { get; set; }

    [BsonElement("OrderNo")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int OrderNo { get; set; }

    [BsonElement("Visible")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int Visible { get; set; }

    [BsonElement("AdminInsert")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string AdminInsert { get; set; } = string.Empty;

    [BsonElement("_key")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int Key { get; set; }
}
