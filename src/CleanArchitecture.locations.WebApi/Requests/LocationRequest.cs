using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Locations.Webapi.Requests;

public class LocationRequest
{
    [Required] 
    public double Latitude { get; set; }
    
    [Required] 
    public double Longitude { get; set; }
}