# Clean Architecture 

Here's my Medium article on <a href="https://medium.com/@developerstory/how-should-the-folder-structure-look-when-implementing-clean-architecture-for-a-project-d2dc88de6c47 ">"How Should the Folder Structure Look When Implementing Clean Architecture for a Project?"<a/>


## Context: ##
The code is a simple web API for storing and retrieving locations.

A location is a set of coordinates defined by a latitude in degrees and a longitude in degrees. The API contains two endpoints.
One endpoint retrieves the currently stored location and one endpoint can change the currently stored location.

There's also an API to calculate the distance between two locations in the list.

For converting a distance in angles to a distance in metres for a single axis the following formula can also be used.
delta metres = delta degrees * pi / 180.0 * 6378137


<h3><b>Technology stack:</b></h3>

* net8.0
* Entity framework core in-memory database and Postgres database 
* CQRS with MediatR 
* Xunit with FluentAssertions and Moq for unit testing
* Middleware for custom error handling
* TestServer for integration testing
* Repository pattern
* SwaggerUI to display endpoints
* API versioning
* Automapper
* FluentValidation implemented via IPipelineBehavior to be able define rules in CommandValidator and QueryValidator

How to use the database
- I’ve added a property in appsettings.json called “UseInMemoryDatabase”. If this is set to True then the application uses Entity framework core in-memory database, otherwise it uses Postgres. The connection string for Postgres can be found in the same json file ConnectionStrings.PostgresConnectionString.
This is how I configure it in Program.cs

```
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    if (builder.Configuration.GetValue<bool>("UseInMemoryDatabase"))
    {
        options.UseInMemoryDatabase(databaseName: "LocationDb");
    }
    else
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnectionString"));
    }
});
```
- I seed some data to the database at startup:
```
using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    await ApiDbContextSeed.SeedAsync(context);
}
```
Please check ApiDbContextSeed.cs to find out how I seed the data to the database (both in-memory and Postgres).
- Migration file for Postgres can be found under Infrastructure layer

<h3><b>How code architecture looks like</b></h3>
I used CleanArchitecture (hegzagonal design)
1. Core (Domain): The application's business logic, including entities, events, enums, and interfaces (IRepository, IUnitOfWork), Exceptions.
2. Application: This contains commands, queries, handlers (in case of using CQRS and MediatR), the use cases (services), and services that implement the business rules, service interfaces
3. Infrastructure: This contains external service implementations, like repositories (implementation of ILocationRepository defined in Domain) and API integrations (e.g., Slack API, Kafka, RabbitMQ, SQS), context, migrations.
4. Presentation (API): This will contain the ASP.NET Core Web API that exposes endpoints for the Frontend and services. Controllers, DTOs, ViewModels, Middleware (errorhandling, logging), Configurations, Program.cs 
5. Tests: This contains unit and integration tests (I prefer TestServer, Moq, XUnit and FluentAssertions for testing) for each layer. (Specflow tests could also be added) 


[Presentation] -> [Application] -> [Core] <- [Infrastructure]

Use an API Gateway to centralize and manage all incoming API traffic. This allows you to manage versioning, load balancing, and security policies in one place.

```
/src
    /Domain
        /Entities - Location.cs
	/ValueObjects - Latitude.cs -Longitude.cs
        /Interfaces - ILocationRepository.cs
	/Exceptions - DistanceCannotBeCalculatedException.cs - LocationDoesNotExistException.cs
    /Application
	/Services - LocationDistanceCalculatorService.cs
	/Commands (I use CQRS with MediatR) - AddLocationCommand.cs - UpdateLocationCommand.cs
	/Handlers AddLocationCommandHandler.cs - UpdateLocationCommandHandler.cs - GetAllLocationsQueryHandler.cs - AddLocationCommandValidator.cs - UpdateLocationCommandValidator.cs
	/Queries - GetAllLocationsQuery.cs
        /DTOs - DistanceDto.cs, LocationDto
        /Interfaces - ILocationDistanceCalculatorService.cs
	/Mappings - LocationProfile.cs
    /Infrastructure
        /Persistence - ApiDbContextSeed.cs, ApiDbContext.cs
	/Repository - LocationRepository.cs
	/Behavior - ValidationBehavior.cs
	/Migrations -> For Postgres
    /API (Presentation)
        /Controllers - LocationsController.cs - DistanceCalculatorController.cs
        /Requests - AddLocationRequest.cs - UpdateLocationRequest.cs - LocationRequest.cs
        /Middleware - GlobalExceptionHandler.cs
	/Json - DefaultJsonSerializerSettings.cs - LatitudeJsonConverter.cs - LongitudeJsonConverter.cs
	Program.cs
    /tests (it uses efcore in-memory db for testing purposes)
        /Factories - CustomWebApplicationFactory.cs
        /Extensions - JsonSerializerExtensions.cs
        /UnitTests - ApiDbContextIntegrationTests.cs - LocationsControllerIntegrationTests.cs - DistanceCalculatorControllerIntegrationTests.cs
        /IntegrationTests - LocationsControllerTests.cs - DistanceCalculatorControllerTests.cs
```

