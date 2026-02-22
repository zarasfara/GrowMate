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
public class CreateModel : PageModel
{
    private readonly ApplicationContext _context;
    private readonly PlantService _plantService;
    private readonly UserManager<User> _userManager;

    public CreateModel(ApplicationContext context, PlantService plantService, UserManager<User> userManager)
    {
        _context = context;
        _plantService = plantService;
        _userManager = userManager;
    }

    [BindProperty]
    public GardenTask Task { get; set; } = new();

    public List<(int Id, string Name)> Plants { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? plantId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        var plants = await _plantService.GetAllPlantsAsync(user.Id);
        Plants = plants.Select(p => (p.Id, p.Name)).ToList();

        if (plantId.HasValue)
        {
            Task.PlantId = plantId.Value;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        if (string.IsNullOrWhiteSpace(Task.Title))
        {
            ModelState.AddModelError("Task.Title", "Заголовок обязателен");
        }

        if (!ModelState.IsValid)
        {
            var plants = await _plantService.GetAllPlantsAsync(user.Id);
            Plants = plants.Select(p => (p.Id, p.Name)).ToList();
            return Page();
        }

        // ensure plant belongs to user
        var plant = await _context.Plants.FirstOrDefaultAsync(p => p.Id == Task.PlantId && p.UserId == user.Id);
        if (plant == null)
        {
            ModelState.AddModelError(string.Empty, "Выберите корректное растение.");
            var plants = await _plantService.GetAllPlantsAsync(user.Id);
            Plants = plants.Select(p => (p.Id, p.Name)).ToList();
            return Page();
        }

        Task.DueDate = Task.DueDate.Date; // store date-only
        _context.GardenTasks.Add(Task);
        await _context.SaveChangesAsync();
        return RedirectToPage("/GardenCare/Index");
    }
}
