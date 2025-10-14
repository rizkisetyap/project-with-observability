using System.Text.Json;
using System.Text.Json.Serialization;

namespace Observability.HealthCheck;

internal class TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        // Safely parse TimeSpan from string value
        if (
            reader.TokenType == JsonTokenType.String
            && TimeSpan.TryParse(reader.GetString(), out var result)
        )
        {
            return result;
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
