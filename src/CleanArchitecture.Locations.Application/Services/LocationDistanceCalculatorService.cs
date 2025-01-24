using System.Text;
using CleanArchitecture.Locations.Application.Interfaces;
using CleanArchitecture.Locations.Domain.ValueObjects;

namespace CleanArchitecture.Locations.Application.Services;

// GeoCoordinatePortable library could be used to calculate the distance
// var sourceCoord = new GeoCoordinate(loc1.Latitude, loc1.Longitude);
// var destinationCoord = new GeoCoordinate(loc2.Latitude, loc2.Longitude);
// var distanceInMeters = sourceCoord.GetDistanceTo(destinationCoord);
public class LocationDistanceCalculatorService : ILocationDistanceCalculatorService
{
    private const double EarthRadius = 6371e3; // Earth's radius in meters

    public double GetDistanceBetweenLocations((Latitude Latitude, Longitude Longitude) location1, (Latitude Latitude, Longitude Longitude) location2)
    {
        var lat1 = location1.Latitude * Math.PI / 180.0;
        var lat2 = location2.Latitude * Math.PI / 180.0;
        var latDiff = (location2.Latitude - location1.Latitude) * Math.PI / 180.0;
        var lonDiff = (location2.Longitude - location1.Longitude) * Math.PI / 180.0;

        var a = Math.Sin(latDiff / 2) * Math.Sin(latDiff / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(lonDiff / 2) * Math.Sin(lonDiff / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        
        var distance = EarthRadius * c;
        return distance;
    }
}