This implementation creates a robust and maintainable ASP.NET Core application that incorporates PostgreSQL to manage location data. The architecture follows clean architecture principles, ensuring a separation of concerns. Each layer is clearly defined, and the use of patterns like the Repository helps manage data access and transactions effectively.

DistanceCalculatorController
It uses constructor injection to receive a service (ILocationDistanceCalculatorService) for calculating distances between two locations.Client sends a POST request to /DistanceCalculator/calculate with a list of at least two locations in the body.
The controller validates the input. If valid, it calculates the distance between the two locations using the injected service.
If successful, the calculated distance is returned in a DistanceDto.
If the input is invalid (fewer than two locations), a 400 Bad Request error is returned by throwing an exception.
ILocationDistanceCalculatorService: This is a service interface injected into the controller, responsible for providing the logic to calculate distances between two locations.
LocationRequest: This is the request DTO (Data Transfer Object) that represents location data from the client (containing properties like Latitude and Longitude).
DistanceDto: This DTO is the return type containing the calculated distance.

LocationsController
It provides several endpoints for retrieving and managing location data using the CQRS pattern (Command Query Responsibility Segregation) and MediatR for handling requests and commands.
This controller is responsible for handling HTTP requests related to locations. It accepts two dependencies via constructor injection:
- IMediator: MediatR is used for sending queries and commands to the relevant handlers in a decoupled way.
- ILogger<LocationsController>: A logger to log information, errors, or warnings.-
- The controller and its methods are versioned with API version "1.0" to ensure backward compatibility as the API evolves.
Endpoints:
    * GetAllLocations (GET): Fetches all stored locations.
        * It creates a GetAllLocationsQuery object and sends it to MediatR to fetch the locations.
        * Logs the number of locations fetched and returns the list in the response with a status code 200 OK.
    * AddLocation (POST): Adds a new location based on the provided latitude and longitude in the request.
        * It creates an AddLocationCommand and sends it to MediatR to add the new location.
        * The added location is returned in the response.
    * UpdateLocation (POST /update): Updates an existing location by ID.
        * It creates an UpdateLocationCommand and sends it to MediatR to update the location.
        * The updated location is returned in the response.
    * Note: The code includes a comment mentioning the need for idempotency checks for POST requests, indicating awareness of HTTP standards.
SOLID Principles:
1. Single Responsibility Principle (SRP):
    * Each class or method has one responsibility.
    * The controller's responsibility is limited to handling HTTP requests and interacting with the MediatR to perform operations. The actual business logic (adding, updating, and fetching locations) is handled by the respective Command and Query Handlers outside the controller.
2. Open/Closed Principle (OCP):
    * The controller is open for extension but closed for modification. For example, if additional commands, queries, or features are added, it doesn't require modifying existing controller logic. New handlers or new queries/commands can be added to MediatR instead.
    * New handlers can be plugged in without changing the controller itself.
3. Liskov Substitution Principle (LSP):
    * This principle isn’t explicitly showcased here since the controller doesn't derive from any base class or interface directly.
    * However, the use of DTOs and Command/Query objects ensures that MediatR can work interchangeably with different command or query implementations without breaking behavior.
4. Interface Segregation Principle (ISP):
    * The controller depends only on the IMediator and ILogger interfaces, keeping the code clean and avoiding unnecessary dependencies. The interfaces are focused and specific.
    * This ensures that the controller isn’t forced to depend on methods or logic it doesn’t use, adhering to ISP.
5. Dependency Inversion Principle (DIP):
    * The controller depends on abstractions (IMediator and ILogger<LocationsController>) rather than concrete implementations.
    * The actual implementations (such as handlers for commands and queries) are injected via dependency injection, ensuring that high-level modules (like the controller) do not depend on low-level modules (such as database or repository logic).
Usage of CQRS and MediatR:
* The controller uses CQRS (Command Query Responsibility Segregation), where queries are used for reading data (like GetAllLocationsQuery), and commands are used for modifying data (like AddLocationCommand and UpdateLocationCommand).
* MediatR acts as a mediator between the controller and the actual command/query handlers, ensuring a clean separation of concerns. The controller doesn't know or care about the implementation of these handlers, promoting loose coupling.
Clean Code Aspects:
* DTOs (like LocationDto) are used to decouple the domain model from the API layer and present a simplified version of the data to the client.
* Command and Query Objects encapsulate the intent (either retrieving data or performing an action) and separate the logic from the controller.
Summary of Key Points:
* This LocationsController adheres to the SOLID principles, ensuring the code is maintainable, flexible, and easy to extend without modifying existing logic.
* It uses CQRS and MediatR to decouple the controller from the business logic.
* The controller is versioned, indicating that this API is designed for long-term evolution while ensuring backward compatibility.
* The code adheres to modern ASP.NET Core conventions, such as using dependency injection, cancellation tokens, and async/await patterns for scalability.

