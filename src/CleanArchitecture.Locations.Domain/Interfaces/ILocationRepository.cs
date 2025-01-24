using CleanArchitecture.Locations.Domain.Entities;

namespace CleanArchitecture.Locations.Domain.Interfaces;

public interface ILocationRepository
{
    Task<List<Location>> GetLocationsAsync(CancellationToken cancellationToken);
    Task AddLocationAsync(Location location, CancellationToken cancellationToken);
    Task UpdateLocationAsync(Location location, CancellationToken cancellationToken);
    Task<Location?> GetLocationByIdAsync(int locationId, CancellationToken cancellationToken);
}