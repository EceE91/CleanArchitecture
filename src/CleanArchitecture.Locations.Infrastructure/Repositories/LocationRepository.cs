using CleanArchitecture.Locations.Domain.Entities;
using CleanArchitecture.Locations.Domain.Exceptions;
using CleanArchitecture.Locations.Domain.Interfaces;
using CleanArchitecture.Locations.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Locations.Infrastructure.Repositories;

public class LocationRepository(ApiDbContext context) : ILocationRepository
{
    public Task<List<Location>> GetLocationsAsync(CancellationToken cancellationToken)
        => context.Locations.ToListAsync(cancellationToken: cancellationToken);
    
    public async Task AddLocationAsync(Location location, CancellationToken cancellationToken)
    {
        await context.AddAsync(location, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Location?> GetLocationByIdAsync(int locationId, CancellationToken cancellationToken)
        => context.Locations.FindAsync([locationId], cancellationToken).AsTask();
    
    public async Task UpdateLocationAsync(Location location, CancellationToken cancellationToken)
    {
        var found = await context.Locations.FindAsync([location.Id], cancellationToken);
        if (found == null) throw new LocationDoesNotExistException(location.Id);
        
        found.Latitude = location.Latitude;
        found.Longitude = location.Longitude;
        await context.SaveChangesAsync(cancellationToken);
    }
}
