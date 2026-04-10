using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRM.BuildingBlocks.Serialization
{
    public class EmptyStringToNullableGuidConverter : JsonConverter<Guid?>
    {
        public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string stringValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return null;
                }

                if (Guid.TryParse(stringValue, out Guid guid))
                {
                    return guid;
                }

                throw new JsonException($"'{stringValue}' is not a valid GUID.");
            }

            if (reader.TokenType == JsonTokenType.Null || reader.TokenType == JsonTokenType.Number)
            {
                // Accept 0 or other numbers as a fallback to null from weak-typed frontends
                if (reader.TokenType == JsonTokenType.Number) {
                    reader.GetDouble(); // Advance reader safely over the number token
                }
                return null;
            }

            throw new JsonException($"Unexpected token parsing Guid?. Expected String or Null, got {reader.TokenType}.");
        }

        public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
