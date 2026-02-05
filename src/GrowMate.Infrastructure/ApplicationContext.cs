using GrowMate.Domain.GardenTasks;
using GrowMate.Domain.Plants;
using GrowMate.Infrastructure.GardenTasks;
using GrowMate.Infrastructure.Plants;
using Microsoft.EntityFrameworkCore;

namespace GrowMate.Infrastructure;

public sealed class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) 
        : base(options)
    {
    }

    public DbSet<Plant> Plants { get; set; }

    public DbSet<GardenTask> GardenTasks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
            
        modelBuilder.ApplyConfiguration(new PlantEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GardenTaskEntityTypeConfiguration());
    }
}