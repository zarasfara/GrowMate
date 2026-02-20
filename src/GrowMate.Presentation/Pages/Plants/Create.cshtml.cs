using GrowMate.Applicatoin.DTOs;
using GrowMate.Applicatoin.Services;
using GrowMate.Application.Services;
using GrowMate.Domain.Plants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace GrowMate.Presentation.Pages.Plants;

[Authorize]
public class CreateModel : PageModel
{
    private readonly PlantService _plantService;
    private readonly GardenBedService _gardenBedService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(PlantService plantService, GardenBedService gardenBedService, ILogger<CreateModel> logger)
    {
        _plantService = plantService;
        _gardenBedService = gardenBedService;
        _logger = logger;
    }

    [BindProperty]
    public CreatePlantDto NewPlant { get; set; } = new() 
    { 
        Name = string.Empty, 
        GardenBedId = 0,
        PlantingDate = DateTime.Today,
        Quantity = 1,
        IsUnique = false
    };

    public List<SelectListItem> GardenBeds { get; set; } = [];
    public List<SelectListItem> PlantTypes { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int? gardenBedId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID not found in claims");
            return RedirectToPage("/Account/Login");
        }

        await LoadSelectLists(userId);

        if (gardenBedId.HasValue)
        {
            NewPlant.GardenBedId = gardenBedId.Value;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID not found in claims");
            return RedirectToPage("/Account/Login");
        }

        // Дополнительная проверка даты на стороне сервера
        if (NewPlant.PlantingDate.Date > DateTime.Today)
        {
            ModelState.AddModelError("NewPlant.PlantingDate", "Дата посадки не может быть в будущем");
        }

        if (!ModelState.IsValid)
        {
            await LoadSelectLists(userId);
            return Page();
        }

        try
        {
            await _plantService.CreatePlantAsync(NewPlant, userId);
            TempData["SuccessMessage"] = "Посадка успешно создана!";
            return RedirectToPage("/Plants/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plant");
            ModelState.AddModelError(string.Empty, "Произошла ошибка при создании посадки.");
            await LoadSelectLists(userId);
            return Page();
        }
    }

    private async Task LoadSelectLists(string userId)
    {
        var gardenBeds = await _gardenBedService.GetUserGardenBedsAsync(userId);
        GardenBeds = gardenBeds.Select(gb => new SelectListItem
        {
            Value = gb.Id.ToString(),
            Text = gb.Name
        }).ToList();

        PlantTypes = Enum.GetValues<PlantType>()
            .Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),
                Text = GetPlantTypeDisplayName(t)
            }).ToList();
    }

    private static string GetPlantTypeDisplayName(PlantType type) => type switch
    {
        PlantType.Vegetable => "🥕 Овощ",
        PlantType.Fruit => "🍎 Фрукт",
        PlantType.Berry => "🍓 Ягода",
        PlantType.Flower => "🌸 Цветок",
        PlantType.Tree => "🌳 Дерево",
        PlantType.Herb => "🌿 Зелень/Травы",
        PlantType.Other => "🌱 Другое",
        _ => type.ToString()
    };
}




