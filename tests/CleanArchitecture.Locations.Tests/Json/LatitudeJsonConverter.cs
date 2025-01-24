using CleanArchitecture.Locations.Domain.ValueObjects;
using Newtonsoft.Json;

namespace CleanArchitecture.Locations.Webapi.Tests.Json;

public class LatitudeJsonConverter : JsonConverter<Latitude>
{
    public override void WriteJson(JsonWriter writer, Latitude? value, JsonSerializer serializer)
    {
        if (value != null)
        {
            writer.WriteValue(value.Value);
        }
    }
    
    public override Latitude ReadJson(JsonReader reader, Type objectType, Latitude? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.Value is double doubleValue)
        {
            return new Latitude(doubleValue);
        }
        throw new NotSupportedException();
    }
}