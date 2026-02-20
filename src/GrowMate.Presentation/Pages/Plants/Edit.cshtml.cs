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
public class EditModel : PageModel
{
    private readonly PlantService _plantService;
    private readonly GardenBedService _gardenBedService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(PlantService plantService, GardenBedService gardenBedService, ILogger<EditModel> logger)
    {
        _plantService = plantService;
        _gardenBedService = gardenBedService;
        _logger = logger;
    }

    [BindProperty]
    public UpdatePlantDto EditPlant { get; set; } = new()
    {
        Name = string.Empty
    };

    public List<SelectListItem> GardenBeds { get; set; } = [];
    public List<SelectListItem> PlantTypes { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID not found in claims");
            return RedirectToPage("/Account/Login");
        }

        var plant = await _plantService.GetPlantByIdAsync(id, userId);
        if (plant == null)
        {
            _logger.LogWarning("Plant with ID {PlantId} not found", id);
            return NotFound();
        }

        EditPlant = new UpdatePlantDto
        {
            Id = plant.Id,
            Name = plant.Name,
            Variety = plant.Variety,
            Description = plant.Description,
            PlantingDate = plant.PlantingDate,
            Type = plant.Type,
            Quantity = plant.Quantity,
            IsUnique = plant.IsUnique,
            PlantTemplateId = plant.PlantTemplateId,
            GardenBedId = plant.GardenBedId
        };

        await LoadSelectLists(userId);

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
        if (EditPlant.PlantingDate.Date > DateTime.Today)
        {
            ModelState.AddModelError("EditPlant.PlantingDate", "Дата посадки не может быть в будущем");
        }

        if (!ModelState.IsValid)
        {
            await LoadSelectLists(userId);
            return Page();
        }

        try
        {
            var result = await _plantService.UpdatePlantAsync(EditPlant, userId);
            if (result)
            {
                TempData["SuccessMessage"] = "Посадка успешно обновлена!";
                return RedirectToPage("/Plants/Details", new { id = EditPlant.Id });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Посадка не найдена.");
                await LoadSelectLists(userId);
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating plant with ID {PlantId}", EditPlant.Id);
            ModelState.AddModelError(string.Empty, "Произошла ошибка при обновлении посадки.");
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




