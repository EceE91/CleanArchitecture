using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Asp.Versioning;
using FluentValidation;
using CleanArchitecture.Locations.Application.Commands;
using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Application.Handlers;
using CleanArchitecture.Locations.Application.Interfaces;
using CleanArchitecture.Locations.Application.Mappings;
using CleanArchitecture.Locations.Application.Queries;
using CleanArchitecture.Locations.Application.Services;
using CleanArchitecture.Locations.Domain.Interfaces;
using CleanArchitecture.Locations.Infrastructure.Behavior;
using CleanArchitecture.Locations.Infrastructure.Persistence;
using CleanArchitecture.Locations.Infrastructure.Repositories;
using CleanArchitecture.Locations.Webapi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Locations.Webapi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers().AddNewtonsoftJson();
            
            // add api versioning to support multiple API versions
            builder.Services.AddApiVersioning(
                x =>
                {
                    x.DefaultApiVersion = new ApiVersion(1, 0);
                    // set the default version when the client has not specified any versions.
                    // If we haven't set this flag to true and client hit the API
                    // without mentioning the version then UnsupportedApiVersion exception occurs (api-supported-versions is set)
                    x.AssumeDefaultVersionWhenUnspecified = true;
                    x.ReportApiVersions = true; // return the API version in response header.
                }
            );
            
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            
            builder.Services.AddAutoMapper(typeof(LocationProfile));
            
            // repositories
            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            
            // handlers
            builder.Services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddScoped<IRequestHandler<GetAllLocationsQuery, IEnumerable<LocationDto>>, GetAllLocationsQueryHandler>();
            builder.Services.AddScoped<IRequestHandler<AddLocationCommand, LocationDto>, AddLocationCommandHandler>();
            builder.Services.AddScoped<IValidator<AddLocationCommand>, AddLocationCommandValidator>();
            builder.Services.AddScoped<IRequestHandler<UpdateLocationCommand, LocationDto>, UpdateLocationCommandHandler>();
            builder.Services.AddScoped<IValidator<UpdateLocationCommand>, UpdateLocationCommandValidator>();
            
            // database context
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
            
            // services
            builder.Services.AddScoped<ILocationDistanceCalculatorService, LocationDistanceCalculatorService>();
            
            WebApplication app = builder.Build();
            
            // Seed data to database at startup
            using(var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                await ApiDbContextSeed.SeedAsync(context);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            
            app.UseExceptionHandler();

            await app.RunAsync();
        }
    }
}
