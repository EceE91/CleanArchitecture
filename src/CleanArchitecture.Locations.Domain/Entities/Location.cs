namespace CleanArchitecture.Locations.Domain.Entities;

public record Location 
{
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}