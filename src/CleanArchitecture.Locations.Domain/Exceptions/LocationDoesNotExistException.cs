namespace CleanArchitecture.Locations.Domain.Exceptions;

public class LocationDoesNotExistException(int locationId)
    : Exception($"LocationId {locationId} does not exist.");