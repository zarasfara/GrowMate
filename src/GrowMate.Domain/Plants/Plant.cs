using GrowMate.Domain.GardenTasks;

namespace GrowMate.Domain.Plants;

public sealed class Plant
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Variety { get; set; }

    public string? Description { get; set; }

    public DateTime PlantingDate { get; set; } = DateTime.Now;

    public PlantType Type { get; set; }

    public required string UserId { get; set; }
    
    public ICollection<GardenTask> Tasks { get; set; } = [];
}