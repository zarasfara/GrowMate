using GrowMate.Applicatoin.DTOs;
using GrowMate.Applicatoin.Services;
using GrowMate.Application.Services;
using GrowMate.Domain.Plants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EditModel(
        PlantService plantService,
        GardenBedService gardenBedService,
        ILogger<EditModel> logger,
        IWebHostEnvironment webHostEnvironment)
    {
        _plantService = plantService;
        _gardenBedService = gardenBedService;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    [BindProperty]
    public UpdatePlantDto EditPlant { get; set; } = new()
    {
        Name = string.Empty
    };

    [BindProperty]
    public IFormFile? PlantImage { get; set; }

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
            ImagePath = plant.ImagePath,
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

        if (PlantImage is not null)
        {
            ValidateImage(PlantImage, nameof(PlantImage));
        }

        if (!ModelState.IsValid)
        {
            await LoadSelectLists(userId);
            return Page();
        }

        try
        {
            string? oldImagePath = null;
            if (PlantImage is not null)
            {
                var currentPlant = await _plantService.GetPlantByIdAsync(EditPlant.Id, userId);
                oldImagePath = currentPlant?.ImagePath;
                EditPlant.ImagePath = await SaveImageAsync(PlantImage);
            }

            var result = await _plantService.UpdatePlantAsync(EditPlant, userId);
            if (result)
            {
                if (PlantImage is not null)
                {
                    DeleteImageIfExists(oldImagePath);
                }

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

    private async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "plants");
        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsFolder, fileName);

        await using var stream = System.IO.File.Create(fullPath);
        await file.CopyToAsync(stream);

        return $"/uploads/plants/{fileName}";
    }

    private void DeleteImageIfExists(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        var relativePath = imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
        if (!System.IO.File.Exists(fullPath))
        {
            return;
        }

        try
        {
            System.IO.File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete old image file {ImagePath}", fullPath);
        }
    }

    private void ValidateImage(IFormFile file, string key)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            ModelState.AddModelError(key, "Допустимые форматы: JPG, PNG, WEBP.");
        }

        const long maxSize = 5 * 1024 * 1024;
        if (file.Length > maxSize)
        {
            ModelState.AddModelError(key, "Размер файла не должен превышать 5 MB.");
        }
    }
}




