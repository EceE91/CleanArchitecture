using CleanArchitecture.Locations.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Locations.Webapi.Tests.Factories;

/*
 * It allows us to customize how the application is configured and initialized specifically for testing purposes.
 * This factory enables running integration tests that simulate the behavior of the actual web application,
 * including middleware, services, and the API itself.

 * It sets up a test server that runs your application in a controlled environment,
 * enabling us to send HTTP requests to the API just like a real client would.
 
 * It provides the ability to override configurations such as services
 * (e.g., using a test database instead of a production one), environment settings, or other dependencies for the tests.
   It allows the use of dependency injection to inject mocks or fakes
   , which is useful for isolating specific components during testing.
 */

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing ApiDbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApiDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a new in-memory database for testing
            services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Create a scope to be able to resolve services
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
                options.UseInternalServiceProvider(serviceProvider);
            });

            // Ensure the database is created
            using var scope = services.BuildServiceProvider().CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApiDbContext>();
            db.Database.EnsureCreated();
        });
    }
}

