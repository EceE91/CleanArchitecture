using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Domain.ValueObjects;
using MediatR;

namespace CleanArchitecture.Locations.Application.Commands;

public class UpdateLocationCommand(int id, Latitude latitude, Longitude longitude) : IRequest<LocationDto>
{
    public int Id { get; } = id;
    public double Latitude { get; } = latitude;
    public double Longitude { get; } = longitude;
}