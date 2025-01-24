using CleanArchitecture.Locations.Domain.Entities;
using CleanArchitecture.Locations.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace CleanArchitecture.Locations.Webapi.Tests.IntegrationTests;


public class ApiDbContextIntegrationTests
{
    [Fact]
    public async Task Can_Add_Location_To_DbContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb1")
            .Options;

        await using var context = new ApiDbContext(options);
        var location = new Location { Latitude = 52.0, Longitude = 4.0 };

        // Act
        await context.Locations.AddAsync(location);
        await context.SaveChangesAsync();

        // Assert
        var retrievedLocation = await context.Locations.FirstOrDefaultAsync(l => l.Id == location.Id);
        retrievedLocation.Should().NotBeNull();
        retrievedLocation?.Latitude.Should().Be(52.0);
        retrievedLocation?.Longitude.Should().Be(4.0);
    }

    [Fact]
    public async Task SeedAsync_Populates_Database_If_Empty()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb2")
            .Options;

        await using var context = new ApiDbContext(options);

        // Act
        await ApiDbContextSeed.SeedAsync(context);
        
        // Assert
        var locations = await context.Locations.ToListAsync();
        locations.Should().HaveCount(3);
        locations.Should().Contain(l => l.Id == 1 && l.Latitude == 52.0 && l.Longitude == 4.0);
        locations.Should().Contain(l => l.Id == 2 && l.Latitude == 53.0 && l.Longitude == 5.0);
        locations.Should().Contain(l => l.Id == 3 && l.Latitude == 54.0 && l.Longitude == 6.0);
    }
}