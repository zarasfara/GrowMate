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
public class EditModel : PageModel
{
    private readonly ApplicationContext _context;
    private readonly PlantService _plantService;
    private readonly UserManager<User> _userManager;

    public EditModel(ApplicationContext context, PlantService plantService, UserManager<User> userManager)
    {
        _context = context;
        _plantService = plantService;
        _userManager = userManager;
    }

    [BindProperty]
    public GardenTask Task { get; set; } = new();

    public List<(int Id, string Name)> Plants { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        var task = await _context.GardenTasks.Include(t => t.Plant).FirstOrDefaultAsync(t => t.Id == id && t.Plant!.UserId == user.Id);
        if (task == null) return RedirectToPage("/GardenCare/Index");

        Task = task;
        var plants = await _plantService.GetAllPlantsAsync(user.Id);
        Plants = plants.Select(p => (p.Id, p.Name)).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid) {
            var plants = await _plantService.GetAllPlantsAsync(user.Id);
            Plants = plants.Select(p => (p.Id, p.Name)).ToList();
            return Page();
        }

        var existing = await _context.GardenTasks.Include(t => t.Plant).FirstOrDefaultAsync(t => t.Id == Task.Id && t.Plant!.UserId == user.Id);
        if (existing == null) return RedirectToPage("/GardenCare/Index");

        existing.Title = Task.Title;
        existing.Type = Task.Type;
        existing.DueDate = Task.DueDate.Date;
        existing.PlantId = Task.PlantId;
        existing.IsCompleted = Task.IsCompleted;

        await _context.SaveChangesAsync();
        return RedirectToPage("/GardenCare/Index");
    }
}
