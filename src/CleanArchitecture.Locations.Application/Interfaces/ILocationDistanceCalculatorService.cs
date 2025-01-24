using CleanArchitecture.Locations.Domain.ValueObjects;

namespace CleanArchitecture.Locations.Application.Interfaces;

public interface ILocationDistanceCalculatorService
{
     double GetDistanceBetweenLocations((Latitude Latitude, Longitude Longitude) location1, (Latitude Latitude, Longitude Longitude) location2);   
}