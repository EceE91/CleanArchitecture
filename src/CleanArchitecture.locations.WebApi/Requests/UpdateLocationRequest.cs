using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Locations.Webapi.Requests;

public class UpdateLocationRequest
{
    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }
}