using GrowMate.Applicatoin.Services;
using GrowMate.Domain.GardenTasks;
using GrowMate.Domain.Users;
using GrowMate.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GrowMate.Presentation.Pages.GardenCare;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationContext _context;
    private readonly PlantService _plantService;
    private readonly UserManager<User> _userManager;

    public IndexModel(ApplicationContext context, PlantService plantService, UserManager<User> userManager)
    {
        _context = context;
        _plantService = plantService;
        _userManager = userManager;
    }

    public List<GardenTask> Tasks { get; set; } = new();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            Tasks = new List<GardenTask>();
            return;
        }

        Tasks = await _context.GardenTasks
            .Where(t => t.Plant!.UserId == user.Id)
            .Include(t => t.Plant)
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostToggleCompleteAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        var task = await _context.GardenTasks.Include(t => t.Plant).FirstOrDefaultAsync(t => t.Id == id && t.Plant!.UserId == user.Id);
        if (task == null) return RedirectToPage();

        task.IsCompleted = !task.IsCompleted;
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        var task = await _context.GardenTasks.Include(t => t.Plant).FirstOrDefaultAsync(t => t.Id == id && t.Plant!.UserId == user.Id);
        if (task == null) return RedirectToPage();

        _context.GardenTasks.Remove(task);
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }
}
