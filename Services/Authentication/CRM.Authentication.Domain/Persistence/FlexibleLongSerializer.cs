using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CRM.Authentication.Domain.Persistence;

public class FlexibleLongSerializer : SerializerBase<long>
{
    public override long Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.CurrentBsonType;
        switch (bsonType)
        {
            case BsonType.Int32:
                return context.Reader.ReadInt32();
            case BsonType.Int64:
                return context.Reader.ReadInt64();
            case BsonType.String:
                var s = context.Reader.ReadString();
                return long.TryParse(s, out var result) ? result : 0;
            case BsonType.Double:
                return (long)context.Reader.ReadDouble();
            case BsonType.Null:
                context.Reader.ReadNull();
                return 0;
            default:
                context.Reader.SkipValue();
                return 0;
        }
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, long value)
    {
        context.Writer.WriteInt64(value);
    }
}
