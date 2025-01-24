using CleanArchitecture.Locations.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Locations.Infrastructure.Persistence;

public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // auto-increment id
        modelBuilder.Entity<Location>(location =>
        {
            location.Property(e => e.Id);
            location.HasKey(e => e.Id);
        });
    }
    
    public DbSet<Location> Locations { get; set; }
}