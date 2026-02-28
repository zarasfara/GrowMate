﻿using GrowMate.Applicatoin.DTOs;
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
public class CreateModel : PageModel
{
    private readonly PlantService _plantService;
    private readonly GardenBedService _gardenBedService;
    private readonly ILogger<CreateModel> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CreateModel(
        PlantService plantService,
        GardenBedService gardenBedService,
        ILogger<CreateModel> logger,
        IWebHostEnvironment webHostEnvironment)
    {
        _plantService = plantService;
        _gardenBedService = gardenBedService;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
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

    [BindProperty]
    public List<IFormFile> PlantImages { get; set; } = [];

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

        if (PlantImages.Count > 0)
        {
            ValidateImages(PlantImages, nameof(PlantImages));
        }

        if (!ModelState.IsValid)
        {
            await LoadSelectLists(userId);
            return Page();
        }

        try
        {
            NewPlant.ImagePaths = await SaveImagesAsync(PlantImages);
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

    private async Task<List<string>> SaveImagesAsync(IEnumerable<IFormFile> files)
    {
        var savedPaths = new List<string>();
        foreach (var file in files)
        {
            if (file.Length == 0)
            {
                continue;
            }

            var savedPath = await SaveImageAsync(file);
            if (!string.IsNullOrWhiteSpace(savedPath))
            {
                savedPaths.Add(savedPath);
            }
        }

        return savedPaths;
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

    private void ValidateImages(IEnumerable<IFormFile> files, string key)
    {
        foreach (var file in files)
        {
            ValidateImage(file, key);
        }
    }
}