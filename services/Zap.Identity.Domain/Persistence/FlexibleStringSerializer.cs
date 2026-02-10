using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Zap.Identity.Domain.Persistence;

public class FlexibleStringSerializer : SerializerBase<string>
{
    public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.CurrentBsonType;
        switch (bsonType)
        {
            case BsonType.String:
                return context.Reader.ReadString();
            case BsonType.Int32:
                return context.Reader.ReadInt32().ToString();
            case BsonType.Int64:
                return context.Reader.ReadInt64().ToString();
            case BsonType.Null:
                context.Reader.ReadNull();
                return string.Empty;
            default:
                var value = BsonSerializer.Deserialize<BsonValue>(context.Reader);
                return value.ToString() ?? string.Empty;
        }
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
    {
        context.Writer.WriteString(value ?? string.Empty);
    }
}
