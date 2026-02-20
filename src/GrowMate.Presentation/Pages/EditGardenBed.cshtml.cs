using GrowMate.Domain.Users;
using GrowMate.Infrastructure;
using GrowMate.Presentation.Models;
using GrowMate.Presentation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GrowMate.Presentation.Pages;

public class EditGardenBedModel : PageModel
{
    private readonly GardenBedService _gardenBedService;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationContext _context;

    [BindProperty]
    public UpdateGardenBedDto EditGardenBed { get; set; } = new();

    public int GardenBedId { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public EditGardenBedModel(
        GardenBedService gardenBedService, 
        UserManager<User> userManager,
        ApplicationContext context)
    {
        _gardenBedService = gardenBedService;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var gardenBed = await _gardenBedService.GetGardenBedByIdAsync(id, user.Id);
        if (gardenBed == null)
            return NotFound();

        GardenBedId = id;
        EditGardenBed.Name = gardenBed.Name;
        EditGardenBed.Type = gardenBed.Type;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(EditGardenBed.Name))
        {
            GardenBedId = id;
            ErrorMessage = "Пожалуйста, укажите название грядки.";
            return Page();
        }

        if (!ModelState.IsValid)
        {
            GardenBedId = id;
            ErrorMessage = "Пожалуйста, заполните все поля корректно.";
            return Page();
        }

        var success = await _gardenBedService.UpdateGardenBedAsync(id, EditGardenBed, user.Id);
        if (success)
        {
            TempData["SuccessMessage"] = "Грядка успешно обновлена!";
            return RedirectToPage("GardenBeds");
        }

        ErrorMessage = "Не удалось обновить грядку.";
        GardenBedId = id;
        return Page();
    }
}


