namespace CleanArchitecture.Locations.Domain.ValueObjects;

public record Latitude(double Value)
{
    // The Latitude record encapsulates latitude values, providing type safety and clarity
    // it explicitly represents a latitude, not just any double value
    
    //It allows me to implicitly convert an instance of Latitude to a double without needing explicit casting.
    public static implicit operator double(Latitude latitude) => latitude.Value;
}