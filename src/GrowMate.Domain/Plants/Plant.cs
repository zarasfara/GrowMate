using GrowMate.Domain.GardenBeds;
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

    /// <summary>
    ///     Количество растений данного типа (для групповых посадок, например, 20 помидоров).
    ///     Для уникальных растений (розы, деревья) = 1.
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    ///     Является ли это уникальной посадкой (true для роз, деревьев) или групповой (false для помидоров).
    /// </summary>
    public bool IsUnique { get; set; }

    public required string UserId { get; set; }

    /// <summary>
    ///     Необязательная ссылка на шаблон культуры, по которому создана эта посадка.
    /// </summary>
    public int? PlantTemplateId { get; set; }

    public PlantTemplate? Template { get; set; }

    /// <summary>
    ///     Идентификатор грядки, на которой находится растение.
    /// </summary>
    public int GardenBedId { get; set; }

    /// <summary>
    ///     Навигационное свойство к грядке.
    /// </summary>
    public GardenBed? GardenBed { get; set; }

    public ICollection<GardenTask> Tasks { get; set; } = [];
}