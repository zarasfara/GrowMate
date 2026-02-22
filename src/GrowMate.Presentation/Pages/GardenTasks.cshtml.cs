using GrowMate.Domain.CalendarEvents;
using GrowMate.Domain.Users;
using GrowMate.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GrowMate.Presentation.Pages;

[Authorize]
public class GardenTasksModel : PageModel
{
    private static readonly HashSet<string> AllowedTypes =
    [
        "watering",
        "fertilizing",
        "treatment",
        "planting",
        "pruning",
        "harvest",
        "other"
    ];

    private readonly ApplicationContext _context;
    private readonly UserManager<User> _userManager;

    public GardenTasksModel(ApplicationContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetEventsAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var events = await _context.CalendarEvents
            .Where(e => e.UserId == user.Id)
            .OrderBy(e => e.ScheduledAt)
            .Select(e => new CalendarEventDto(
                e.Id,
                e.Title,
                e.Type,
                e.ScheduledAt.ToUniversalTime().ToString("O"),
                e.Notes ?? string.Empty
            ))
            .ToListAsync();

        return new JsonResult(events);
    }

    public async Task<IActionResult> OnPostEventsAsync([FromBody] CreateCalendarEventRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 120)
        {
            return BadRequest(new { message = "Некорректный заголовок события." });
        }

        if (!AllowedTypes.Contains(request.Type))
        {
            return BadRequest(new { message = "Некорректный тип события." });
        }

        if (!DateTimeOffset.TryParse(request.ScheduledAt, out var scheduledAt))
        {
            return BadRequest(new { message = "Некорректная дата и время." });
        }

        var entity = new CalendarEvent
        {
            UserId = user.Id,
            Title = request.Title.Trim(),
            Type = request.Type,
            ScheduledAt = scheduledAt.UtcDateTime,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };

        _context.CalendarEvents.Add(entity);
        await _context.SaveChangesAsync();

        var createdEvent = new CalendarEventDto(
            entity.Id,
            entity.Title,
            entity.Type,
            entity.ScheduledAt.ToUniversalTime().ToString("O"),
            entity.Notes ?? string.Empty
        );

        return new JsonResult(createdEvent);
    }

    public sealed record CreateCalendarEventRequest(string Title, string Type, string ScheduledAt, string? Notes);

    public sealed record CalendarEventDto(int Id, string Title, string Type, string ScheduledAt, string Notes);
}
