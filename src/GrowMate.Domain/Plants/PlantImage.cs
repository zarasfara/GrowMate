namespace GrowMate.Domain.Plants;

public sealed class PlantImage
{
    public int Id { get; set; }

    public required string Path { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int PlantId { get; set; }

    public Plant? Plant { get; set; }
}
