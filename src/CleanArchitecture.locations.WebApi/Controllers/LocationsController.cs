using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using CleanArchitecture.Locations.Application.Commands;
using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Application.Queries;
using CleanArchitecture.Locations.Domain.ValueObjects;
using CleanArchitecture.Locations.Webapi.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Locations.Webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class LocationsController(
        IMediator mediator, // CQRS and MediatR to decouple the controller from the business logic.
                            // loose-coupling, single responsibility, testability, segragation
        ILogger<LocationsController> logger)
        : ControllerBase
    {
        [ApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<LocationDto>> GetAllLocations(CancellationToken cancellationToken)
        {
            var query = new GetAllLocationsQuery();
            var locations = await mediator.Send(query, cancellationToken);
            logger.LogInformation($"{locations.Count()} locations are fetched from the database");
            return Ok(locations);
        }
        
        // Note: POST isn't idempotent, thus we need some sort of requestId or idempotencyKey check!
        [ApiVersion("1.0")]
        [HttpPost]
        public async Task<ActionResult<LocationDto>> AddLocation([FromBody] AddLocationRequest request, CancellationToken cancellationToken)
        {
            var command = new AddLocationCommand(new Latitude(request.Latitude), new Longitude(request.Longitude));
            var location = await mediator.Send(command, cancellationToken);
            return Ok(location);
        }
        
        // Note: Instead of strictly following REST and using PUT, which should create non-existent resources,
        // I decided to implement an endpoint only to update existing locations to demonstrate a different scenario.
        // That's why again I'm using POST instead of PUT here.
        [ApiVersion("1.0")]
        [HttpPost("{id}")]
        public async Task<ActionResult<LocationDto>> UpdateLocation([FromRoute] int id, [FromBody] UpdateLocationRequest request, CancellationToken cancellationToken)
        {
           var command = new UpdateLocationCommand(id, new Latitude(request.Latitude), new Longitude(request.Longitude));
           var location = await mediator.Send(command, cancellationToken);
           return Ok(location);
        }
    }
}
