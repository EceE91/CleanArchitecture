using CleanArchitecture.Locations.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Locations.Application.Queries;

public class GetAllLocationsQuery : IRequest<IEnumerable<LocationDto>>;