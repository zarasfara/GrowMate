using GrowMate.Domain.Plants;

namespace GrowMate.Domain.GardenTasks;

public class GardenTask
{

    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime DueDate { get; set; } = DateTime.Now;

    public bool IsCompleted { get; set; } = false;

    public int PlantId { get; set; }

    public Plant? Plant { get; set; }
}