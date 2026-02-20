using GrowMate.Application.Services;
using GrowMate.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrowMate.Presentation.Pages.GardenBeds;

public class EditModel : PageModel
{
    private readonly GardenBedService _gardenBedService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        GardenBedService gardenBedService,
        UserManager<User> userManager,
        ILogger<EditModel> logger)
    {
        _gardenBedService = gardenBedService;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public UpdateGardenBedDto EditGardenBed { get; set; } = new() { Name = string.Empty };

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogWarning("User not found during OnGet");
            return Unauthorized();
        }

        var gardenBed = await _gardenBedService.GetGardenBedByIdAsync(id, user.Id);
        if (gardenBed == null)
        {
            _logger.LogWarning("GardenBed not found. GardenBedId: {GardenBedId}, UserId: {UserId}", id, user.Id);
            return NotFound();
        }

        EditGardenBed = new UpdateGardenBedDto { Name = gardenBed.Name, Type = gardenBed.Type };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogWarning("User not found during OnPost");
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            ErrorMessage = "Пожалуйста, заполните все поля корректно.";
            return Page();
        }

        var success = await _gardenBedService.UpdateGardenBedAsync(id, EditGardenBed, user.Id);
        if (!success)
        {
            _logger.LogError("Failed to update GardenBed. GardenBedId: {GardenBedId}", id);
            ErrorMessage = "Не удалось обновить грядку.";
            return Page();
        }

        TempData["SuccessMessage"] = "Грядка успешно обновлена!";
        return RedirectToPage("Index");
    }
}

