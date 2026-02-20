using GrowMate.Domain.GardenBeds;

namespace GrowMate.Presentation.Models;

/// <summary>
/// DTO для отображения грядки
/// </summary>
public class GardenBedDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public GardenBedType Type { get; set; }
    public int PlantCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO для создания грядки
/// </summary>
public class CreateGardenBedDto
{
    public string? Name { get; set; }
    public GardenBedType Type { get; set; } = GardenBedType.OpenGround;
}

/// <summary>
/// DTO для обновления грядки
/// </summary>
public class UpdateGardenBedDto
{
    public string? Name { get; set; }
    public GardenBedType Type { get; set; }
}


