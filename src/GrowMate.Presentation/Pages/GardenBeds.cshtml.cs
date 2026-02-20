using GrowMate.Domain.Users;
using GrowMate.Presentation.Models;
using GrowMate.Presentation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrowMate.Presentation.Pages;

public class GardenBedsModel : PageModel
{
    private readonly GardenBedService _gardenBedService;
    private readonly UserManager<User> _userManager;

    [BindProperty]
    public CreateGardenBedDto NewGardenBed { get; set; } = new();

    public List<GardenBedDto> GardenBeds { get; set; } = [];
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public GardenBedsModel(GardenBedService gardenBedService, UserManager<User> userManager)
    {
        _gardenBedService = gardenBedService;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        GardenBeds = await _gardenBedService.GetUserGardenBedsAsync(user.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(NewGardenBed.Name))
        {
            GardenBeds = await _gardenBedService.GetUserGardenBedsAsync(user.Id);
            ErrorMessage = "Пожалуйста, укажите название грядки.";
            return Page();
        }

        if (!ModelState.IsValid)
        {
            GardenBeds = await _gardenBedService.GetUserGardenBedsAsync(user.Id);
            ErrorMessage = "Пожалуйста, заполните все поля корректно.";
            return Page();
        }

        try
        {
            await _gardenBedService.CreateGardenBedAsync(NewGardenBed, user.Id);
            SuccessMessage = "Грядка успешно создана!";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при создании грядки: {ex.Message}";
            GardenBeds = await _gardenBedService.GetUserGardenBedsAsync(user.Id);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var success = await _gardenBedService.DeleteGardenBedAsync(id, user.Id);
        if (success)
        {
            SuccessMessage = "Грядка успешно удалена!";
        }
        else
        {
            ErrorMessage = "Не удалось удалить грядку.";
        }

        return RedirectToPage();
    }
}


