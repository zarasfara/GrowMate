using GrowMate.Domain.Plants;

namespace GrowMate.Domain.GardenBeds;

public sealed class GardenBed
{
    public int Id { get; set; }

    /// <summary>
    ///     Название грядки, отображаемое пользователю.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Тип грядки (открытый грунт, теплица, контейнер и т.д.).
    /// </summary>
    public GardenBedType Type { get; set; } = GardenBedType.OpenGround;

    /// <summary>
    ///     Идентификатор пользователя-владельца грядки.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    ///     Посадки (растения), размещённые на этой грядке.
    /// </summary>
    public ICollection<Plant> Plants { get; set; } = [];
}