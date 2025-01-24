using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Application.Interfaces;
using CleanArchitecture.Locations.Application.Queries;
using CleanArchitecture.Locations.Webapi.Controllers;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentAssertions.Execution;
using CleanArchitecture.Locations.Application.Commands;
using CleanArchitecture.Locations.Domain.Exceptions;
using CleanArchitecture.Locations.Domain.ValueObjects;
using CleanArchitecture.Locations.Webapi.Requests;

namespace CleanArchitecture.Locations.Webapi.Tests.UnitTests;

public class LocationsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<LocationsController>> _loggerMock;
    private readonly LocationsController _controller;

    public LocationsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<LocationsController>>();

        _controller = new LocationsController(
            _mediatorMock.Object, 
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllLocations_ShouldReturnOk_WhenLocationsAreFetched()
    {
        // Arrange
        var locations = new List<LocationDto>
        {
            new(1, new Latitude(52.0), new Longitude(4.0)),
            new(2, new Latitude(53.0), new Longitude(5.0))
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllLocationsQuery>(), default))
            .ReturnsAsync(locations);

        // Act
        var result = await _controller.GetAllLocations(CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        using (new AssertionScope())
        {
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(locations);
        }
    }
    
    [Fact]
    public async Task AddLocation_ShouldReturnOk_WhenLocationIsAddedSuccessfully()
    {
        // Arrange
        var request = new AddLocationRequest
        {
            Latitude = 33.0,
            Longitude = 4.0
        };

        var locationDto = new LocationDto(5, new Latitude(request.Latitude), new Longitude(request.Longitude));

        _mediatorMock.Setup(m => m.Send(It.IsAny<AddLocationCommand>(), default))
            .ReturnsAsync(locationDto);

        // Act
        var result = await _controller.AddLocation(request,CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        using (new AssertionScope())
        {
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(locationDto);

            _mediatorMock.Verify(m => m.Send(It.Is<AddLocationCommand>(cmd =>
                cmd.Latitude == request.Latitude &&
                cmd.Longitude == request.Longitude), default), Times.Once);
        }
    }

    [Fact]
    public async Task AddLocation_ShouldReturnBadRequest_WhenAnExceptionOccurs()
    {
        // Arrange
        var request = new AddLocationRequest
        {
            Latitude = 52.0,
            Longitude = 4.0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<AddLocationCommand>(), default))
            .ThrowsAsync(new Exception("Something went wrong"));

        // Act
        Func<Task> act = async () => await _controller.AddLocation(request,CancellationToken.None);

        // Assert
        using (new AssertionScope())
        {
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Something went wrong");

            _mediatorMock.Verify(m => m.Send(It.IsAny<AddLocationCommand>(), default), Times.Once);
        }
    }
    
     [Fact]
    public async Task UpdateLocation_Should_Return_Ok_When_Location_Updated()
    {
        // Arrange
        const int locationId = 1;
        var request = new UpdateLocationRequest { Latitude = 40.0, Longitude = 70.0 };
        var expectedLocation = new LocationDto(1, new Latitude(40.0), new Longitude(70.0));

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateLocationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLocation);

        // Act
        var result = await _controller.UpdateLocation(locationId, request, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        using (new AssertionScope())
        {
            okResult.Should().NotBeNull();
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(expectedLocation);

            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateLocationCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}