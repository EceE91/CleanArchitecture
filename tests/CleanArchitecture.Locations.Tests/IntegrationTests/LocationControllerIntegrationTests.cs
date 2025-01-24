using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Webapi.Requests;
using CleanArchitecture.Locations.Webapi.Tests.Json;
using CleanArchitecture.Locations.Webapi.Tests.Factories;
using Newtonsoft.Json;
using Xunit;

namespace CleanArchitecture.Locations.Webapi.Tests.IntegrationTests;

public class LocationsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly JsonSerializer _jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings().InitializeDefaultSettings());
    
    [Fact]
    public async Task GetAllLocations_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/Locations");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var locations = _jsonSerializer.Deserialize<List<LocationDto>>(await response.Content.ReadAsStreamAsync());
        locations.Should().NotBeNull();
        locations.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task AddLocation_ReturnsOk()
    {
        // Arrange
        var newLocation = new AddLocationRequest
        {
            Latitude = 52.1,
            Longitude = 4.1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/Locations", newLocation);

        var addedLocation = _jsonSerializer.Deserialize<LocationDto>(await response.Content.ReadAsStreamAsync());
        
        // Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            addedLocation.Should().NotBeNull();
            addedLocation?.Latitude.Should().Be(newLocation.Latitude);
            addedLocation?.Longitude.Should().Be(newLocation.Longitude);
            addedLocation?.Id.Should().BeGreaterThan(0);
        }
    }

    [Fact]
    public async Task UpdateLocation_ReturnsOk()
    {
        // Arrange
        var newLocation = new AddLocationRequest
        {
            Latitude = 52.1,
            Longitude = 4.1
        };

        var addResponse = await _client.PostAsJsonAsync("/Locations", newLocation);
        var addedLocation = _jsonSerializer.Deserialize<LocationDto>(await addResponse.Content.ReadAsStreamAsync());

        if (addedLocation != null)
        {
            var updateRequest = new UpdateLocationRequest
            {
                Latitude = 52.2,
                Longitude = 4.2
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/Locations/{addedLocation.Id}", updateRequest);
        
            var updatedLocation = _jsonSerializer.Deserialize<LocationDto>(await response.Content.ReadAsStreamAsync());
        
            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                updatedLocation.Should().NotBeNull();
                updatedLocation?.Latitude.Should().Be(updateRequest.Latitude);
                updatedLocation?.Longitude.Should().Be(updateRequest.Longitude);
            }
        }
    }

    [Fact]
    public async Task UpdateLocation_LocationDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        const int nonExistentId = 9999;
        var updateRequest = new UpdateLocationRequest
        {
            Latitude = 52.2,
            Longitude = 4.2
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Locations/{nonExistentId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddLocation_InvalidCoordinates_ReturnsBadRequest()
    {
        // Arrange
        var invalidLocation = new AddLocationRequest
        {
            Latitude = 95.0, // Invalid latitude
            Longitude = 190.0 // Invalid longitude
        };

        // Act
        var response = await _client.PostAsJsonAsync("/Locations", invalidLocation);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(-100.0, 70.0)]  // Invalid latitude
    [InlineData(40.0, -200.0)] // Invalid longitude
    public async Task UpdateLocation_InvalidCoordinates_ReturnsBadRequest(double latitude, double longitude)
    {
        // Arrange
        const int locationId = 1;
        var invalidLocation = new UpdateLocationRequest
        {
            Latitude = latitude,
            Longitude = longitude
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Locations/{locationId}", invalidLocation);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}