using System.Collections.Generic;
using Asp.Versioning;
using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Application.Interfaces;
using CleanArchitecture.Locations.Domain.Exceptions;
using CleanArchitecture.Locations.Domain.ValueObjects;
using CleanArchitecture.Locations.Webapi.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Locations.Webapi.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class DistanceCalculatorController(ILocationDistanceCalculatorService locationDistanceCalculatorService) : ControllerBase
{
    [ApiVersion("1.0")]
    [HttpPost("calculate")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<DistanceDto> Calculate([FromBody] List<LocationRequest> locations)
    {
        if (locations.Count >= 2)
        {
            var location1 = (new Latitude(locations[0].Latitude), new Longitude(locations[0].Longitude));
            var location2 = (new Latitude(locations[1].Latitude), new Longitude(locations[1].Longitude));
            var distance = locationDistanceCalculatorService.GetDistanceBetweenLocations(location1, location2);
            return Ok(new DistanceDto(distance));
        }
        
        throw new DistanceCannotBeCalculatedException();
    }
}