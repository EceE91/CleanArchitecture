using AutoMapper;
using CleanArchitecture.Locations.Application.Commands;
using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Domain.Entities;
using CleanArchitecture.Locations.Domain.Interfaces;
using MediatR;

namespace CleanArchitecture.Locations.Application.Handlers;

public class UpdateLocationCommandHandler(ILocationRepository locationRepository, IMapper mapper)
    : IRequestHandler<UpdateLocationCommand, LocationDto>
{
    public async Task<LocationDto> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        await locationRepository.UpdateLocationAsync(new Location
        {
            Id = request.Id, 
            Latitude = request.Latitude, 
            Longitude = request.Longitude
        }, cancellationToken);
        var updatedLocation = await locationRepository.GetLocationByIdAsync(request.Id, cancellationToken);
        return mapper.Map<LocationDto>(updatedLocation);
    }
}