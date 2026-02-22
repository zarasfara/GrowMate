namespace GrowMate.Domain.CalendarEvents;

public sealed class CalendarEvent
{
    public int Id { get; set; }

    public required string UserId { get; set; }

    public required string Title { get; set; }

    public required string Type { get; set; }

    public DateTime ScheduledAt { get; set; }

    public string? Notes { get; set; }
}
