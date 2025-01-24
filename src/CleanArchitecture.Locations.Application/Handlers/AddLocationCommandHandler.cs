using AutoMapper;
using CleanArchitecture.Locations.Application.Commands;
using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Domain.Entities;
using CleanArchitecture.Locations.Domain.Interfaces;
using MediatR;

namespace CleanArchitecture.Locations.Application.Handlers;

public class AddLocationCommandHandler(ILocationRepository locationRepository, IMapper mapper)
    : IRequestHandler<AddLocationCommand, LocationDto>
{

    public async Task<LocationDto> Handle(AddLocationCommand request, CancellationToken cancellationToken)
    {
        var location = new Location
        {
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };
        
        await locationRepository.AddLocationAsync(location, cancellationToken);
        
        return mapper.Map<LocationDto>(location);
    }
}