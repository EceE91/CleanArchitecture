using CleanArchitecture.Locations.Domain.ValueObjects;
using Newtonsoft.Json;

namespace CleanArchitecture.Locations.Application.DTOs;

public class LocationDto(int id, Latitude latitude, Longitude longitude)
{
    [JsonProperty(PropertyName = "id")]
    public int Id { get; } = id;

    [JsonProperty(PropertyName = "latitude")]
    public double Latitude { get; } = latitude;

    [JsonProperty(PropertyName = "longitude")]
    public double Longitude { get; } = longitude;
}