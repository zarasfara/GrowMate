namespace GrowMate.Domain.Plants;

/// <summary>
///     Справочная запись о культуре (шаблон растения),
///     на основе которой создаются конкретные посадки (Plant).
/// </summary>
public sealed class PlantTemplate
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public PlantType Type { get; set; }

    public string? Description { get; set; }

    /// <summary>
    ///     Рекомендуемый интервал полива в днях (если задан).
    /// </summary>
    public int? WateringIntervalDays { get; set; }

    /// <summary>
    ///     Через сколько дней после посадки рекомендована первая подкормка.
    /// </summary>
    public int? FirstFertilizingAfterDays { get; set; }

    /// <summary>
    ///     Через сколько дней после посадки рекомендована первая обработка.
    /// </summary>
    public int? FirstTreatmentAfterDays { get; set; }

    /// <summary>
    ///     Конкретные посадки, созданные по этому шаблону.
    /// </summary>
    public ICollection<Plant> Plants { get; set; } = [];
}