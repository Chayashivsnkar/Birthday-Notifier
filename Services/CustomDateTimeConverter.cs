using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CustomDateTimeConverter : JsonConverter<DateTime?>
{
    private readonly string[] _formats = new[]
    {
        "M/d/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ssZ"
    };

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (DateTime.TryParseExact(str, _formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result))
            return result;

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
        else
            writer.WriteNullValue();
    }
}
