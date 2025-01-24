namespace CleanArchitecture.Locations.Domain.Exceptions;

public class DistanceCannotBeCalculatedException()
    : Exception("You need 2 location information to calculate distance");

