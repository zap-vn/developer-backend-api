using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Zap.Identity.Domain.Persistence;

namespace Zap.Identity.Domain.Entities;

[BsonIgnoreExtraElements]
public class Post
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("_key")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int Key { get; set; }

    [BsonElement("Title")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Title { get; set; } = string.Empty;

    [BsonElement("Content")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Content { get; set; } = string.Empty;

    [BsonElement("AuthorId")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string AuthorId { get; set; } = string.Empty;

    [BsonElement("CreateDate")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string CreateDate { get; set; } = string.Empty;

    [BsonElement("Visible")]
    [BsonSerializer(typeof(FlexibleIntSerializer))]
    public int Visible { get; set; } = 1;

    [BsonElement("_rev")]
    [BsonSerializer(typeof(FlexibleStringSerializer))]
    public string Revision { get; set; } = string.Empty;
}
