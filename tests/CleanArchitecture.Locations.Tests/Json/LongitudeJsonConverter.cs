using CleanArchitecture.Locations.Domain.ValueObjects;
using Newtonsoft.Json;

namespace CleanArchitecture.Locations.Webapi.Tests.Json;

public class LongitudeJsonConverter : JsonConverter<Longitude>
{
    public override void WriteJson(JsonWriter writer, Longitude? value, JsonSerializer serializer)
    {
        if (value != null)
        {
            writer.WriteValue(value.Value);
        }
    }
    
    public override Longitude ReadJson(JsonReader reader, Type objectType, Longitude? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.Value is double doubleValue)
        {
            return new Longitude(doubleValue);
        }
        throw new NotSupportedException();
    }
}