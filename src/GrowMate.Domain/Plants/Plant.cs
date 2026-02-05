using GrowMate.Domain.GardenTasks;

namespace GrowMate.Domain.Plants;

public sealed class Plant
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Variety { get; set; }

    public string? Description { get; set; }

    public DateTime PlantingDate { get; set; } = DateTime.Now;

    public PlantType Type { get; set; }

    public string UserId { get; set; } = string.Empty;
    
    public ICollection<GardenTask> Tasks { get; set; } = new List<GardenTask>();
}