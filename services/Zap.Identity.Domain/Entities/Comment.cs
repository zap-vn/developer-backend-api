using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Zap.Identity.Domain.Persistence;

namespace Zap.Identity.Domain.Entities;

[BsonIgnoreExtraElements]
public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("_key")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int Key { get; set; }

    [BsonElement("PostId")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string PostId { get; set; } = string.Empty;

    [BsonElement("ParentId")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string? ParentId { get; set; }

    [BsonElement("Content")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Content { get; set; } = string.Empty;

    [BsonElement("AuthorId")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string AuthorId { get; set; } = string.Empty;

    [BsonElement("CreateDate")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string CreateDate { get; set; } = string.Empty;
    
    [BsonElement("UpdateDate")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string UpdateDate { get; set; } = string.Empty;

    [BsonElement("Visible")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int Visible { get; set; } = 1;

    [BsonElement("_rev")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Revision { get; set; } = string.Empty;
}
