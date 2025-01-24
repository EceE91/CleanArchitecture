using System.Net;
using System.Text;
using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Infrastructure.Persistence;
using CleanArchitecture.Locations.Webapi.Requests;
using CleanArchitecture.Locations.Webapi.Tests.Factories;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace CleanArchitecture.Locations.Webapi.Tests.IntegrationTests;

public class DistanceCalculatorControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    
    // This method will reset the in-memory database
    // We don't actually need it, but I decided to leave it for demonstration purposes
    private async Task ResetDatabaseAsync()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var scopedServices = scope.ServiceProvider;
        var dbContext = scopedServices.GetRequiredService<ApiDbContext>();

        // Clear any existing data
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }
    
    [Fact]
    public async Task Calculate_ValidLocations_ReturnsOk()
    {
        // Arrange
        var locations = new List<LocationRequest>
        {
            new() { Latitude = 34.0522, Longitude = -118.2437 }, // Los Angeles
            new() { Latitude = 36.1699, Longitude = -115.1398 }  // Las Vegas
        };
            
        var jsonContent = JsonConvert.SerializeObject(locations);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/DistanceCalculator/calculate", httpContent);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var distanceDto = JsonConvert.DeserializeObject<DistanceDto>(responseString);
        Assert.NotNull(distanceDto);
        Assert.True(distanceDto.Distance >= 0); // Assuming distance cannot be negative
    }

    [Fact]
    public async Task Calculate_InsufficientLocations_ReturnsBadRequest()
    {
        // Arrange
        var locations = new List<LocationRequest> // Only one location provided
        {
            new() { Latitude = 34.0522, Longitude = -118.2437 } // Los Angeles
        };
            
        var jsonContent = JsonConvert.SerializeObject(locations);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/DistanceCalculator/calculate", httpContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}