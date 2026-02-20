using System.Security.Claims;
using GrowMate.Domain.GardenBeds;
using GrowMate.Domain.GardenTasks;
using GrowMate.Domain.Plants;
using GrowMate.Domain.Users;
using GrowMate.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GrowMate.Presentation.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationContext _context;
    private readonly UserManager<User> _userManager;

    public IndexModel(ApplicationContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Данные для отображения
    public List<GardenBed> GardenBeds { get; set; } = new();
    public List<Plant> RecentPlants { get; set; } = new();
    public List<GardenTask> UpcomingTasks { get; set; } = new();

    // Статистика
    public int TotalGardenBeds => GardenBeds.Count;
    public int TotalPlants => RecentPlants.Count;
    public int PendingTasks => UpcomingTasks.Count(t => !t.IsCompleted);
    public int CompletedTasksToday => UpcomingTasks.Count(t => t.IsCompleted && t.DueDate.Date == DateTime.Today);

    // Дополнительная статистика
    public int OverdueTasks => UpcomingTasks.Count(t => !t.IsCompleted && t.DueDate.Date < DateTime.Today);
    public int TasksToday => UpcomingTasks.Count(t => t.DueDate.Date == DateTime.Today);
    public int PlantsByType(PlantType type) => RecentPlants.Count(p => p.Type == type);

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            await LoadDataFromDatabaseAsync(user.Id);
        }
    }

    private async Task LoadDataFromDatabaseAsync(string userId)
    {
        try
        {
            GardenBeds = await _context.GardenBeds
                .Where(g => g.UserId == userId)
                .ToListAsync();

            RecentPlants = await _context.Plants
                .Where(p => p.UserId == userId)
                .Include(p => p.GardenBed)
                .OrderByDescending(p => p.PlantingDate)
                .Take(12)
                .ToListAsync();

            UpcomingTasks = await _context.GardenTasks
                .Include(t => t.Plant)
                .Where(t => t.Plant != null && t.Plant.UserId == userId && t.DueDate >= DateTime.Today.AddDays(-7))
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }
        catch
        {
            // Если произойдет ошибка, показываем пустые данные
            GardenBeds = new List<GardenBed>();
            RecentPlants = new List<Plant>();
            UpcomingTasks = new List<GardenTask>();
        }
    }
}

