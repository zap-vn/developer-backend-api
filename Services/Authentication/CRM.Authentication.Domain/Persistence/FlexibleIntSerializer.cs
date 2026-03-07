using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CRM.Authentication.Domain.Persistence;

public class FlexibleIntSerializer : SerializerBase<int>
{
    public override int Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.CurrentBsonType;
        switch (bsonType)
        {
            case BsonType.Int32:
                return context.Reader.ReadInt32();
            case BsonType.Int64:
                return (int)context.Reader.ReadInt64();
            case BsonType.String:
                var s = context.Reader.ReadString();
                return int.TryParse(s, out var result) ? result : 0;
            case BsonType.Null:
                context.Reader.ReadNull();
                return 0;
            default:
                context.Reader.SkipValue();
                return 0;
        }
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, int value)
    {
        context.Writer.WriteInt32(value);
    }
}
