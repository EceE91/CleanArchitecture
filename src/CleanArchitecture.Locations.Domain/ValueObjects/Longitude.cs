namespace CleanArchitecture.Locations.Domain.ValueObjects;

public record Longitude(double Value)
{
    // The Longitude record encapsulates latitude values, providing type safety and clarity
    // it explicitly represents a longitude, not just any double value
    
    //It allows me to implicitly convert an instance of Longitude to a double without needing explicit casting.
    public static implicit operator double(Longitude longitude) => longitude.Value;
}