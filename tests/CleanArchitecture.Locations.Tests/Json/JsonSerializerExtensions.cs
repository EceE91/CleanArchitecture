using Newtonsoft.Json;

namespace CleanArchitecture.Locations.Webapi.Tests.Json;

public static class JsonSerializerExtensions
{
    public static T? Deserialize<T>(this JsonSerializer serializer, Stream stream)
    {
        using StreamReader reader = new StreamReader(stream);
        using JsonTextReader jsonReader = new JsonTextReader(reader);
        return serializer.Deserialize<T>(jsonReader);
    }
}