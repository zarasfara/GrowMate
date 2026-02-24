using GrowMate.Domain.CalendarEvents;
using GrowMate.Domain.GardenBeds;
using GrowMate.Domain.GardenTasks;
using GrowMate.Domain.Plants;
using GrowMate.Domain.Users;
using GrowMate.Infrastructure.CalendarEvents;
using GrowMate.Infrastructure.GardenBeds;
using GrowMate.Infrastructure.GardenTasks;
using GrowMate.Infrastructure.Plants;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GrowMate.Infrastructure;

public sealed class ApplicationContext : IdentityDbContext<User>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public DbSet<GardenBed> GardenBeds { get; set; }

    public DbSet<Plant> Plants { get; set; }

    public DbSet<PlantImage> PlantImages { get; set; }

    public DbSet<PlantTemplate> PlantTemplates { get; set; }

    public DbSet<GardenTask> GardenTasks { get; set; }

    public DbSet<CalendarEvent> CalendarEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new GardenBedEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PlantTemplateEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PlantEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PlantImageEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GardenTaskEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CalendarEventEntityTypeConfiguration());
    }
}