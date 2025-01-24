using Newtonsoft.Json;

namespace CleanArchitecture.Locations.Webapi.Tests.Json;

/*
 * When reading JSON, the converters create new Latitude and Longitude instances from the raw numeric values (doubles).
 * If the value cannot be converted, they throw a NotSupportedException.
 */
public static class DefaultJsonSerializerSettings
{
    public static JsonSerializerSettings InitializeDefaultSettings(this JsonSerializerSettings settings)
    {
        settings.Converters.Add(new LatitudeJsonConverter());
        settings.Converters.Add(new LongitudeJsonConverter());
        return settings;
    }
}