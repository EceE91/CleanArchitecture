using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Locations.Webapi.Requests;

public class AddLocationRequest
{
    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }
}