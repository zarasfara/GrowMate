using GrowMate.Domain.Plants;

namespace GrowMate.Domain.GardenTasks;

public class GardenTask
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    /// <summary>
    ///     Тип задачи (полив, подкормка, обработка и т.д.).
    /// </summary>
    public TaskType Type { get; set; } = TaskType.Other;

    public DateTime DueDate { get; set; } = DateTime.Now;

    public bool IsCompleted { get; set; } = false;

    public int PlantId { get; set; }

    public Plant? Plant { get; set; }
}