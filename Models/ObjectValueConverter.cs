using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConstructionAPI.Models
{
    public class ObjectValueConverter : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                case JsonTokenType.False:
                    return reader.GetBoolean();

                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out long l))
                        return l;
                    return reader.GetDouble();

                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out DateTime datetime))
                        return datetime;
                    return reader.GetString();

                default:
                    using (var document = JsonDocument.ParseValue(ref reader))
                    {
                        return document.RootElement.Clone();
                    }
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case bool b:
                    writer.WriteBooleanValue(b);
                    break;

                case long l:
                    writer.WriteNumberValue(l);
                    break;

                case int i:
                    writer.WriteNumberValue(i);
                    break;

                case double d:
                    writer.WriteNumberValue(d);
                    break;

                case string s:
                    writer.WriteStringValue(s);
                    break;

                case DateTime dt:
                    writer.WriteStringValue(dt);
                    break;

                default:
                    JsonSerializer.Serialize(writer, value, options);
                    break;
            }
        }
    }
}