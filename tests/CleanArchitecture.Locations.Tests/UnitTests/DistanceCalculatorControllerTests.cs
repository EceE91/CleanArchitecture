using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Application.Interfaces;
using CleanArchitecture.Locations.Webapi.Controllers;
using Xunit;
using Moq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using CleanArchitecture.Locations.Domain.Exceptions;
using CleanArchitecture.Locations.Domain.ValueObjects;
using CleanArchitecture.Locations.Webapi.Requests;

namespace CleanArchitecture.Locations.Webapi.Tests.UnitTests;

public class DistanceCalculatorControllerTests
{
    private readonly Mock<ILocationDistanceCalculatorService> _locationDistanceCalculatorServiceMock; 
    private readonly DistanceCalculatorController _controller;

    public DistanceCalculatorControllerTests()
    {
        _locationDistanceCalculatorServiceMock = new Mock<ILocationDistanceCalculatorService>();
        _controller = new DistanceCalculatorController(_locationDistanceCalculatorServiceMock.Object);
    }
    
    [Fact]
    public void CalculateDistance_Should_Throw_DistanceCannotBeCalculatedException_When_Less_Than_Two_Locations()
    {
        // Arrange
        var locations = new List<LocationRequest>
        {
            new() { Latitude = 52.0, Longitude = 4.0 }
        };
    
        // Act
        var act = () => _controller.Calculate(locations);

        // Assert
        using (new AssertionScope())
        {
            act.Should().Throw<DistanceCannotBeCalculatedException>();

            _locationDistanceCalculatorServiceMock.Verify(
                s => s.GetDistanceBetweenLocations(It.IsAny<(Latitude, Longitude)>(), It.IsAny<(Latitude, Longitude)>()), Times.Never);
        }
    }


    [Fact]
    public void CalculateDistance_Should_Return_Ok_With_Valid_Distance()
    {
        // Arrange
        var locations = new List<LocationRequest>
        {
            new() { Latitude = 52.0, Longitude = 4.0 },
            new() { Latitude = 53.0, Longitude = 5.0 }
        };
    
        var expectedDistance = new DistanceDto(157000); // Example distance in meters

        _locationDistanceCalculatorServiceMock
            .Setup(s => s.GetDistanceBetweenLocations(It.IsAny<(Latitude, Longitude)>(), It.IsAny<(Latitude, Longitude)>()))
            .Returns(expectedDistance.Distance);

        // Act
        var result = _controller.Calculate(locations);

        // Assert
        var okResult = result.Result as OkObjectResult;
        using (new AssertionScope())
        {
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(expectedDistance);

            var location1 = (new Latitude(locations[0].Latitude), new Longitude(locations[0].Longitude));
            var location2 = (new Latitude(locations[1].Latitude), new Longitude(locations[1].Longitude));
            _locationDistanceCalculatorServiceMock.Verify(s => s.GetDistanceBetweenLocations(location1, location2),
                Times.Once);
        }
    }
}