using AutoMapper;
using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Application.Queries;
using CleanArchitecture.Locations.Domain.Interfaces;
using MediatR;

namespace CleanArchitecture.Locations.Application.Handlers;

public class GetAllLocationsQueryHandler(ILocationRepository locationRepository, IMapper mapper)
    : IRequestHandler<GetAllLocationsQuery, IEnumerable<LocationDto>>
{
    public async Task<IEnumerable<LocationDto>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
    {
        var locations = await locationRepository.GetLocationsAsync(cancellationToken);
        return mapper.Map<IEnumerable<LocationDto>>(locations);
    }
}