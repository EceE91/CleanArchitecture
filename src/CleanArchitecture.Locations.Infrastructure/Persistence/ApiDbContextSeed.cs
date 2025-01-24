using CleanArchitecture.Locations.Domain.Entities;

namespace CleanArchitecture.Locations.Infrastructure.Persistence;

public static class ApiDbContextSeed
{
    public static async Task SeedAsync(ApiDbContext dbContext)
    {
        if (!dbContext.Locations.Any())
        {
            var locations = new List<Location>
            {
                new()
                {
                    Latitude = 52.0000000d, Longitude = 4.0000000d
                },
                new()
                {
                    Latitude = 53.0000000d, Longitude = 5.0000000d
                },
                new()
                {
                    Latitude = 54.0000000d, Longitude = 6.0000000d
                }
            };

            await dbContext.AddRangeAsync(locations);
            await dbContext.SaveChangesAsync();   
        }
    }
}