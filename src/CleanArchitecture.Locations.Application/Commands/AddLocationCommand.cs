using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Domain.ValueObjects;
using MediatR;

namespace CleanArchitecture.Locations.Application.Commands;

public class AddLocationCommand(Latitude latitude, Longitude longitude) : IRequest<LocationDto>
{
    public double Latitude { get; } = latitude;
    public double Longitude { get; } = longitude;
}